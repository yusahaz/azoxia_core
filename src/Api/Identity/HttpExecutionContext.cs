namespace Azoxia.Api.Identity
{
    using Azoxia.Core.Identity;
    using System.Security.Claims;

    internal class HttpExecutionContext :
        IExecutionContext
    {
        #region Fields

        private readonly IHttpContextAccessor _httpContextAccessor;

        private ILookup<string, string>? _claimsCache;

        #endregion Fields

        #region Ctor

        public HttpExecutionContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        #endregion Ctor

        #region Utils

        /// <summary>
        /// 
        /// </summary>
        private void EnsureClaimsCached()
        {
            if (_claimsCache != null)
            {
                return;
            }

            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || user.Claims == null)
            {
                _claimsCache = Enumerable.Empty<Claim>().ToLookup(x => x.Type, x => x.Value);
                return;
            }

            _claimsCache = user.Claims.ToLookup(c => c.Type, c => c.Value);
        }

        #endregion Utils

        #region Methods

        /// <inheritdoc/>
        public string? GetClaim(string claimType)
        {
            EnsureClaimsCached();
            return _claimsCache?[claimType].FirstOrDefault();
        }

        /// <inheritdoc/>
        public IReadOnlyList<string> GetClaims(string claimType)
        {
            EnsureClaimsCached();
            return _claimsCache?[claimType].ToArray() ?? [];
        }

        #endregion Methods

        #region Properties

        /// <inheritdoc/>
        public bool IsAuthenticated =>
             _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

        #endregion Properties
    }
}
