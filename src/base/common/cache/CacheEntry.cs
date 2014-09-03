using Nohros.Concurrent;
using Nohros.Extensions.Time;

namespace Nohros.Caching
{
  public partial class LoadingCache<T>
  {
    #region CacheEntry
    /// <summary>
    /// An entry in the cache, consisting of a key, value and attributes.
    /// </summary>
    /// <remarks>This class is used as a wrapper arround the cached value by
    /// the <see cref="AbstractCache{T}"/> class. It is used to decorate
    /// the cached value with some attributes, such as write access time
    /// and read access time.
    /// <para> Entries in the cache can be in the following states:
    /// <para>Valid</para>
    /// <list type="bullet">
    /// <term>Live: valid key/value are set.</term>
    /// <term>Loading: loading is pending.</term>
    /// </list>
    /// <para>
    /// Invalid
    /// <list type="bulltet">
    /// <term>Expired: time expired (key/value may still be set).</term>
    /// </list>
    /// </para>
    /// </para>
    /// </remarks>
    internal class CacheEntry<T>
    {
      readonly string key_;
      volatile IValueReference<T> value_reference_;

      AtomicLong access_time_;
      AtomicLong write_time_;

      #region .ctor
      /// <summary>
      /// Initializes a new instance of the <see cref="CacheEntry{T}"/> class
      /// by using the specified key and value.
      /// </summary>
      public CacheEntry(string key) {
        key_ = key;

        long now = (long) 0.ToNanos(TimeUnit.Milliseconds);
        access_time_ = new AtomicLong(now);
        write_time_ = new AtomicLong(now);
        value_reference_ = (IValueReference<T>) UnsetValueReference<T>.UNSET;
      }
      #endregion

      /// <summary>
      /// Gets the key for this entry.
      /// </summary>
      /// <value>The key for this entry.</value>
      public string Key {
        get { return key_; }
      }

      /// <summary>
      /// Gets or sets the value reference from this entry.
      /// </summary>
      public IValueReference<T> ValueReference {
        get { return value_reference_; }
        set { value_reference_ = value; }
      }

      /// <summary>
      /// Gets or sets the time that this entry was last written, in ns.
      /// </summary>
      /// <remarks>
      /// Any operation performed on this property(get or set) is atomic.
      /// </remarks>
      public long WriteTime {
        get { return (long) write_time_; }
        set { write_time_.Exchange(value); }
      }

      /// <summary>
      /// Gets or sets the time that this entry was last accessed, in ns.
      /// </summary>
      /// <remarks>
      /// Any operation performed on this property(get or set) is atomic.
      /// </remarks>
      public long AccessTime {
        get { return (long) access_time_; }
        set { access_time_.Exchange(value); }
      }
    }
    #endregion
  }
}