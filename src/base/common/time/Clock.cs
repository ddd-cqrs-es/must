using System;

namespace Nohros
{
  /// <summary>
  /// An abstraction for how the time passes.
  /// </summary>
  public abstract class Clock
  {
    /// <summary>
    /// Gets the current value of the most precise available system timer, in
    /// nanoseconds.
    /// </summary>
    /// <remarks>
    /// This method should be used only to measure elapsed time
    /// and is not related to any other notion of system or wall-clock time.
    /// The value returned represents nanoseconds since some fixed but
    /// arbitrary time(perhaps in the future, so values may be negative). This
    /// method provides nanoseconds precision, but not necessarily nanoseconds
    /// accurancy.No guarantees are made about how frequently values changes,
    /// and while its return value is nanoseconds, the update interval is
    /// typically only microseconds(10ms or 15ms on windows).
    /// </remarks>
    public static long NanoTime {
      // A single tick represents one hundred nanoseconds.
      get { return DateTime.UtcNow.Ticks*100; }
    }

    /// <summary>
    /// Gets the current time in milliseconds.
    /// </summary>
    /// <value>The diferrence</value>
    /// <remarks>This method should be used only to measure elapsed time
    /// and is not related to any other notion of system or wall-clock time.
    /// The value returned represents miliseconds since some fixed but
    /// arbitrary time(perhaps in the future, so values may be negative). This
    /// method provides miliseconds precision, but not necessarily miliseconds
    /// accurancy.No guarantees are made about how frequently values changes,
    /// and while its return value is miliseconds, the update interval is
    /// typically only microseconds(10ms or 15ms on windows).
    /// </remarks>
    public static long CurrentTimeMilis {
      get { return (long) (NanoTime*0.0001); }
    }

    /// <summary>
    /// Gets the current time tick.
    /// </summary>
    /// <value>
    /// The current time tick.
    /// </value>
    /// <remarks>
    /// The smallest unit of time is the tick, which is equals to 100
    /// nanoseconds.
    /// <para>
    /// This method should be used only to measure elapsed time
    /// and is not related to any other notion of system or wall-clock time.
    /// The value returned represents nanoseconds since some fixed but
    /// arbitrary time(perhaps in the future, so values may be negative). This
    /// method provides nanoseconds precision, but not necessarily nanoseconds
    /// accurancy.No guarantees are made about how frequently values changes,
    /// and while its return value is nanoseconds, the update interval is
    /// typically only microseconds(10ms or 15ms on windows).
    /// </para>
    /// </remarks>
    public abstract long Tick { get; }
  }
}
