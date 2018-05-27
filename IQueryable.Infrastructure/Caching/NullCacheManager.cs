using System;
using System.Threading.Tasks;

namespace IQueryableFilter.Infrastructure.Caching
{
    /// <summary>
    ///     Represents a NopNullCache
    /// </summary>
    public sealed class NullCacheManager : ICacheManager
    {
        /// <summary>
        /// Clear all cache data.
        /// </summary>
        public void Clear()
        {
        }


        /// <summary>
        /// Gets the value associated with the specified key or <paramref name="defaultValue" /> if not found.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>
        /// The value associated with the specified key.
        /// </returns>
        public T Get<T>(string key, T defaultValue = default)
        {
            return defaultValue;
        }

        /// <summary>
        /// Gets the value associated with the specified key or adds it to the cache.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="acquire">The acquire.</param>
        /// <param name="cacheTime">The cache time.</param>
        /// <param name="dependencies">The dependencies.</param>
        /// <returns>
        /// The value.
        /// </returns>
        public T GetOrAdd<T>(string key, Func<T> acquire, TimeSpan? cacheTime, params string[] dependencies)
        {
            return acquire();
        }

        /// <summary>
        /// Gets the task associated with the specified key or adds task to the cache.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="acquire">The acquire.</param>
        /// <param name="cacheTime">The cache time.</param>
        /// <param name="dependencies">The dependencies.</param>
        /// <returns>
        /// The task.
        /// </returns>
        public Task<T> GetOrAddTask<T>(string key, Func<Task<T>> acquire, TimeSpan? cacheTime, params string[] dependencies)
        {
            return acquire();
        }

        /// <summary>
        /// Gets a value indicating whether the value associated with the specified key is cached
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>
        ///   <c>true</c> [if key is set]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsSet(string key)
        {
            return false;
        }

        /// <summary>
        /// Removes the value with the specified key from the cache.
        /// </summary>
        /// <param name="key">key</param>
        public object Remove(string key)
        {
            return null;
        }

        /// <summary>
        /// Removes items by prefix.
        /// </summary>
        /// <param name="prefix">prefix</param>
        /// <param name="comparison">comparison type</param>
        public void RemoveByPrefix(string prefix, StringComparison comparison)
        {
        }

        /// <summary>
        /// Removes items by pattern.
        /// </summary>
        /// <param name="pattern">pattern</param>
        public void RemoveByRegex(string pattern)
        {
        }

        /// <summary>
        /// Adds the specified key and object to the cache.
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="data">Data</param>
        /// <param name="cacheTime">Cache time</param>
        /// <param name="dependencies">Dependencies</param>
        public void Set(string key, object data, TimeSpan? cacheTime, params string[] dependencies)
        {
        }
    }
}