namespace Azoxia.Core.Persistence.DependencyInjection
{
    using Azoxia.Core.Configuration;
    using Azoxia.Core.DependencyInjection;
    using Azoxia.Core.Persistence.Configuration;
    using Azoxia.Core.Persistence.DbLogging;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Registers persistence-oriented services and exposes EF-backed logging registration for the host <see cref="ILoggingBuilder"/>.
    /// </summary>
    public class ServiceRegister :
        IServiceRegister
    {
        #region Methods

        /// <summary>
        /// Registers persistence-oriented services into <paramref name="services"/>.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configuration">Application configuration (EF log bölümü için <see cref="Config.GetOrCreateConfig{TConfig}"/> önbelleği).</param>
        public void Register(IServiceCollection services, IConfiguration configuration)
        {
            _ = Config.GetOrCreateConfig<EfLoggingConfig>(configuration);
        }

        /// <summary>
        /// Registers the Azoxia EF Core <see cref="ILoggerProvider"/> that writes to <see cref="AppLog"/>.
        /// Çağrılmadan önce barındırıcıda <c>AddDbContextFactory&lt;TDbContext&gt;(...)</c> kayıtlı olmalıdır.
        /// </summary>
        /// <typeparam name="TDbContext">Somut <see cref="DbContext"/>; <see cref="AppLog"/> eşlemesi Persistence derlemesindedir.</typeparam>
        /// <param name="builder">Uygulama hostunun <see cref="ILoggingBuilder"/> örneği (ör. <c>WebApplicationBuilder.Logging</c>).</param>
        /// <param name="configuration">Uygulama yapılandırması.</param>
        /// <returns>Aynı <paramref name="builder"/> örneği.</returns>
        public static ILoggingBuilder AddAzoxiaEfLogging<TDbContext>(ILoggingBuilder builder, IConfiguration configuration)
            where TDbContext : DbContext
        {
            ArgumentNullException.ThrowIfNull(builder);
            ArgumentNullException.ThrowIfNull(configuration);

            builder.Services.TryAddSingleton(_ => Config.GetOrCreateConfig<EfLoggingConfig>(configuration));
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, EfLoggerProvider<TDbContext>>());
            return builder;
        }

        #endregion Methods
    }
}
