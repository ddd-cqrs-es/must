using System;
using System.Threading;
using Nohros.Concurrent;
using Nohros.Extensions.Time;

namespace Nohros
{
  public sealed class ExpiringMemoizingSupplier<T> : ISupplier<T>
  {
    readonly ISupplier<T> supplier_;
    readonly long duration_nanos_;
    readonly object mutex_;

    T value_;

    // The special value 0 means "not yet initialized".
    AtomicLong expiration_nanos_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="ExpiringMemoizingSupplier{T}"/> using the specified
    /// supplier, duration and duration unit.
    /// </summary>
    /// <param name="supplier">A <see cref="ISupplier{T}.Supply"/> that is
    /// used to create the first instance of the type <typeparamref name="T"/>
    /// </param>
    /// <param name="duration">The length of time after a value is created
    /// that it should be stop beign returned by subsequent
    /// <see cref="ISupplier{T}.Supply"/> calls.</param>
    /// <param name="unit">The unit that duration is expressed in.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="duration"/> is not positive.</exception>
    public ExpiringMemoizingSupplier(ISupplier<T> supplier, long duration,
      TimeUnit unit) {
      if (duration < 0) {
        throw new ArgumentOutOfRangeException("duration");
      }

      supplier_ = supplier;
      duration_nanos_ = duration.ToNanos(unit);
      expiration_nanos_ = new AtomicLong(0);
      mutex_ = new object();
    }
    #endregion

    /// <summary>
    /// Gets a <typeparamref name="T"/> object and cache it until the specified
    /// time has not passed.
    /// </summary>
    /// <remarks>
    /// Subsequent calls to <see cref="ISupplier{T}.Supply()"/> return the
    /// cached value if the expiration time has not passed. After the
    /// expiration time, a new value is retrieved, cached, and returned.
    /// <para>
    /// The returned supplier is thread-safe.
    /// </para>
    /// </remarks>
    public T Supply() {
      // A variant of double checked locking.
      //
      // We use two atomic reads. We could reduce this to one by
      // putting our fields into a holder class, but (at least on x86)
      // the extra memory consumption and indirection are more expensive
      // than extra atomic read.
      long nanos = (long)expiration_nanos_;
      long now = Clock.NanoTime;
      if(nanos == 0 || now - nanos >=0) {
        lock(mutex_) {
          if(nanos == (long)expiration_nanos_) { // recheck for lost race
            T t = supplier_.Supply();

            // Disable the reordering between the instantiation of "t"
            // and assignment of "t" to "value"
            Thread.MemoryBarrier();

            value_ = t;

            nanos = now + duration_nanos_;

            // In the very unlikely event that nanos is 0, set it to 1;
            // no one will notice 1 ns of tardiness.
            expiration_nanos_.Exchange((nanos == 0) ? 1 : nanos);
            return t;
          }
        }
      }
      return value_;
    }
  }
}
