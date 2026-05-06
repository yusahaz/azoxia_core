namespace Azoxia.Core.Application.DependencyInjection
{
    using Azoxia.Core.DependencyInjection;
    using Azoxia.Core.Application.Validation;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Registers application-layer services (sender, handlers, etc.) into the DI container.
    /// </summary>
    public class ServiceRegister :
        IServiceRegister
    {
        #region Methods

        /// <summary>
        /// Registers application-layer services into <paramref name="services"/>.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="_">Unused configuration root (required by <see cref="IServiceRegister"/>).</param>
        public void Register(IServiceCollection services, IConfiguration _)
        {
            services.AddScoped<ISender, Sender>();
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));
        }

        #endregion Methods
    }
}
