using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Kooco.Pikachu.Performance
{
    /// <summary>
    /// Service caching implementation to reduce service resolution overhead
    /// Optimizes performance by caching expensive service operations
    /// </summary>
    public interface IServiceCachingService
    {
        /// <summary>
        /// Get or create a cached service instance
        /// </summary>
        T GetOrCreate<T>(string key, Func<T> factory, TimeSpan? expiration = null);

        /// <summary>
        /// Get or create a cached service instance asynchronously
        /// </summary>
        Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null);

        /// <summary>
        /// Remove a cached item
        /// </summary>
        void Remove(string key);

        /// <summary>
        /// Clear all cached items
        /// </summary>
        void Clear();
    }

    /// <summary>
    /// Implementation of service caching using IMemoryCache
    /// Registered as Singleton for application-wide caching
    /// </summary>
    public class ServiceCachingService : IServiceCachingService, ISingletonDependency
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<ServiceCachingService> _logger;
        private readonly ConcurrentDictionary<string, object> _lockObjects;

        // Default cache expiration times
        private static readonly TimeSpan DefaultExpiration = TimeSpan.FromMinutes(30);
        private static readonly TimeSpan ShortExpiration = TimeSpan.FromMinutes(5);

        public ServiceCachingService(
            IMemoryCache memoryCache,
            ILogger<ServiceCachingService> logger)
        {
            _memoryCache = memoryCache;
            _logger = logger;
            _lockObjects = new ConcurrentDictionary<string, object>();
        }

        public T GetOrCreate<T>(string key, Func<T> factory, TimeSpan? expiration = null)
        {
            if (_memoryCache.TryGetValue(key, out T cachedValue))
            {
                _logger.LogDebug("Cache hit for key: {Key}", key);
                return cachedValue;
            }

            // Use lock to prevent multiple threads from creating the same item
            var lockObject = _lockObjects.GetOrAdd(key, _ => new object());
            
            lock (lockObject)
            {
                // Double-check pattern
                if (_memoryCache.TryGetValue(key, out cachedValue))
                {
                    return cachedValue;
                }

                try
                {
                    _logger.LogDebug("Cache miss for key: {Key}, creating new instance", key);
                    var result = factory();
                    
                    var cacheOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = expiration ?? DefaultExpiration,
                        SlidingExpiration = TimeSpan.FromMinutes(10),
                        Priority = CacheItemPriority.Normal
                    };

                    _memoryCache.Set(key, result, cacheOptions);
                    _logger.LogDebug("Cached result for key: {Key}", key);
                    
                    return result;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating cached item for key: {Key}", key);
                    throw;
                }
                finally
                {
                    _lockObjects.TryRemove(key, out _);
                }
            }
        }

        public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null)
        {
            if (_memoryCache.TryGetValue(key, out T cachedValue))
            {
                _logger.LogDebug("Cache hit for key: {Key}", key);
                return cachedValue;
            }

            // Use lock to prevent multiple threads from creating the same item
            var lockObject = _lockObjects.GetOrAdd(key, _ => new object());
            
            lock (lockObject)
            {
                // Double-check pattern
                if (_memoryCache.TryGetValue(key, out cachedValue))
                {
                    return cachedValue;
                }
            }

            try
            {
                _logger.LogDebug("Cache miss for key: {Key}, creating new instance asynchronously", key);
                var result = await factory();
                
                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expiration ?? DefaultExpiration,
                    SlidingExpiration = TimeSpan.FromMinutes(10),
                    Priority = CacheItemPriority.Normal
                };

                _memoryCache.Set(key, result, cacheOptions);
                _logger.LogDebug("Cached result for key: {Key}", key);
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating cached item for key: {Key}", key);
                throw;
            }
            finally
            {
                _lockObjects.TryRemove(key, out _);
            }
        }

        public void Remove(string key)
        {
            _memoryCache.Remove(key);
            _logger.LogDebug("Removed cached item for key: {Key}", key);
        }

        public void Clear()
        {
            if (_memoryCache is MemoryCache mc)
            {
                mc.Clear();
                _logger.LogInformation("Cleared all cached items");
            }
        }
    }

    /// <summary>
    /// Extension methods for common service caching scenarios
    /// </summary>
    public static class ServiceCachingExtensions
    {
        /// <summary>
        /// Cache a service resolution for a specified duration
        /// </summary>
        public static T CacheService<T>(this IServiceCachingService cache, IServiceProvider serviceProvider, TimeSpan? expiration = null)
        {
            var key = $"Service_{typeof(T).FullName}";
            return cache.GetOrCreate(key, () => serviceProvider.GetRequiredService<T>(), expiration);
        }

        /// <summary>
        /// Cache a strategy creation for a specified duration
        /// </summary>
        public static T CacheStrategy<T>(this IServiceCachingService cache, string strategyType, Func<T> factory, TimeSpan? expiration = null)
        {
            var key = $"Strategy_{strategyType}_{typeof(T).Name}";
            return cache.GetOrCreate(key, factory, expiration ?? TimeSpan.FromHours(1));
        }

        /// <summary>
        /// Cache expensive configuration lookups
        /// </summary>
        public static T CacheConfiguration<T>(this IServiceCachingService cache, string configurationKey, Func<T> factory)
        {
            var key = $"Config_{configurationKey}";
            return cache.GetOrCreate(key, factory, TimeSpan.FromHours(2));
        }
    }
}