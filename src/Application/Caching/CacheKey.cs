namespace Azoxia.Core.Application.Caching
{
    /// <summary>
    /// Logical cache key; produces a stable storage key string for L1/L2.
    /// </summary>
    public readonly record struct CacheKey
    {
        #region Fields

        private const string KeyVersion = "v1";

        #endregion Fields

        #region Ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheKey"/> struct.
        /// </summary>
        /// <param name="namespace">Logical namespace (e.g. <c>entity</c>, <c>query</c>).</param>
        /// <param name="type">Type or collection name (e.g. <c>Country</c>).</param>
        /// <param name="id">Instance identifier (e.g. <c>123</c>).</param>
        public CacheKey(string @namespace, string type, string id)
        {
            Namespace = @namespace;
            Type = type;
            Id = id;
        }

        #endregion Ctors

        #region Properties

        /// <summary>
        /// Gets the identifier segment.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets the logical namespace segment.
        /// </summary>
        public string Namespace { get; }

        /// <summary>
        /// Gets the type or collection name segment.
        /// </summary>
        public string Type { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Builds a key for a single entity (same pattern as dependency invalidation).
        /// </summary>
        /// <param name="entityType">Entity name (e.g. <c>City</c>).</param>
        /// <param name="entityId">Entity id.</param>
        /// <returns>A <see cref="CacheKey"/> under the <c>entity</c> namespace.</returns>
        public static CacheKey ForEntity(string entityType, string entityId)
            => new("entity", entityType, entityId);

        /// <summary>
        /// Storage key shared by <see cref="Microsoft.Extensions.Caching.Memory.IMemoryCache"/> and distributed cache.
        /// </summary>
        /// <returns>A single string key.</returns>
        public readonly string ToStorageKey()
            => $"{KeyVersion}|{Namespace}|{Type}|{Id}";

        #endregion Methods
    }
}
