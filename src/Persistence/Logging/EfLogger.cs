namespace Azoxia.Core.Persistence.DbLogging
{
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Per-category logger that forwards entries to <see cref="EfLoggerProvider{TDbContext}"/>.
    /// </summary>
    internal sealed class EfLogger<TDbContext> :
        ILogger
        where TDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        #region Fields

        private readonly string _categoryName;

        private readonly EfLoggerProvider<TDbContext> _provider;

        #endregion Fields

        #region Ctors

        public EfLogger(EfLoggerProvider<TDbContext> provider, string categoryName)
        {
            _provider = provider;
            _categoryName = categoryName;
        }

        #endregion Ctors

        #region Methods

        /// <inheritdoc />
        public IDisposable? BeginScope<TState>(TState state)
            where TState : notnull
        {
            return null;
        }

        /// <inheritdoc />
        public bool IsEnabled(LogLevel logLevel)
        {
            return _provider.IsEnabled(logLevel);
        }

        /// <inheritdoc />
        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (EfLoggerProvider<TDbContext>.ShouldSuppressCategory(_categoryName))
            {
                return;
            }

            ArgumentNullException.ThrowIfNull(formatter);
            string message = formatter(state, exception);
            string? exText = exception?.ToString();
            if (exText is { Length: > 8000 })
            {
                exText = exText[..8000];
            }

            _provider.Enqueue(new EfQueuedLogLine
            {
                Timestamp = DateTimeOffset.UtcNow,
                Level = logLevel,
                Category = _categoryName,
                Message = message.Length > 4000 ? message[..4000] : message,
                Exception = exText,
                EventId = eventId.Id,
                EventName = eventId.Name,
            });
        }

        #endregion Methods
    }
}
