namespace Azoxia.Core.Infrastructure.Services
{
    using System.Collections.Concurrent;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    using Azoxia.Core.Application.Caching;

    using Microsoft.Extensions.Caching.Distributed;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Logging;

    using StackExchange.Redis;

    /// <summary>
    /// Two-tier cache (memory L1, distributed L2) with dependency-keyed invalidation; optional Redis sets index cross-process dependencies.
    /// </summary>
    internal sealed class CacheService(
        IMemoryCache memoryCache,
        IDistributedCache distributedCache,
        ILogger<CacheService> logger,
        IConnectionMultiplexer? redisMultiplexer) :
        ICacheService
    {
        #region Fields

        private static readonly TimeSpan BackfillMemoryTtl = TimeSpan.FromMinutes(1);

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
        };

        private const string RedisDepPrefix = "azoxia:cache:dep:";

        private readonly IDistributedCache _distributedCache = distributedCache;

        private readonly ConcurrentDictionary<string, HashSet<string>> _localDepToKeys = new(StringComparer.Ordinal);

        private readonly ILogger<CacheService> _logger = logger;

        private readonly IMemoryCache _memoryCache = memoryCache;

        private readonly IDatabase? _redisDb = redisMultiplexer?.GetDatabase();

        private readonly ConcurrentDictionary<string, IReadOnlyList<CacheDependency>> _storageKeyToDeps = new(StringComparer.Ordinal);

        #endregion Fields

        #region Methods

        /// <inheritdoc />
        public async Task<T?> GetAsync<T>(CacheKey key, CancellationToken cancellationToken = default)
        {
            string storageKey = key.ToStorageKey();

            if (_memoryCache.TryGetValue(storageKey, out object? memObj) && memObj is T typedMem)
            {
                return typedMem;
            }

            byte[]? bytes = await _distributedCache.GetAsync(storageKey, cancellationToken).ConfigureAwait(false);
            if (bytes is null || bytes.Length == 0)
            {
                return default;
            }

            try
            {
                T? fromDistributed = JsonSerializer.Deserialize<T>(bytes, JsonOptions);
                if (fromDistributed is not null)
                {
                    _memoryCache.Set(
                        storageKey,
                        fromDistributed,
                        new MemoryCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = BackfillMemoryTtl,
                        });
                }

                return fromDistributed;
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "Cache deserialize failed for key {StorageKey}.", storageKey);
                return default;
            }
        }

        /// <inheritdoc />
        public async Task InvalidateByDependencyAsync(CacheDependency dependency, CancellationToken cancellationToken = default)
        {
            string primarySk = CacheKey.ForEntity(dependency.EntityType, dependency.EntityId).ToStorageKey();
            await RemoveStorageKeyFullyAsync(primarySk, cancellationToken).ConfigureAwait(false);

            string depKey = RedisDepKey(dependency);
            List<string> members = await GetDependencyMembersAsync(depKey, cancellationToken).ConfigureAwait(false);

            foreach (string memberSk in members)
            {
                if (string.Equals(memberSk, primarySk, StringComparison.Ordinal))
                {
                    continue;
                }

                await RemoveStorageKeyFullyAsync(memberSk, cancellationToken).ConfigureAwait(false);
            }

            if (_redisDb is not null)
            {
                await _redisDb.KeyDeleteAsync(depKey).ConfigureAwait(false);
            }
            else
            {
                _localDepToKeys.TryRemove(depKey, out _);
            }
        }

        /// <inheritdoc />
        public async Task RemoveAsync(CacheKey key, CancellationToken cancellationToken = default)
        {
            await RemoveStorageKeyFullyAsync(key.ToStorageKey(), cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task SetAsync<T>(CacheKey key, T value, CacheEntryOptions options, CancellationToken cancellationToken = default)
        {
            string storageKey = key.ToStorageKey();

            await UnregisterDependenciesForStorageKeyAsync(storageKey, cancellationToken).ConfigureAwait(false);

            _memoryCache.Set(
                storageKey,
                value!,
                new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = options.MemoryTtl,
                });

            if (options.DistributedTtl is { } distTtl)
            {
                byte[] payload = JsonSerializer.SerializeToUtf8Bytes(value, JsonOptions);
                await _distributedCache.SetAsync(
                    storageKey,
                    payload,
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = distTtl,
                    },
                    cancellationToken).ConfigureAwait(false);
            }

            await RegisterDependenciesAsync(storageKey, options.Dependencies, cancellationToken).ConfigureAwait(false);
        }

        #endregion Methods

        #region Utils

        private async Task<List<string>> GetDependencyMembersAsync(string depKey, CancellationToken _)
        {
            if (_redisDb is not null)
            {
                RedisValue[] vals = await _redisDb.SetMembersAsync(depKey).ConfigureAwait(false);
                List<string> list = new(vals.Length);
                foreach (RedisValue rv in vals)
                {
                    string? s = rv.ToString();
                    if (!string.IsNullOrEmpty(s))
                    {
                        list.Add(s);
                    }
                }

                return list;
            }

            if (_localDepToKeys.TryGetValue(depKey, out HashSet<string>? set))
            {
                lock (set)
                {
                    return set.ToList();
                }
            }

            return new List<string>();
        }

        private static string RedisDepKey(CacheDependency dependency)
            => $"{RedisDepPrefix}{dependency.ToIndexSegment()}";

        private async Task RegisterDependenciesAsync(
            string storageKey,
            IReadOnlyList<CacheDependency> dependencies,
            CancellationToken _)
        {
            if (dependencies.Count == 0)
            {
                return;
            }

            IReadOnlyList<CacheDependency> snapshot = dependencies.ToArray();

            _storageKeyToDeps[storageKey] = snapshot;

            foreach (CacheDependency dep in snapshot)
            {
                string depKey = RedisDepKey(dep);
                if (_redisDb is not null)
                {
                    await _redisDb.SetAddAsync(depKey, storageKey).ConfigureAwait(false);
                }
                else
                {
                    HashSet<string> set = _localDepToKeys.GetOrAdd(depKey, static _ => new HashSet<string>(StringComparer.Ordinal));
                    lock (set)
                    {
                        set.Add(storageKey);
                    }
                }
            }
        }

        private async Task RemoveStorageKeyFullyAsync(string storageKey, CancellationToken cancellationToken)
        {
            await UnregisterDependenciesForStorageKeyAsync(storageKey, cancellationToken).ConfigureAwait(false);
            _memoryCache.Remove(storageKey);
            await _distributedCache.RemoveAsync(storageKey, cancellationToken).ConfigureAwait(false);
        }

        private async Task UnregisterDependenciesForStorageKeyAsync(string storageKey, CancellationToken cancellationToken)
        {
            if (!_storageKeyToDeps.TryRemove(storageKey, out IReadOnlyList<CacheDependency>? deps))
            {
                return;
            }

            foreach (CacheDependency dep in deps)
            {
                string depKey = RedisDepKey(dep);
                if (_redisDb is not null)
                {
                    await _redisDb.SetRemoveAsync(depKey, storageKey).ConfigureAwait(false);
                }
                else if (_localDepToKeys.TryGetValue(depKey, out HashSet<string>? set))
                {
                    lock (set)
                    {
                        set.Remove(storageKey);
                    }
                }
            }
        }

        #endregion Utils
    }
}
