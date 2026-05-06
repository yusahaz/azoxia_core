namespace Azoxia.Core.Configuration
{
    using Microsoft.Extensions.Configuration;
    using System.Collections.Concurrent;

    /// <summary>
    /// Resolves strongly typed configuration sections backed by a process-wide cache.
    /// </summary>
    public class Config
    {
        #region Methods

        /// <summary>
        /// Returns a previously registered configuration instance of type <typeparamref name="TConfig"/>.
        /// </summary>
        /// <typeparam name="TConfig">The configuration type.</typeparam>
        /// <returns>The cached configuration.</returns>
        /// <exception cref="KeyNotFoundException">No configuration was registered for this type.</exception>
        public static TConfig GetConfig<TConfig>()
            where TConfig : class, IConfig
            => ConfigSingleton.Instance.GetConfig<TConfig>();

        /// <summary>
        /// Gets or creates a configuration instance bound from <paramref name="configuration"/>.
        /// </summary>
        /// <typeparam name="TConfig">The configuration type.</typeparam>
        /// <param name="configuration">The application configuration root.</param>
        /// <returns>The configuration instance.</returns>
        public static TConfig GetOrCreateConfig<TConfig>(IConfiguration configuration)
            where TConfig : class, IConfig
            => ConfigSingleton.Instance.GetOrCreateConfig<TConfig>(configuration);

        #endregion Methods
    }

    /// <summary>
    /// Process-wide cache of bound <see cref="IConfig"/> instances keyed by configuration section name (the config type name).
    /// </summary>
    internal sealed class ConfigSingleton
    {
        #region Fields

        private readonly ConcurrentDictionary<string, IConfig> _configs;

        #endregion Fields

        #region Ctors

        private ConfigSingleton()
        {
            _configs = new();
        }

        static ConfigSingleton()
        {
        }

        #endregion Ctors

        #region Methods

        /// <summary>
        /// Returns a configuration instance previously added to the cache for <typeparamref name="TConfig"/>.
        /// </summary>
        /// <typeparam name="TConfig">The configuration type.</typeparam>
        /// <returns>The cached configuration.</returns>
        /// <exception cref="KeyNotFoundException">No entry exists for the section name derived from <typeparamref name="TConfig"/>.</exception>
        public TConfig GetConfig<TConfig>()
        {
            string sectionName = typeof(TConfig).Name;
            if (!_configs.TryGetValue(sectionName, out IConfig? config))
            {
                throw new KeyNotFoundException($"Config section '{sectionName}' not found. Make sure it has been created and added to the singleton.");
            }

            return (TConfig)config;
        }

        /// <summary>
        /// Gets or creates a configuration instance of type <typeparamref name="TConfig"/>, binding from <paramref name="configuration"/>.
        /// </summary>
        /// <typeparam name="TConfig">The configuration type.</typeparam>
        /// <param name="configuration">The application configuration root.</param>
        /// <returns>The shared configuration instance.</returns>
        public TConfig GetOrCreateConfig<TConfig>(IConfiguration configuration)
            where TConfig : class, IConfig
            => (TConfig)GetOrCreateConfig(configuration, typeof(TConfig));

        /// <summary>
        /// Gets or creates a configuration instance for <paramref name="configType"/>, binding from <paramref name="configuration"/>.
        /// </summary>
        /// <param name="configuration">The application configuration root.</param>
        /// <param name="configType">The concrete <see cref="IConfig"/> type.</param>
        /// <returns>The shared configuration instance for that section.</returns>
        /// <exception cref="InvalidOperationException"><paramref name="configType"/> cannot be instantiated as <see cref="IConfig"/>.</exception>
        public IConfig GetOrCreateConfig(IConfiguration configuration, Type configType)
        {
            string sectionName = configType.Name;

            return _configs.GetOrAdd(sectionName, _ =>
            {
                IConfig config = Activator.CreateInstance(configType) as IConfig ??
                    throw new InvalidOperationException($"Type '{configType.Name}' could not be instantiated as IConfig.");
                configuration.GetSection(sectionName).Bind(config);
                return config;
            });
        }

        #endregion Methods

        #region Properties

        /// <summary>
        /// Gets the singleton cache used by <see cref="Config"/>.
        /// </summary>
        public static readonly ConfigSingleton Instance = new();

        #endregion Properties
    }
}
