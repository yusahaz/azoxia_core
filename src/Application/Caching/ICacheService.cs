namespace Azoxia.Core.Application.Caching
{
    /// <summary>
    /// L1/L2 read and write with dependency-based bulk invalidation.
    /// </summary>
    public interface ICacheService
    {
        #region Methods

        /// <summary>
        /// Reads from memory first, then distributed cache; may back-fill memory on L2 hit.
        /// </summary>
        /// <typeparam name="T">Serialized payload type.</typeparam>
        /// <param name="key">Logical key.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Cached value or default.</returns>
        Task<T?> GetAsync<T>(CacheKey key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes the entity key and all storage keys registered against the dependency (e.g. evict aggregate Country when City changes).
        /// </summary>
        /// <param name="dependency">The changed entity (e.g. updated City).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task InvalidateByDependencyAsync(CacheDependency dependency, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes the key from L1/L2 and unregisters dependency index entries.
        /// </summary>
        /// <param name="key">Logical key.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task RemoveAsync(CacheKey key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Writes L1 first, then L2 when <see cref="CacheEntryOptions.DistributedTtl"/> is set, and registers dependencies.
        /// </summary>
        /// <typeparam name="T">Value type.</typeparam>
        /// <param name="key">Logical key.</param>
        /// <param name="value">Value to cache.</param>
        /// <param name="options">TTL and dependencies.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task SetAsync<T>(CacheKey key, T value, CacheEntryOptions options, CancellationToken cancellationToken = default);

        #endregion Methods
    }
}
