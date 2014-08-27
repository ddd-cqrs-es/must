using System;
using System.Runtime.Caching;
using Nohros.Caching.Providers;
using Nohros.Extensions.Time;

namespace Nohros.Caching
{
  public class MemoryCacheProvider : ICacheProvider
  {
    readonly MemoryCache memory_cache_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryCacheProvider"/>
    /// using the specified <see cref="MemoryCache"/> object.
    /// </summary>
    /// <param name="memory_cache">
    /// A <see cref="MemoryCache"/> that is used to cache objects.
    /// </param>
    public MemoryCacheProvider(MemoryCache memory_cache) {
      if (memory_cache == null) {
        throw new ArgumentNullException("memory_cache");
      }
      memory_cache_ = memory_cache;
    }
    #endregion

    public T Get<T>(string key) {
      object entry = memory_cache_.Get(key);
      if (entry == null) {
        return default(T);
      }
      return (T) entry;
    }

    public bool Get<T>(string key, out T item) {
      object entry = memory_cache_.Get(key);
      if (entry == null) {
        item = default(T);
        return false;
      }
      item = (T) entry;
      return true;
    }

    public void Set(string key, object value) {
      var policy = new CacheItemPolicy();
      memory_cache_.Set(key, value, policy);
    }

    public void Set(string key, object value, long duration, TimeUnit unit) {
      var policy = new CacheItemPolicy();
      policy.AbsoluteExpiration =
        DateTimeOffset.Now.AddMilliseconds(duration.ToMilliseconds(unit));
      memory_cache_.Set(key, value, policy);
    }

    public void Add(string key, object value) {
      var policy = new CacheItemPolicy();
      memory_cache_.Add(key, value, policy);
    }

    public void Add(string key, object value, long duration, TimeUnit unit) {
      var policy = new CacheItemPolicy();
      policy.AbsoluteExpiration =
        DateTimeOffset.Now.AddMilliseconds(duration.ToMilliseconds(unit));
      memory_cache_.Add(key, value, policy);
    }

    public void Remove(string key) {
      memory_cache_.Remove(key);
    }

    public void Clear() {
      // do nothing, since memory cache does not have a Clear method.
    }
  }
}
