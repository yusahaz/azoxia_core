namespace Azoxia.Core.Infrastructure.FileLogging
{
    using Azoxia.Core.Infrastructure.Configuration;

    using Microsoft.Extensions.Logging;

    using System.Diagnostics;
    using System.Globalization;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Channels;

    /// <summary>
    /// Writes log entries to daily rolling UTF-8 JSON lines under <see cref="FileLoggingConfig.LogDirectory"/>.
    /// </summary>
    public sealed class FileLoggerProvider :
        ILoggerProvider,
        IDisposable
    {
        #region Fields

        private readonly FileLoggingConfig _config;

        private readonly Channel<QueuedLogLine> _channel;

        private readonly CancellationTokenSource _cts = new();

        private readonly Task _writerTask;

        private LogLevel _minimumLevel = LogLevel.Information;

        private bool _disposed;

        #endregion Fields

        #region Ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="FileLoggerProvider"/> class.
        /// </summary>
        /// <param name="config">Bound file logging configuration.</param>
        public FileLoggerProvider(FileLoggingConfig config)
        {
            ArgumentNullException.ThrowIfNull(config);
            _config = config;
            _minimumLevel = ParseMinLevel(config.MinimumLevel);

            _channel = Channel.CreateUnbounded<QueuedLogLine>(new UnboundedChannelOptions
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
                Trace.WriteLine($"FileLoggerProvider dispose: {ex}");
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

            return new FileLogger(this, categoryName);
        }

        internal bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None && logLevel >= _minimumLevel;
        }

        internal void Enqueue(in QueuedLogLine line)
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
                return LogLevel.Information;
            }

            return Enum.TryParse(name, ignoreCase: true, out LogLevel level) ? level : LogLevel.Information;
        }

        private static string ResolveLogDirectory(string logDirectory)
        {
            if (Path.IsPathRooted(logDirectory))
            {
                return logDirectory;
            }

            return Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, logDirectory));
        }

        private async Task WriterLoopAsync()
        {
            JsonSerializerOptions jsonOptions = new() { WriteIndented = false };

            string dir = ResolveLogDirectory(_config.LogDirectory);
            Directory.CreateDirectory(dir);

            string prefix = string.IsNullOrWhiteSpace(_config.FileNamePrefix) ? "app" : _config.FileNamePrefix.Trim();
            DateOnly currentDay = DateOnly.FromDateTime(DateTime.UtcNow);
            StreamWriter? writer = null;

            try
            {
                await foreach (QueuedLogLine line in _channel.Reader.ReadAllAsync(_cts.Token).ConfigureAwait(false))
                {
                    DateOnly day = DateOnly.FromDateTime(line.Timestamp.UtcDateTime);
                    if (writer is null || day != currentDay)
                    {
                        currentDay = day;
                        if (writer is not null)
                        {
                            await writer.DisposeAsync().ConfigureAwait(false);
                            writer = null;
                        }

                        string fileName = $"{prefix}-{day:yyyyMMdd}.log";
                        string path = Path.Combine(dir, fileName);
                        writer = new StreamWriter(
                            new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.Read),
                            new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
                        TryDeleteOldFiles(dir, prefix);
                    }

                    if (writer is null)
                    {
                        continue;
                    }

                    string json = JsonSerializer.Serialize(
                        new
                        {
                            t = line.Timestamp.ToString("O", CultureInfo.InvariantCulture),
                            l = line.Level.ToString(),
                            c = line.Category,
                            m = line.Message,
                            e = line.Exception,
                            id = line.EventId,
                            en = line.EventName,
                        },
                        jsonOptions);

                    await writer.WriteLineAsync(json).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"FileLoggerProvider writer: {ex}");
            }
            finally
            {
                if (writer is not null)
                {
                    await writer.DisposeAsync().ConfigureAwait(false);
                }
            }
        }

        private void TryDeleteOldFiles(string dir, string prefix)
        {
            int keep = Math.Max(0, _config.RetainedFileCountLimit);
            if (keep == 0)
            {
                return;
            }

            try
            {
                string pattern = $"{prefix}-*.log";
                string[] files = Directory.GetFiles(dir, pattern);
                if (files.Length <= keep)
                {
                    return;
                }

                Array.Sort(files, StringComparer.Ordinal);
                int remove = files.Length - keep;
                for (int i = 0; i < remove; i++)
                {
                    try
                    {
                        File.Delete(files[i]);
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine($"FileLoggerProvider retention delete '{files[i]}': {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"FileLoggerProvider retention: {ex.Message}");
            }
        }

        #endregion Methods
    }
}
