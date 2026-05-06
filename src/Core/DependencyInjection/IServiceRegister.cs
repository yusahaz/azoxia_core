namespace Azoxia.Core.DependencyInjection
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Registers services for a module or feature into the application <see cref="IServiceCollection"/>.
    /// </summary>
    public interface IServiceRegister
    {
        #region Methods

        /// <summary>
        /// Adds services required by this registration to <paramref name="services"/>.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configuration">Application configuration (binding, feature flags, connection strings).</param>
        void Register(IServiceCollection services, IConfiguration configuration);

        #endregion Methods
    }
}
