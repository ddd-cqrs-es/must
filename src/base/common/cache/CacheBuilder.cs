using System;

using Nohros.Caching.Providers;
using Nohros.Extensions.Time;

namespace Nohros.Caching
{
  /// <summary>
  /// A builder of <see cref="ICache{T}"/> and <see cref="ILoadingCache{T}"/>.
  /// </summary>
  /// <typeparam name="T">
  /// The type of the objects that is stored in cache.
  /// </typeparam>
  public sealed class CacheBuilder<T>
  {
    internal const int kUnsetInt = -1;

    long expiry_after_access_nanos_;
    long expiry_after_write_nanos_;
    long refresh_nanos_;
    StrengthType value_strench_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="CacheBuilder{T}"/> using.
    /// </summary>
    public CacheBuilder() {
      expiry_after_access_nanos_ = kUnsetInt;
      expiry_after_write_nanos_ = kUnsetInt;
      refresh_nanos_ = kUnsetInt;
      value_strench_ = StrengthType.Strong;
    }
    #endregion

    /// <summary>
    /// Builds a cache, which either returns an already-loaded value for a
    /// given key or atomically computes or retrieves it using the supplied
    /// <see cref="CacheLoader{T}"/>.
    /// </summary>
    /// <param name="loader">
    /// The cache loader used to obtain new values.
    /// </param>
    /// <param name="provider">
    /// An object that implements the plugabble <see cref="ICacheProvider"/>
    /// interface and is used to cache the objects.
    /// </param>
    /// <returns>
    /// A <see cref="ILoadingCache{T}"/> object having the requested features.
    /// </returns>
    /// <remarks>
    /// If another thread is currently loadind the value for the given key,
    /// the returned cache, simply waits for that thread to finish and returns
    /// its loaded value. Note that multiple threads can concurrently load
    /// values for distinct keys.
    /// <para>
    /// This method does not alter the state of the
    /// <see cref="CacheBuilder{T}"/> instance, so it can be invoked again to
    /// create multiple independent caches.
    /// </para>
    /// </remarks>
    public ILoadingCache<T> Build(ICacheProvider provider, CacheLoader<T> loader) {
      return new LoadingCache<T>(provider, this, loader);
    }

    /// <summary>
    /// Builds a cache which does not automatically load values when keys are
    /// requested.
    /// </summary>
    /// <param name="provider">
    /// An object that implements the plugabble <see cref="ICacheProvider"/>
    /// interface and is used to cache the objects.
    /// </param>
    /// <returns>
    /// A <see cref="ICache{T}"/> object having the requested features.
    /// </returns>
    /// <remarks>
    /// Consider <see cref="Build(ICacheProvider, CacheLoader{T})"/> instead,
    /// if it is feasible to implement a <see cref="CacheLoader{T}"/>.
    /// <para>
    /// This method does not alter the state of the
    /// <see cref="CacheBuilder{T}"/> instance, so it can be invoked again to
    /// create multiple independent caches.
    /// </para>
    /// </remarks>
    public ICache<T> Build(ICacheProvider provider) {
      if (refresh_nanos_ != kUnsetInt) {
        throw new InvalidOperationException(
          "RefreshAfterWrite requires a ILoadindCache");
      }
      return new LocalManualCache<T>(provider, this);
    }

    /// <summary>
    /// Specifies that each entry should be automatically removed from the
    /// cache once a fixed duration has elapsed after entry's creation, the
    /// most recent replacement of its value, or its last access.
    /// </summary>
    /// <param name="duration">
    /// The length of time after an entry is last
    /// accessed that it should be automatically removed.</param>
    /// <returns>
    /// This cache builder instance with the expire after nanos
    /// set to the specified values.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// You set the <paramref name="duration"/> to a value that is less than
    /// zero.
    /// </exception>
    /// <remarks>Expired entries may be counted in <see cref="ICache{T}.Size"/>,
    /// but should never be visible to read or write operations.
    /// </remarks>
    public CacheBuilder<T> ExpireAfterAccess(TimeSpan duration) {
      if (duration < TimeSpan.Zero) {
        Thrower.ThrowArgumentOutOfRangeException(ExceptionArgument.duration);
      }
      expiry_after_access_nanos_ = duration.ToUnit(TimeUnit.Nanoseconds);
      return this;
    }

    /// <summary>
    /// Specifies that each entry should be automatically removed from the
    /// cache once a fixed duration has elapsed after the entry's creation,
    /// or the most recent replacement of its value.
    /// </summary>
    /// <param name="duration">
    /// The length of time after an entry is created
    /// that it should be automatically. removed.
    /// </param>
    /// <returns>This cache builder instance with the expire after nanos
    /// set to the specified values.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// You set the
    /// <paramref name="duration"/> to a value that is less than zero.
    /// </exception>
    /// <remarks>
    /// Expired entries may be counted in <see cref="ICache.Size"/>,
    /// but should never be visible to read or write operations.
    /// </remarks>
    public CacheBuilder<T> ExpireAfterWrite(TimeSpan duration) {
      if (duration < TimeSpan.Zero) {
        Thrower.ThrowArgumentOutOfRangeException(ExceptionArgument.duration);
      }
      expiry_after_write_nanos_ = duration.ToUnit(TimeUnit.Nanoseconds);
      return this;
    }

    /// <summary>
    /// Specifies that active entries are eligible for automatic refresh once
    /// a fixed duration has elapsed after the entry's creation, or the most
    /// recent replacement of its value.
    /// </summary>
    /// <param name="duration"></param>
    /// <returns></returns>
    /// <remarks>
    /// <para>
    /// The semantics of refreshes are specified in
    /// <see cref="ILoadingCache{T}.Refresh()"/>, and are performed by calling
    /// <see cref="CacheLoader{T}.Load"/>.
    /// <para>
    /// As the default implementation
    /// TODO: continue.
    /// </para>
    /// </para>
    /// </remarks>
    public CacheBuilder<T> RefreshAfterWrite(TimeSpan duration) {
      if(duration < TimeSpan.Zero) {
        Thrower.ThrowArgumentOutOfRangeException(ExceptionArgument.duration);
      }
      refresh_nanos_ = duration.ToUnit(TimeUnit.Nanoseconds);
      return this;
    }

    /// <summary>
    /// Gets a 64-bit integer that indicates how long after the last access to
    /// an entry in the cache will retain that entry.
    /// </summary>
    public long ExpiryAfterWriteNanos {
      get { return expiry_after_write_nanos_; }
    }

    /// <summary>
    /// Gets a 64bit integer that indicates how long after the last write to
    /// an entry in the cache will retain that entry.
    /// </summary>
    public long ExpiryAfterAccessNanos {
      get { return expiry_after_access_nanos_; }
    }

    /// <summary>
    /// Specifies that each value (not keys) stored in the cache should be
    /// strongly referenced.
    /// </summary>
    public CacheBuilder<T> StrongValues() {
      return SetValueStrench(StrengthType.Strong);
    }

    CacheBuilder<T> SetValueStrench(StrengthType strength_type) {
      value_strench_ = strength_type;
      return this;
    }

    internal StrengthType ValueStrength {
      get { return value_strench_; }
    }

    /// <summary>
    /// Gets a 64-bit integer that indicates how long after the last write an
    /// entry becomes a candidate for refresh.
    /// </summary>
    public long RefreshNanos {
      get { return refresh_nanos_; }
    }
  }
}