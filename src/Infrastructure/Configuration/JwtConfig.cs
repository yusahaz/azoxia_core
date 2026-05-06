namespace Azoxia.Core.Infrastructure.Configuration
{
    using Azoxia.Core.Configuration;
    using System.Text;

    /// <summary>
    /// JWT signing and validation settings bound from configuration.
    /// </summary>
    public record JwtConfig :
        IConfig
    {
        #region Properties

        /// <summary>
        /// Gets or sets the intended JWT audience.
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        /// Gets or sets the access token lifetime in minutes.
        /// </summary>
        public int ExpireMinutes { get; set; }

        /// <summary>
        /// Gets or sets the JWT issuer.
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// Gets or sets the symmetric signing key (UTF-8 text).
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets the signing key material derived from <see cref="Key"/>.
        /// </summary>
        public byte[] KeyBytes => Encoding.UTF8.GetBytes(Key);

        /// <summary>
        /// Gets or sets the refresh token lifetime in days.
        /// </summary>
        public int RefreshTokenExpireDays { get; set; } = 7;

        #endregion Properties
    }
}
