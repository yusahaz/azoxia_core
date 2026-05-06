namespace Azoxia.Core.Infrastructure.FileLogging
{
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// One structured line queued for the file writer task.
    /// </summary>
    internal readonly struct QueuedLogLine
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
