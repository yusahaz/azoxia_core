namespace Azoxia.Core.Persistence.DbLogging
{
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// One log entry queued for the EF writer task.
    /// </summary>
    internal readonly struct EfQueuedLogLine
    {
        #region Properties

        public DateTimeOffset Timestamp { get; init; }

        public LogLevel Level { get; init; }

        public string Category { get; init; }

        public string Message { get; init; }

        public string? Exception { get; init; }

        public int EventId { get; init; }

        public string? EventName { get; init; }

        #endregion Properties
    }
}
