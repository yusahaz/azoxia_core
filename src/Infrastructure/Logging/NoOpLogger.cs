namespace Azoxia.Core.Infrastructure.FileLogging
{
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Returned from <see cref="FileLoggerProvider.CreateLogger"/> when file logging is disabled.
    /// </summary>
    internal sealed class NoOpLogger :
        ILogger
    {
        #region Fields

        internal static readonly NoOpLogger Instance = new();

        #endregion Fields

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
            return false;
        }

        /// <inheritdoc />
        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
        }

        #endregion Methods
    }
}
