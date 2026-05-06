namespace Azoxia.Core.Application.Caching
{
    /// <summary>
    /// Invalidation unit: declares which entity changes should evict a cache entry.
    /// </summary>
    public readonly record struct CacheDependency
    {
        #region Ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheDependency"/> struct.
        /// </summary>
        /// <param name="entityType">Entity type name (e.g. <c>City</c>).</param>
        /// <param name="entityId">Entity identifier.</param>
        public CacheDependency(string entityType, string entityId)
        {
            EntityType = entityType;
            EntityId = entityId;
        }

        #endregion Ctors

        #region Properties

        /// <summary>
        /// Gets the entity identifier.
        /// </summary>
        public string EntityId { get; }

        /// <summary>
        /// Gets the entity type name (e.g. City, Country).
        /// </summary>
        public string EntityType { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Single segment for reverse index and Redis set naming.
        /// </summary>
        /// <returns>For example <c>City:321</c>.</returns>
        public readonly string ToIndexSegment()
            => $"{EntityType}:{EntityId}";

        #endregion Methods
    }
}
