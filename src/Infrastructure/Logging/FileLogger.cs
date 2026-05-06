namespace Azoxia.Core.Infrastructure.FileLogging
{
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Per-category logger that forwards entries to <see cref="FileLoggerProvider"/>.
    /// </summary>
    internal sealed class FileLogger :
        ILogger
    {
        #region Fields

        private readonly string _categoryName;

        private readonly FileLoggerProvider _provider;

        #endregion Fields

        #region Ctors

        public FileLogger(FileLoggerProvider provider, string categoryName)
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

            ArgumentNullException.ThrowIfNull(formatter);
            string message = formatter(state, exception);
            string? exText = exception?.ToString();
            if (exText is { Length: > 32000 })
            {
                exText = exText[..32000];
            }

            _provider.Enqueue(new QueuedLogLine
            {
                Timestamp = DateTimeOffset.UtcNow,
                Level = logLevel,
                Category = _categoryName,
                Message = message,
                Exception = exText,
                EventId = eventId.Id,
                EventName = eventId.Name,
            });
        }

        #endregion Methods
    }
}
