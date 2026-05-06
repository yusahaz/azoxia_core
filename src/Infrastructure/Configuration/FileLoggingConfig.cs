namespace Azoxia.Core.Infrastructure.Configuration
{
    using Azoxia.Core.Configuration;

    /// <summary>
    /// File sink settings bound from the <c>FileLoggingConfig</c> configuration section.
    /// </summary>
    public record FileLoggingConfig :
        IConfig
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether the file logger provider is active.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the directory for log files (created if missing). Relative paths are resolved against the app base directory.
        /// </summary>
        public string LogDirectory { get; set; } = "logs";

        /// <summary>
        /// Gets or sets the file name prefix before the date suffix (for example <c>app</c> → <c>app-20260504.log</c>).
        /// </summary>
        public string FileNamePrefix { get; set; } = "app";

        /// <summary>
        /// Gets or sets the minimum <see cref="Microsoft.Extensions.Logging.LogLevel"/> name (for example <c>Information</c>, <c>Warning</c>).
        /// </summary>
        public string MinimumLevel { get; set; } = "Information";

        /// <summary>
        /// Gets or sets how many rotated daily files to keep in <see cref="LogDirectory"/>; older matching files are deleted after a new day file is opened.
        /// </summary>
        public int RetainedFileCountLimit { get; set; } = 7;

        #endregion Properties
    }
}
