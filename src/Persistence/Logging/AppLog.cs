namespace Azoxia.Core.Persistence.DbLogging
{
    /// <summary>
    /// Persisted application log row written by <see cref="EfLoggerProvider{TDbContext}"/>.
    /// </summary>
    public class AppLog
    {
        #region Properties

        /// <summary>
        /// Gets or sets the surrogate key.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the UTC timestamp when the log entry was created.
        /// </summary>
        public DateTimeOffset UtcTimestamp { get; set; }

        /// <summary>
        /// Gets or sets the numeric <see cref="Microsoft.Extensions.Logging.LogLevel"/> value.
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// Gets or sets the logger category name.
        /// </summary>
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the formatted log message.
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the exception text, if any.
        /// </summary>
        public string? Exception { get; set; }

        /// <summary>
        /// Gets or sets the optional structured event identifier.
        /// </summary>
        public int? EventId { get; set; }

        /// <summary>
        /// Gets or sets the optional event name paired with <see cref="EventId"/>.
        /// </summary>
        public string? EventName { get; set; }

        #endregion Properties
    }
}
