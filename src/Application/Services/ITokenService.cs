namespace Azoxia.Core.Application.Services
{
    using System.Security.Claims;

    /// <summary>
    /// Issues JWT access and refresh tokens for authenticated callers.
    /// </summary>
    public interface ITokenService
    {
        #region Methods

        /// <summary>
        /// Creates a signed access token for the supplied claims.
        /// </summary>
        /// <param name="claims">Claims embedded in the access token.</param>
        /// <returns>The token string and its absolute expiry time.</returns>
        (string Token, DateTime ExpiresAt) GenerateAccessToken(params Claim[] claims);

        /// <summary>
        /// Creates a signed refresh token.
        /// </summary>
        /// <returns>The token string and its absolute expiry time.</returns>
        (string Token, DateTime ExpiresAt) GenerateRefreshToken();

        #endregion Methods
    }
}
