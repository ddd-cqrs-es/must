using System;

namespace Nohros
{
  /// <summary>
  /// A <see cref="TimeUnit"/> represents the duration at a given unit of
  /// granularity. <see cref="TimeUnit"/> helps organize and use time
  /// representations that may be maintened separately across various context.
  /// </summary>
  public enum TimeUnit
  {
    /// <summary>
    /// A nanosecond is defined as one thousandth of a microsecond.
    /// </summary>
    Nanoseconds = 0,

    /// <summary>
    /// A microsecond is defined as thousandth of a millisecond.
    /// </summary>
    Microseconds = 1,

    /// <summary>
    /// A millisecond is defined as thousandth of a second.
    /// </summary>
    Milliseconds = 2,

    /// <summary>
    /// The base unit of mensure.
    /// </summary>
    Seconds = 3,

    /// <summary>
    /// A minute is defined as sixty seconds.
    /// </summary>
    Minutes = 4,

    /// <summary>
    /// A hour is defined as sixty minutes.
    /// </summary>
    Hours = 5,

    /// <summary>
    /// A day is defined as twenty four hours.
    /// </summary>
    Days = 6,

    /// <summary>
    /// A tick is defines as 100 nanoseconds.
    /// </summary>
    Ticks = 7
  }

  public static class TimeUnitHelper
  {
    /// <summary>
    /// Handy constants for conversion methods.
    /// </summary>
    const long C0 = 1L; // 1 nanosecond

    const long C1 = 100L; // 1 nanosecond

    // nanos to micros
    const long C2 = C0*1000L;

    // micros to milis
    const long C3 = C2*1000L;

    // milis to seconds
    const long C4 = C3*1000L;

    // seconds to minutes
    const long C5 = C4*60L;

    // minutes to hours
    const long C6 = C5*60L;

    // hours to days
    const long C7 = C6*24L;

    // the maximum allowed value
    const long MAX = long.MaxValue;

    static DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    /// <summary>
    /// Scale <paramref name="d"/> by <paramref name="m"/>, checking for
    /// overflow. This has a short name to make above code more readable.
    /// </summary>
    static long x(long d, long m, long over) {
      if (d > over) return long.MaxValue;
      if (d < -over) return long.MinValue;
      return d*m;
    }

    /// <summary>
    /// Convert the specified time duration in the given unit to the
    /// nanoseconds units.
    /// </summary>
    /// <returns></returns>
    public static long ToTicks(long duration, TimeUnit unit) {
      switch (unit) {
        case TimeUnit.Nanoseconds:
          return duration/(C1/C0);

        case TimeUnit.Ticks:
          return duration;

        case TimeUnit.Microseconds:
          return x(duration, C2/C1, MAX/(C2/C1));

        case TimeUnit.Milliseconds:
          return x(duration, C3/C1, MAX/(C3/C1));

        case TimeUnit.Seconds:
          return x(duration, C4/C1, MAX/(C4/C1));

        case TimeUnit.Minutes:
          return x(duration, C5/C1, MAX/(C5/C4));

        case TimeUnit.Hours:
          return x(duration, C6/C1, MAX/(C6/C4));

        case TimeUnit.Days:
          return x(duration, C7/C1, MAX/(C7/C4));
      }
      throw new ArgumentOutOfRangeException("unit");
    }

    /// <summary>
    /// Convert the specified time duration in the given unit to the
    /// nanoseconds units.
    /// </summary>
    /// <returns></returns>
    public static long ToNanos(long duration, TimeUnit unit) {
      switch (unit) {
        case TimeUnit.Nanoseconds:
          return duration;

        case TimeUnit.Ticks:
          return x(duration, C1/C0, MAX/(C1/C0));

        case TimeUnit.Microseconds:
          return x(duration, C2/C0, MAX/(C2/C0));

        case TimeUnit.Milliseconds:
          return x(duration, C3/C0, MAX/(C3/C0));

        case TimeUnit.Seconds:
          return x(duration, C4/C0, MAX/(C4/C0));

        case TimeUnit.Minutes:
          return x(duration, C5/C0, MAX/(C5/C0));

        case TimeUnit.Hours:
          return x(duration, C6/C0, MAX/(C6/C0));

        case TimeUnit.Days:
          return x(duration, C7/C0, MAX/(C7/C0));
      }
      throw new ArgumentOutOfRangeException("unit");
    }

    /// <summary>
    /// Convert the specified timestamp to the nano seconds unit.
    /// </summary>
    /// <returns>The total number of nanoseconds that the</returns>
    public static long ToNanos(TimeSpan duration) {
      // one tick have a undred nanoseconds.
      return duration.Ticks*100;
    }

    /// <summary>
    /// Converts the specified datetime to the unix time unit.
    /// </summary>
    /// <returns>
    /// The number of seconds since unix epoch.
    /// </returns>
    /// <remarks>
    /// If <see cref="DateTime.Kind"/> property of the given
    /// <see cref="duration"/> is <see cref="DateTimeKind.Unspecified"/>
    /// <paramref name="duration"/> is assumed to be a UTC time.
    /// </remarks>
    [Obsolete("Use ToUnixEpoch instead", true)]
    public static long ToUnixTime(DateTime duration) {
      return (long) duration.ToUniversalTime().Subtract(epoch).TotalSeconds;
    }

