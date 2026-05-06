namespace Azoxia.Core.Persistence.Configuration
{
    using Azoxia.Core.Configuration;

    /// <summary>
    /// EF Core database log sink settings bound from the <c>EfLoggingConfig</c> configuration section.
    /// </summary>
    public record EfLoggingConfig :
        IConfig
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether the EF log provider is active.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the minimum <see cref="Microsoft.Extensions.Logging.LogLevel"/> name (for example <c>Warning</c>).
        /// </summary>
        public string MinimumLevel { get; set; } = "Warning";

        /// <summary>
        /// Gets or sets the maximum number of rows to accumulate before calling <c>SaveChangesAsync</c>.
        /// </summary>
        public int BatchSize { get; set; } = 50;

        /// <summary>
        /// Gets or sets the coalescing window in milliseconds: after the first queued row, additional rows are read until this window elapses or <see cref="BatchSize"/> is reached.
        /// </summary>
        public int CoalesceMilliseconds { get; set; } = 50;

        #endregion Properties
    }
}
