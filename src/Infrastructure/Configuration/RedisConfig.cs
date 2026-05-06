namespace Azoxia.Core.Infrastructure.Configuration
{
    using Azoxia.Core.Configuration;

    /// <summary>
    /// Redis connection and feature flags bound from the <c>RedisConfig</c> configuration section.
    /// </summary>
    public record RedisConfig :
        IConfig
    {
        #region Properties

        /// <summary>
        /// Gets or sets the StackExchange.Redis configuration string (host, port, password, SSL, etc.).
        /// </summary>
        public string ConnectionString { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether Redis-backed distributed cache, connection multiplexer, and <see cref="Azoxia.Core.Application.Services.IRedisMessageBus"/> are registered.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets a logical prefix applied to distributed cache keys (instance isolation).
        /// </summary>
        public string InstanceName { get; set; } = string.Empty;

        #endregion Properties
    }
}