    /// <summary>
    /// Converts the specified datetime to the unix time unit.
    /// </summary>
    /// <returns>
    /// The number of seconds since unix epoch.
    /// </returns>
    /// <remarks>
    /// If <see cref="DateTime.Kind"/> property of the given
    /// <see cref="duration"/> is <see cref="DateTimeKind.Unspecified"/>
    /// <paramref name="duration"/> is assumed to be a UTC time.
    /// </remarks>
    public static long ToUnixEpoch(DateTime duration) {
      return (long) duration.ToUniversalTime().Subtract(epoch).TotalSeconds;
    }

    /// <summary>
    /// Converts the specified epoch time to its corresponding local date
    /// and time.
    /// </summary>
    /// <param name="timestamp">
    /// A Unix epoch time.
    /// </param>
    /// <returns>
    /// The local time representation of the specified unix epoch
    /// time.
    /// </returns>
    /// <remarks>
    /// The returned date time represents a local time. To convert to a
    /// Coordinated Universal Time (UTC) use the
    /// <see cref="FromUnixEpoch(long, DateTimeKind)"/> overload
    /// </remarks>
    public static DateTime FromUnixEpoch(long timestamp) {
      return FromUnixEpoch(timestamp, DateTimeKind.Local);
    }

    /// <summary>
    /// Converts the specified epoch time to its corresponding date and time
    /// using the given date time kind.
    /// </summary>
    /// <param name="timestamp">
    /// A Unix epoch time.
    /// </param>
    /// <returns>
    /// The <see cref="DateTime"/> representation of the specified unix epoch
    /// time.
    /// </returns>
    /// <remarks>
    /// If <paramref name="kind"/> is set to
    /// <see cref="DateTimeKind.Unspecified"/> the
    /// <see cref="DateTimeKind.Local"/> will be used.
    /// </remarks>
    public static DateTime FromUnixEpoch(long timestamp, DateTimeKind kind) {
      var date = epoch.AddSeconds(timestamp);
      return kind == DateTimeKind.Utc ? date : date.ToLocalTime();
    }

    /// <summary>
    /// Convert the specified time duration in the given unit to the
    /// seconds units.
    /// </summary>
    /// <returns></returns>
    [Obsolete("Use ToMilliseconds instead", true)]
    public static long ToMillis(long duration, TimeUnit unit) {
      return ToMilliseconds(duration, unit);
    }

    /// <summary>
    /// Convert the specified time duration in the given unit to the
    /// seconds units.
    /// </summary>
    /// <returns></returns>
    public static long ToMilliseconds(long duration, TimeUnit unit) {
      switch (unit) {
        case TimeUnit.Nanoseconds:
          return duration/(C3/C0);

        case TimeUnit.Ticks:
          return duration/(C3/C1);

        case TimeUnit.Microseconds:
          return duration/(C3/C2);

        case TimeUnit.Milliseconds:
          return duration;

        case TimeUnit.Seconds:
          return x(duration, C4/C3, MAX/(C4/C3));

        case TimeUnit.Minutes:
          return x(duration, C5/C3, MAX/(C5/C3));

        case TimeUnit.Hours:
          return x(duration, C6/C3, MAX/(C6/C3));

        case TimeUnit.Days:
          return x(duration, (C7/C3), MAX/(C7/C3));
      }
      throw new ArgumentOutOfRangeException("unit");
    }

    /// <summary>
    /// Convert the specified time duration in the given unit to the
    /// nanoseconds units.
    /// </summary>
    /// <returns></returns>
    public static long ToSeconds(long duration, TimeUnit unit) {
      switch (unit) {
        case TimeUnit.Nanoseconds:
          return duration/(C4/C0);

        case TimeUnit.Ticks:
          return duration/(C4/C1);

        case TimeUnit.Microseconds:
          return duration/(C4/C2);

        case TimeUnit.Milliseconds:
          return duration/(C4/C3);

        case TimeUnit.Seconds:
          return duration;

        case TimeUnit.Minutes:
          return x(duration, C5/C4, MAX/(C5/C4));

        case TimeUnit.Hours:
          return x(duration, C6/C4, MAX/(C6/C4));

        case TimeUnit.Days:
          return x(duration, C7/C4, MAX/(C7/C4));
      }
      throw new ArgumentOutOfRangeException("unit");
    }

    public static string Name(this TimeUnit unit) {
      switch (unit) {
        case TimeUnit.Days:
          return "Days";
        case TimeUnit.Hours:
          return "Hours";
        case TimeUnit.Microseconds:
          return "Microseconds";
        case TimeUnit.Milliseconds:
          return "Milliseconds";
        case TimeUnit.Minutes:
          return "Minutes";
        case TimeUnit.Nanoseconds:
          return "Nanoseconds";
        case TimeUnit.Seconds:
          return "Seconds";
        case TimeUnit.Ticks:
          return "Ticks";
        default:
          throw new ArgumentOutOfRangeException("unit");
      }
    }
  }
}
