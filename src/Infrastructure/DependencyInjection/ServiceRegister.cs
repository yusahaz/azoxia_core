namespace Azoxia.Core.Infrastructure.DependencyInjection
{
    using Azoxia.Core.Application.Caching;
    using Azoxia.Core.Application.Services;
    using Azoxia.Core.Configuration;
    using Azoxia.Core.DependencyInjection;
    using Azoxia.Core.Extensions;
    using Azoxia.Core.Infrastructure.Configuration;
    using Azoxia.Core.Infrastructure.FileLogging;
    using Azoxia.Core.Infrastructure.Services;

    using Microsoft.Extensions.Caching.Distributed;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Caching.StackExchangeRedis;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Logging;

    using StackExchange.Redis;

    /// <summary>
    /// Registers infrastructure services (tokens, Redis, distributed cache, <see cref="ICacheService"/>) with the DI container.
    /// </summary>
    public class ServiceRegister :
        IServiceRegister
    {
        #region Methods

        /// <summary>
        /// Adds infrastructure services to <paramref name="services"/> using <paramref name="configuration"/>.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configuration">Application configuration.</param>
        public void Register(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ITokenService, TokenService>();

            services.AddLogging(logging =>
            {
                logging.Services.TryAddSingleton(_ => Config.GetOrCreateConfig<FileLoggingConfig>(configuration));
                logging.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, FileLoggerProvider>());
            });

            RedisConfig redisConfig = Config.GetOrCreateConfig<RedisConfig>(configuration);

            if (!redisConfig.Enabled)
            {
                services.AddSingleton<IRedisMessageBus, NullRedisMessageBus>();
                services.AddDistributedMemoryCache();
            }
            else
            {
                redisConfig.ConnectionString.ThrowIfNullOrWhiteSpace();

                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = redisConfig.ConnectionString;
                    options.InstanceName = redisConfig.InstanceName;
                });

                services.AddSingleton<IConnectionMultiplexer>(_ =>
                    ConnectionMultiplexer.Connect(redisConfig.ConnectionString));

                services.AddSingleton<IRedisMessageBus, RedisMessageBus>();
            }

            services.AddSingleton<ICacheService>(static sp =>
                new CacheService(
                    sp.GetRequiredService<IMemoryCache>(),
                    sp.GetRequiredService<IDistributedCache>(),
                    sp.GetRequiredService<ILogger<CacheService>>(),
                    sp.GetService<IConnectionMultiplexer>()));
        }

        #endregion Methods
    }
}
