namespace Azoxia.Core.Persistence.DbLogging
{
    using Azoxia.Core.Persistence.Configuration;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    using System.Diagnostics;
    using System.Threading.Channels;

    /// <summary>
    /// Batches log entries into <see cref="AppLog"/> rows using <see cref="IDbContextFactory{TContext}"/> so a singleton provider does not capture scoped <see cref="DbContext"/> instances.
    /// </summary>
    /// <typeparam name="TDbContext">Concrete <see cref="DbContext"/> type that includes <see cref="AppLog"/> mapping (via <see cref="AppLogConfiguration"/> in the Persistence assembly).</typeparam>
    public sealed class EfLoggerProvider<TDbContext> :
        ILoggerProvider,
        IDisposable
        where TDbContext : DbContext
    {
        #region Fields

        private readonly Channel<EfQueuedLogLine> _channel;

        private readonly EfLoggingConfig _config;

        private readonly IDbContextFactory<TDbContext> _dbContextFactory;

        private readonly CancellationTokenSource _cts = new();

        private readonly Task _writerTask;

        private LogLevel _minimumLevel = LogLevel.Warning;

        private bool _disposed;

        #endregion Fields

        #region Ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="EfLoggerProvider{TDbContext}"/> class.
        /// </summary>
        /// <param name="dbContextFactory">Factory used to create short-lived contexts per flush batch.</param>
        /// <param name="config">Bound EF logging configuration.</param>
        public EfLoggerProvider(
            IDbContextFactory<TDbContext> dbContextFactory,
            EfLoggingConfig config)
        {
            ArgumentNullException.ThrowIfNull(dbContextFactory);
            ArgumentNullException.ThrowIfNull(config);
            _dbContextFactory = dbContextFactory;
            _config = config;
            _minimumLevel = ParseMinLevel(config.MinimumLevel);

            _channel = Channel.CreateUnbounded<EfQueuedLogLine>(new UnboundedChannelOptions
            {
                SingleReader = true,
                SingleWriter = false,
            });

            if (config.Enabled)
            {
                _writerTask = Task.Run(WriterLoopAsync);
            }
            else
            {
                _writerTask = Task.CompletedTask;
                _channel.Writer.TryComplete();
            }
        }

        #endregion Ctors

        #region Methods

        /// <summary>
        /// Determines whether a category should be ignored to reduce feedback loops (for example EF Core internal logging while saving log rows).
        /// </summary>
        /// <param name="categoryName">Logger category.</param>
        /// <returns><see langword="true"/> if the category is suppressed.</returns>
        public static bool ShouldSuppressCategory(string categoryName)
        {
            if (string.IsNullOrEmpty(categoryName))
            {
                return false;
            }

            return categoryName.StartsWith("Microsoft.EntityFrameworkCore", StringComparison.Ordinal)
                || categoryName.StartsWith("Microsoft.Data.", StringComparison.Ordinal);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            _channel.Writer.TryComplete();
            _cts.Cancel();
            try
            {
                _writerTask.GetAwaiter().GetResult();
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"EfLoggerProvider dispose: {ex}");
            }

            _cts.Dispose();
        }

        /// <inheritdoc />
        public ILogger CreateLogger(string categoryName)
        {
            if (!_config.Enabled)
            {
                return NoOpLogger.Instance;
            }

            if (ShouldSuppressCategory(categoryName))
            {
                return NoOpLogger.Instance;
            }

            return new EfLogger<TDbContext>(this, categoryName);
        }

        internal bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None && logLevel >= _minimumLevel;
        }

        internal void Enqueue(in EfQueuedLogLine line)
        {
            if (!_config.Enabled || _disposed)
            {
                return;
            }

            _channel.Writer.TryWrite(line);
        }

        private static LogLevel ParseMinLevel(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return LogLevel.Warning;
            }

            return Enum.TryParse(name, ignoreCase: true, out LogLevel level) ? level : LogLevel.Warning;
        }

        private static AppLog Map(in EfQueuedLogLine line)
        {
            string cat = line.Category.Length > 512 ? line.Category[..512] : line.Category;
            return new AppLog
            {
                UtcTimestamp = line.Timestamp,
                Level = (int)line.Level,
                Category = cat,
                Message = line.Message,
                Exception = line.Exception,
                EventId = line.EventId == 0 ? null : line.EventId,
                EventName = line.EventName,
            };
        }

        private async Task WriterLoopAsync()
        {
            int batchSize = Math.Clamp(_config.BatchSize, 1, 500);
            int coalesceMs = Math.Clamp(_config.CoalesceMilliseconds, 0, 5000);

            try
            {
                while (await _channel.Reader.WaitToReadAsync(_cts.Token).ConfigureAwait(false))
                {
                    if (!_channel.Reader.TryRead(out EfQueuedLogLine first))
                    {
                        continue;
                    }

                    List<AppLog> batch = new(batchSize) { Map(first) };
                    DateTime deadline = DateTime.UtcNow.AddMilliseconds(coalesceMs);

                    while (batch.Count < batchSize && DateTime.UtcNow < deadline)
                    {
                        if (!_channel.Reader.TryRead(out EfQueuedLogLine next))
                        {
                            await Task.Delay(1, _cts.Token).ConfigureAwait(false);
                            continue;
                        }

                        batch.Add(Map(next));
                    }

                    while (batch.Count < batchSize && _channel.Reader.TryRead(out EfQueuedLogLine extra))
                    {
                        batch.Add(Map(extra));
                    }

                    await FlushAsync(batch).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"EfLoggerProvider writer: {ex}");
            }
        }

        private async Task FlushAsync(List<AppLog> batch)
        {
            if (batch.Count == 0)
            {
                return;
            }

            try
            {
                await using TDbContext ctx = await _dbContextFactory.CreateDbContextAsync(_cts.Token).ConfigureAwait(false);
                await using Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction tx =
                    await ctx.Database.BeginTransactionAsync(_cts.Token).ConfigureAwait(false);
                await ctx.Set<AppLog>().AddRangeAsync(batch, _cts.Token).ConfigureAwait(false);
                await ctx.SaveChangesAsync(_cts.Token).ConfigureAwait(false);
                await tx.CommitAsync(_cts.Token).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"EfLoggerProvider flush ({batch.Count} rows): {ex}");
            }
        }

        #endregion Methods
    }
}
