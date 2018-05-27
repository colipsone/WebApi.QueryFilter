using System;
using System.Threading.Tasks;

namespace IQueryableFilter.Infrastructure.Caching
{
    /// <summary>
    /// The cache manager interface.
    /// </summary>
    public interface ICacheManager
    {
        /// <summary>
        /// Clear all cache data.
        /// </summary>
        void Clear();

        /// <summary>
        /// Gets the value associated with the specified key or <paramref name="defaultValue" /> if not found.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>
        /// The value associated with the specified key.
        /// </returns>
        T Get<T>(string key, T defaultValue = default);

        /// <summary>
        /// Gets the value associated with the specified key or adds it to the cache.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="acquire">The acquire.</param>
        /// <param name="cacheTime">The cache time.</param>
        /// <param name="dependencies">The dependencies.</param>
        /// <returns>The value.</returns>
        T GetOrAdd<T>(string key, Func<T> acquire, TimeSpan? cacheTime, params string[] dependencies);

        /// <summary>
        /// Gets the task associated with the specified key or adds task to the cache.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="acquire">The acquire.</param>
        /// <param name="cacheTime">The cache time.</param>
        /// <param name="dependencies">The dependencies.</param>
        /// <returns>The task.</returns>
        Task<T> GetOrAddTask<T>(string key, Func<Task<T>> acquire, TimeSpan? cacheTime, params string[] dependencies);

        /// <summary>
        /// Gets a value indicating whether the value associated with the specified key is cached
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>
        /// <c>true</c> [if key is set]; otherwise, <c>false</c>.
        /// </returns>
        bool IsSet(string key);

        /// <summary>
        /// Removes the value with the specified key from the cache.
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>The value.</returns>
        object Remove(string key);

        /// <summary>
        /// Removes items by prefix.
        /// </summary>
        /// <param name="prefix">prefix</param>
        /// <param name="comparison">comparison type</param>
        void RemoveByPrefix(string prefix, StringComparison comparison = StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Removes items by pattern.
        /// </summary>
        /// <param name="pattern">pattern</param>
        void RemoveByRegex(string pattern);

        /// <summary>
        /// Adds the specified key and object to the cache.
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="data">Data</param>
        /// <param name="cacheTime">Cache time</param>
        /// <param name="dependencies">Dependencies</param>
        void Set(string key, object data, TimeSpan? cacheTime, params string[] dependencies);
    }
}