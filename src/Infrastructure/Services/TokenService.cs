namespace Azoxia.Core.Infrastructure.Services
{
    using Azoxia.Core.Application.Services;
    using Azoxia.Core.Extensions;
    using Azoxia.Core.Infrastructure.Configuration;
    using Microsoft.IdentityModel.Tokens;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Security.Cryptography;

    /// <summary>
    /// Issues JWT access and refresh tokens using configured <see cref="JwtConfig"/>.
    /// </summary>
    internal class TokenService(JwtConfig jwtConfig) :
        ITokenService
    {
        #region Fields

        private const int REFRESH_TOKEN_BYTE_SIZE = 64;

        #endregion Fields

        #region Methods

        /// <inheritdoc />
        public (string Token, DateTime ExpiresAt) GenerateAccessToken(params Claim[] claims)
        {
            SymmetricSecurityKey securityKey = new(jwtConfig.KeyBytes);
            SigningCredentials credentials = new(securityKey, SecurityAlgorithms.HmacSha256);

            DateTimeOffset expiresAt = DateTimeOffset.UtcNow
                .AddMinutes(jwtConfig.ExpireMinutes);

            JwtSecurityToken token = new(
                issuer: jwtConfig.Issuer,
                audience: jwtConfig.Audience,
                claims: claims,
                expires: expiresAt.UtcDateTime,
                signingCredentials: credentials);

            JwtSecurityTokenHandler handler = new();

            return (handler.WriteToken(token), expiresAt.UtcDateTime);
        }

        /// <inheritdoc />
        public (string Token, DateTime ExpiresAt) GenerateRefreshToken()
            => (RandomNumberGenerator.GetBytes(REFRESH_TOKEN_BYTE_SIZE).ToBase64String(),
                DateTimeOffset.UtcNow.AddDays(jwtConfig.RefreshTokenExpireDays).UtcDateTime);

        #endregion Methods
    }
}
