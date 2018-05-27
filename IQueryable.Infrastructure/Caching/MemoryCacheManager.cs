using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.Caching;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using IQueryableFilter.Infrastructure.Time;

namespace IQueryableFilter.Infrastructure.Caching
{
    /// <summary>
    /// Represents the MemoryCacheCache.
    /// </summary>
    /// <seealso cref="ICacheManager" />
    /// <seealso cref="System.IDisposable" />
    public sealed class MemoryCacheManager : ICacheManager, IDisposable
    {
        private const string CacheName = "EmergeTMS";
        private static readonly ConcurrentDictionary<string, Regex> RegexCache = new ConcurrentDictionary<string, Regex>();

        private readonly ITimeService _timeService;
        private readonly ReaderWriterLockSlim _cacheAccessLock = new ReaderWriterLockSlim();

        private MemoryCache _cache = new MemoryCache(CacheName);
        private bool _disposed;

        public MemoryCacheManager(ITimeService timeService)
        {
            _timeService = timeService ?? throw new ArgumentNullException(nameof(timeService));
        }

        /// <summary>
        /// Clear all cache data.
        /// </summary>
        public void Clear()
        {
            SafeCacheAccess(() =>
                {
                    MemoryCache oldCache = _cache;
                    _cache = new MemoryCache(CacheName);
                    oldCache.Dispose();
                },
                true);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
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
            object item = SafeCacheAccess(() => _cache.Get(key));
            return ExtractValue(item, defaultValue);
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
            var data = Get<T>(key);
            if (!EqualityComparer<T>.Default.Equals(data, default))
            {
                return data;
            }

            if (cacheTime == TimeSpan.Zero)
            {
                return acquire();
            }

            Tuple<Lazy<T>, object> items = SafeCacheAccess(() =>
            {
                CacheItemPolicy policy = GetPolicy(_cache, cacheTime, dependencies);
                EnsureRemovedCallbackDoesNotReturnTheLazy<T>(policy);

                var newLazyCacheItem = new Lazy<T>(acquire);

                return Tuple.Create(newLazyCacheItem, _cache.AddOrGetExisting(key, newLazyCacheItem, policy));
            });
            if (items.Item2 != null)
            {
                return ExtractValue<T>(items.Item2);
            }

            try
            {
                return items.Item1.Value;
            }
            catch //addItemFactory errored so do not cache the exception
            {
                Remove(key);
                throw;
            }
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
        public async Task<T> GetOrAddTask<T>(string key, Func<Task<T>> acquire, TimeSpan? cacheTime, params string[] dependencies)
        {
            try
            {
                return await GetOrAdd(key, acquire, cacheTime, dependencies).ConfigureAwait(false);
            }
            catch
            {
                Remove(key);
                throw;
            }
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
            return SafeCacheAccess(() => _cache.Contains(key));
        }

        /// <summary>
        /// Removes the value with the specified key from the cache.
        /// </summary>
        /// <param name="key">key</param>
        public object Remove(string key)
        {
            return SafeCacheAccess(() => _cache.Remove(key));
        }

        /// <summary>
        /// Removes items by prefix.
        /// </summary>
        /// <param name="prefix">prefix</param>
        /// <param name="comparison">comparison type</param>
        public void RemoveByPrefix(string prefix, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            RemoveMatchedKeys(key => key.StartsWith(prefix, comparison));
        }

        /// <summary>
        /// Removes items by pattern.
        /// </summary>
        /// <param name="pattern">pattern</param>
        public void RemoveByRegex(string pattern)
        {
            Regex regex = RegexCache.GetOrAdd(pattern,
                p => new Regex(p,
                    RegexOptions.Singleline
                    | RegexOptions.Compiled
                    | RegexOptions.IgnoreCase
                    | RegexOptions.ExplicitCapture
                    | RegexOptions.CultureInvariant));

            RemoveMatchedKeys(regex.IsMatch);
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
            if (data == null || cacheTime == TimeSpan.Zero)
            {
                return;
            }

            SafeCacheAccess(() =>
            {
                CacheItemPolicy policy = GetPolicy(_cache, cacheTime, dependencies);
                _cache.Set(key, data, policy);
            });
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }
            if (disposing)
            {
                _cache.Dispose();
                _cacheAccessLock.Dispose();
            }
            _disposed = true;
        }

        private static void EnsureRemovedCallbackDoesNotReturnTheLazy<T>(CacheItemPolicy policy)
        {
            if (policy?.RemovedCallback == null)
            {
                return;
            }

            CacheEntryRemovedCallback originallCallback = policy.RemovedCallback;
            policy.RemovedCallback = args =>
            {
                //unwrap the cache item in a callback given one is specified
                if (args?.CacheItem?.Value is Lazy<T> lazyCacheItem)
                {
                    args.CacheItem.Value = lazyCacheItem.IsValueCreated ? lazyCacheItem.Value : default;
                }
                originallCallback(args);
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static T ExtractValue<T>(object item, T defaultValue = default)
        {
            switch (item)
            {
                case null:
                    return defaultValue;
                case T value:
                    return value;
                case Lazy<T> lazy:
                    return lazy.Value;
            }

            return defaultValue;
        }

        private CacheItemPolicy GetPolicy(ObjectCache cache, TimeSpan? cacheTime, IReadOnlyCollection<string> dependencies)
        {
            Debug.Assert(cache != null);

            var policy = new CacheItemPolicy
            {
                AbsoluteExpiration = cacheTime.HasValue ? _timeService.Now.Add(cacheTime.Value) : ObjectCache.InfiniteAbsoluteExpiration
            };

            if (dependencies == null || dependencies.Count == 0)
            {
                return policy;
            }

            foreach (string dependency in dependencies)
            {
                cache.Add(dependency, string.Empty, ObjectCache.InfiniteAbsoluteExpiration);
            }

            policy.ChangeMonitors.Add(cache.CreateCacheEntryChangeMonitor(dependencies));
            return policy;
        }

        private void RemoveMatchedKeys(Func<string, bool> matchFunc)
        {
            Contract.Assert(matchFunc != null);

            SafeCacheAccess(() =>
                {
                    List<string> keysToRemove = _cache.AsParallel().Where(item => matchFunc(item.Key)).Select(item => item.Key).ToList();
                    keysToRemove.AsParallel().ForAll(key => _cache.Remove(key));
                },
                true);
        }

        private void SafeCacheAccess(Action action, bool exclusive = false)
        {
            Debug.Assert(action != null);

            if (exclusive)
            {
                _cacheAccessLock.EnterWriteLock();
            }
            else
            {
                _cacheAccessLock.EnterReadLock();
            }
            try
            {
                action.Invoke();
            }
            finally
            {
                if (exclusive)
                {
                    _cacheAccessLock.ExitWriteLock();
                }
                else
                {
                    _cacheAccessLock.ExitReadLock();
                }
            }
        }

        private T SafeCacheAccess<T>(Func<T> func, bool exclusive = false)
        {
            Debug.Assert(func != null);

            if (exclusive)
            {
                _cacheAccessLock.EnterWriteLock();
            }
            else
            {
                _cacheAccessLock.EnterReadLock();
            }
            try
            {
                return func.Invoke();
            }
            finally
            {
                if (exclusive)
                {
                    _cacheAccessLock.ExitWriteLock();
                }
                else
                {
                    _cacheAccessLock.ExitReadLock();
                }
            }
        }

        [ExcludeFromCodeCoverage]
        ~MemoryCacheManager()
        {
            Dispose(false);
        }
    }
}