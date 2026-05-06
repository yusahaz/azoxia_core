namespace Azoxia.Core.Api.DependencyInjection
{
    using System;
    using System.Text;
    using Azoxia.Core.Configuration;
    using Azoxia.Core.Exceptions;
    using Azoxia.Core.Extensions;
    using Azoxia.Core.Infrastructure.Configuration;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.IdentityModel.Tokens;

    /// <summary>
    /// Registers JWT Bearer authentication using the shared <see cref="JwtConfig"/> section (<c>JwtConfig</c>) and symmetric validation parameters.
    /// </summary>
    public static class JwtAuthenticationServiceCollectionExtensions
    {
        #region Methods

        /// <summary>
        /// Adds JWT Bearer authentication configured from <see cref="Config.GetOrCreateConfig{T}(IConfiguration)"/> for <see cref="JwtConfig"/>.
        /// </summary>
        /// <param name="services">The host service collection.</param>
        /// <param name="configuration">Application configuration root.</param>
        /// <returns><paramref name="services"/> for chaining.</returns>
        /// <exception cref="AzoxiaException">JWT configuration is missing required values or fails guard checks.</exception>
        public static IServiceCollection AddAzoxiaJwtBearerAuthentication(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            JwtConfig jwt = Config.GetOrCreateConfig<JwtConfig>(configuration);

            jwt.Key.ThrowIfNullOrWhiteSpace();
            (jwt.Key.Length >= 32).ThrowIfFalse(AzoxiaErrorCodes.JwtConfigSigningKeyTooShort);
            jwt.Issuer.ThrowIfNullOrWhiteSpace();
            jwt.Audience.ThrowIfNullOrWhiteSpace();

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(jwt.KeyBytes),
                        ValidateIssuer = true,
                        ValidIssuer = jwt.Issuer,
                        ValidateAudience = true,
                        ValidAudience = jwt.Audience,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromMinutes(1),
                    };
                });

            return services;
        }

        #endregion Methods
    }
}
