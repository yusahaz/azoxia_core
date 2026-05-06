namespace Azoxia.Core.Application.Caching
{
    /// <summary>
    /// L1/L2 TTL and dependency list for a cache entry.
    /// </summary>
    public sealed class CacheEntryOptions
    {
        #region Properties

        /// <summary>
        /// Gets or sets dependency keys used when <see cref="ICacheService.InvalidateByDependencyAsync"/> is invoked.
        /// </summary>
        public IReadOnlyList<CacheDependency> Dependencies { get; set; } = Array.Empty<CacheDependency>();

        /// <summary>
        /// Gets or sets the distributed (L2) lifetime; <see langword="null"/> skips L2 write.
        /// </summary>
        public TimeSpan? DistributedTtl { get; set; } = TimeSpan.FromMinutes(30);

        /// <summary>
        /// Gets or sets the in-memory (L1) lifetime; keep short for fresher local reads.
        /// </summary>
        public TimeSpan MemoryTtl { get; set; } = TimeSpan.FromMinutes(2);

        #endregion Properties
    }
}
