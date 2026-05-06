namespace Azoxia.Core.Identity
{
    using System.Collections.Generic;

    /// <summary>
    /// Read-only view of the current execution identity (authentication and claims).
    /// </summary>
    public interface IExecutionContext
    {
        #region Methods

        /// <summary>
        /// Returns the first claim value for <paramref name="claimType"/>, if present.
        /// </summary>
        /// <param name="claimType">The claim type URI or well-known name.</param>
        /// <returns>The claim value, or <c>null</c> when missing.</returns>
        string? GetClaim(string claimType);

        /// <summary>
        /// Returns all claim values for <paramref name="claimType"/>.
        /// </summary>
        /// <param name="claimType">The claim type URI or well-known name.</param>
        /// <returns>Zero or more values for that claim type.</returns>
        IReadOnlyList<string> GetClaims(string claimType);

        #endregion Methods

        #region Properties

        /// <summary>
        /// Gets a value indicating whether the caller is authenticated.
        /// </summary>
        bool IsAuthenticated { get; }

        #endregion Properties
    }
}
