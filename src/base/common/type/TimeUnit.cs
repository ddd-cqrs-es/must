using System;
using Nohros.Resources;

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

  namespace Extensions.Time
  {
    /// <summary>
    /// Extensions for time based conversion.
    /// </summary>
    public static class TimeUnitHelper
    {
      /// <summary>
      /// Handy constants for conversion methods.
      /// </summary>
      const long C0 = 1L; // 1 nanosecond

      // nanoseconds to ticks
      const long C1 = C0*100L;

      // ticks to microseconds
      const long C2 = C1*10L;

      // microseconds to miliseconds
      const long C3 = C2*1000L;

      // miliseconds to seconds
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
      static double x(double d, double m, double over) {
        if (d > over) return double.MaxValue;
        if (d < -over) return double.MinValue;
        return d*m;
      }

      /// <summary>
      /// Convert the specified time duration in the given unit to the
      /// nanoseconds units.
      /// </summary>
      /// <returns></returns>
      public static long ToTicks(this double duration, TimeUnit unit) {
        switch (unit) {
          case TimeUnit.Nanoseconds:
            return (long) duration/(C1/C0);

          case TimeUnit.Ticks:
            return (long) duration;

          case TimeUnit.Microseconds:
            return (long) x(duration, C2/C1, MAX/(C2/C1));

          case TimeUnit.Milliseconds:
            return (long) x(duration, C3/C1, MAX/(C3/C1));

          case TimeUnit.Seconds:
            return (long) x(duration, C4/C1, MAX/(C4/C1));

          case TimeUnit.Minutes:
            return (long) x(duration, C5/C1, MAX/(C5/C4));

          case TimeUnit.Hours:
            return (long) x(duration, C6/C1, MAX/(C6/C4));

          case TimeUnit.Days:
            return (long) x(duration, C7/C1, MAX/(C7/C4));
        }
        throw new ArgumentOutOfRangeException("unit");
      }

      /// <summary>
      /// Convert the specified time duration in the given unit to the
      /// nanoseconds units.
      /// </summary>
      public static long ToNanos(this double duration, TimeUnit unit) {
        switch (unit) {
          case TimeUnit.Nanoseconds:
            return (long) duration;

          case TimeUnit.Ticks:
            return (long) x(duration, C1/C0, MAX/(C1/C0));

          case TimeUnit.Microseconds:
            return (long) x(duration, C2/C0, MAX/(C2/C0));

          case TimeUnit.Milliseconds:
            return (long) x(duration, C3/C0, MAX/(C3/C0));

          case TimeUnit.Seconds:
            return (long) x(duration, C4/C0, MAX/(C4/C0));

          case TimeUnit.Minutes:
            return (long) x(duration, C5/C0, MAX/(C5/C0));

          case TimeUnit.Hours:
            return (long) x(duration, C6/C0, MAX/(C6/C0));

          case TimeUnit.Days:
            return (long) x(duration, C7/C0, MAX/(C7/C0));
        }
        throw new ArgumentOutOfRangeException("unit");
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
      public static long ToUnixEpoch(this DateTime duration) {
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
      public static DateTime FromUnixEpoch(this long timestamp) {
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
      public static DateTime FromUnixEpoch(this long timestamp,
        DateTimeKind kind) {
        var date = epoch.AddSeconds(timestamp);
        return kind == DateTimeKind.Utc ? date : date.ToLocalTime();
      }

      /// <summary>
      /// Convert the specified time duration in the given unit to the
      /// seconds units.
      /// </summary>
      /// <returns></returns>
      [Obsolete("THis method is obsolete. Use ToMilliseconds instead", true)]
      public static long ToMillis(long duration, TimeUnit unit) {
        return (long) ToMilliseconds(duration, unit);
      }

      /// <summary>
      /// Convert the specified time duration in the given unit to the
      /// seconds units.
      /// </summary>
      /// <returns></returns>
      public static double ToMilliseconds(this double duration, TimeUnit unit) {
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
      public static double ToSeconds(this double duration, TimeUnit unit) {
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

      /// <summary>
      /// Convert the specified time duration in the given unit to the
      /// nanoseconds units.
      /// </summary>
      /// <returns></returns>
      public static long ToTicks(this long duration, TimeUnit unit) {
        return ToTicks((double) duration, unit);
      }

      /// <summary>
      /// Convert the specified time duration in the given unit to the
      /// nanoseconds units.
      /// </summary>
      public static long ToNanos(this long duration, TimeUnit unit) {
        return ToNanos((double) duration, unit);
      }

      /// <summary>
      /// Convert the specified time duration in the given unit to the
      /// seconds units.
      /// </summary>
      /// <returns></returns>
      public static double ToMilliseconds(this long duration, TimeUnit unit) {
        return (long) ToMilliseconds((double) duration, unit);
      }

      /// <summary>
      /// Convert the specified time duration in the given unit to the
      /// nanoseconds units.
      /// </summary>
      /// <returns></returns>
      public static double ToSeconds(this long duration, TimeUnit unit) {
        return (long) ToSeconds((double) duration, unit);
      }

      /// <summary>
      /// Convert the specified time duration in the given unit to the
      /// nanoseconds units.
      /// </summary>
      /// <returns></returns>
      public static long ToTicks(this int duration, TimeUnit unit) {
        return ToTicks((long) duration, unit);
      }

      /// <summary>
      /// Convert the specified time duration in the given unit to the
      /// nanoseconds units.
      /// </summary>
      public static double ToNanos(this int duration, TimeUnit unit) {
        return ToNanos((double) duration, unit);
      }

      /// <summary>
      /// Convert the specified time duration in the given unit to the
      /// seconds units.
      /// </summary>
      /// <returns></returns>
      public static double ToMilliseconds(this int duration, TimeUnit unit) {
        return ToMilliseconds((double) duration, unit);
      }

      /// <summary>
      /// Convert the specified time duration in the given unit to the
      /// nanoseconds units.
      /// </summary>
      /// <returns></returns>
      public static double ToSeconds(this int duration, TimeUnit unit) {
        return ToSeconds((double) duration, unit);
      }

      /// <summary>
      /// Convert the specified time duration in the given unit to the
      /// nanoseconds units.
      /// </summary>
      /// <returns></returns>
      public static long ToTicks(this TimeUnit @from, int duration) {
        return (long) @from.Convert(duration, TimeUnit.Ticks);
      }

      /// <summary>
      /// Convert the specified time duration in the given unit to the
      /// nanoseconds units.
      /// </summary>
      public static double ToNanos(this TimeUnit @from, int duration) {
        return @from.Convert(duration, TimeUnit.Nanoseconds);
      }

      /// <summary>
      /// Convert the specified time duration in the given unit to the
      /// seconds units.
      /// </summary>
      /// <returns></returns>
      public static double ToMilliseconds(this TimeUnit @from, int duration) {
        return @from.Convert(duration, TimeUnit.Milliseconds);
      }

      /// <summary>
      /// Convert the specified time duration in the given unit to the
      /// nanoseconds units.
      /// </summary>
      /// <returns></returns>
      public static double ToSeconds(this TimeUnit @from, int duration) {
        return @from.Convert(duration, TimeUnit.Seconds);
      }

      /// <summary>
      /// Convert the specified time duration in the given unit to the
      /// nanoseconds units.
      /// </summary>
      /// <returns></returns>
      public static double ToSeconds(this TimeUnit @from, long duration) {
        return @from.Convert(duration, TimeUnit.Seconds);
      }

      /// <summary>
      /// Convert the specified time duration in the given unit to the
      /// nanoseconds units.
      /// </summary>
      /// <returns></returns>
      public static long ToTicks(this TimeUnit @from, long duration) {
        return (long) @from.Convert(duration, TimeUnit.Ticks);
      }

      /// <summary>
      /// Convert the specified time duration in the given unit to the
      /// nanoseconds units.
      /// </summary>
      public static double ToNanos(this TimeUnit @from, long duration) {
        return @from.Convert(duration, TimeUnit.Nanoseconds);
      }

      /// <summary>
      /// Convert the specified time duration in the given unit to the
      /// seconds units.
      /// </summary>
      /// <returns></returns>
      public static double ToMilliseconds(this TimeUnit @from, long duration) {
        return @from.Convert(duration, TimeUnit.Milliseconds);
      }

      /// <summary>
      /// Convert the specified time duration in the given unit to the
      /// nanoseconds units.
      /// </summary>
      /// <returns></returns>
      public static double ToSeconds(this TimeUnit @from, double duration) {
        return @from.Convert(duration, TimeUnit.Seconds);
      }

      /// <summary>
      /// Convert the specified time duration in the given unit to the
      /// nanoseconds units.
      /// </summary>
      /// <returns></returns>
      public static long ToTicks(this TimeUnit @from, double duration) {
        return (long) @from.Convert(duration, TimeUnit.Ticks);
      }

      /// <summary>
      /// Convert the specified time duration in the given unit to the
      /// nanoseconds units.
      /// </summary>
      public static double ToNanos(this TimeUnit @from, double duration) {
        return @from.Convert(duration, TimeUnit.Nanoseconds);
      }

      /// <summary>
      /// Convert the specified time duration in the given unit to the
      /// seconds units.
      /// </summary>
      /// <returns></returns>
      public static double ToMilliseconds(this TimeUnit @from, double duration) {
        return @from.Convert(duration, TimeUnit.Milliseconds);
      }

      /// <summary>
      /// Gets the name of the given <see cref="TimeUnit"/>.
      /// </summary>
      /// <param name="unit">
      /// THe <see cref="TimeUnit"/> to get the name.
      /// </param>
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

      /// <summary>
      /// Gets the abbreviation for the unit as defined by IEEE standard.
      /// </summary>
      /// <param name="unit">
      /// Th <see cref="TimeUnit"/> to get abbreviation.
      /// </param>
      /// <returns>
      /// The abbreviation of the <paramref name="unit"/> as defined by IEEE
      /// standard or the name of the unit in lowercase if the abbreviation
      /// for the unit is not defined.
      /// </returns>
      public static string Abbreviation(this TimeUnit unit) {
        switch (unit) {
          case TimeUnit.Days:
            return "days";
          case TimeUnit.Hours:
            return "h";
          case TimeUnit.Microseconds:
            return (char) 230 + "s";
          case TimeUnit.Milliseconds:
            return "ms";
          case TimeUnit.Minutes:
            return "min";
          case TimeUnit.Nanoseconds:
            return "ns";
          case TimeUnit.Seconds:
            return "s";
          case TimeUnit.Ticks:
            return "ticks";
          default:
            throw new ArgumentOutOfRangeException("unit");
        }
      }

      /// <summary>
      /// Converts a <see cref="TimeSpan"/> to the <paramref name="unit"/>.
      /// </summary>
      /// <param name="duration">
      /// The duration to be converted.
      /// </param>
      /// <param name="unit">
      /// The unit which <paramref name="duration"/> should be converted to.
      /// </param>
      [Obsolete("This method is obsolete. Use the Convert method instead.", true
        )]
      public static long ToUnit(this TimeSpan duration, TimeUnit unit) {
        return (long) Convert(duration, unit);
      }

      /// <summary>
      /// Converts a <see cref="TimeSpan"/> to the <paramref name="unit"/>.
      /// </summary>
      /// <param name="duration">
      /// The duration to be converted.
      /// </param>
      /// <param name="unit">
      /// The unit which <paramref name="duration"/> should be converted to.
      /// </param>
      public static double Convert(this TimeSpan duration, TimeUnit unit) {
        switch (unit) {
          case TimeUnit.Days:
            return duration.TotalDays;
          case TimeUnit.Hours:
            return duration.Hours;
          case TimeUnit.Microseconds:
            return duration.TotalSeconds*1000000;
          case TimeUnit.Milliseconds:
            return duration.TotalMilliseconds;
          case TimeUnit.Minutes:
            return duration.TotalMinutes;
          case TimeUnit.Nanoseconds:
            return duration.TotalSeconds*1000000000;
          case TimeUnit.Seconds:
            return duration.TotalSeconds;
          case TimeUnit.Ticks:
            return duration.Ticks;
          default:
            throw new ArgumentOutOfRangeException("unit");
        }
      }

      /// <summary>
      /// Converts the <paramref name="duration"/> from the unit
      /// <paramref name="@from"/> to the unit <paramref name="to"/>.
      /// </summary>
      /// <param name="from">
      /// The unit of <paramref name="duration"/>.
      /// </param>
      /// <param name="duration">
      /// The duration to be converted.
      /// </param>
      /// <param name="to">
      /// The unit which <to name="duration"/> should be converted to.
      /// </param>
      public static double Convert(this TimeUnit @from, double duration,
        TimeUnit to) {
        switch (to) {
          case TimeUnit.Days:
            //return duration.ToDays();
            throw new NotImplementedException();
          case TimeUnit.Hours:
            //return duration.ToHours();
            throw new NotImplementedException();
          case TimeUnit.Microseconds:
            //return duration.ToMicroseconds();
            throw new NotImplementedException();
          case TimeUnit.Milliseconds:
            return duration.ToMilliseconds(@from);
          case TimeUnit.Minutes:
            //return duration.ToMinutes();
            throw new NotImplementedException();
          case TimeUnit.Nanoseconds:
            return duration.ToNanos(@from);
          case TimeUnit.Seconds:
            return duration.ToSeconds(@from);
          case TimeUnit.Ticks:
            return duration.ToTicks(@from);
          default:
            throw new ArgumentOutOfRangeException("to");
        }
      }
    }
  }
}
