using System;
using System.Collections;
using System.Collections.Generic;

namespace Nohros
{
  /// <summary>
  /// Represents a range of dates and times that has strict boundaries.
  /// </summary>
  /// <remarks>
  /// The boundaries is defined as the following:
  /// <list type="bullet">
  /// <item>
  /// <term>
  /// For <see cref="DateGranularity.Second"/></term>
  /// <description>
  /// The millisecond component of the begin and end dates is set to zero.
  /// </description>
  /// </item>
  /// <item>
  /// <term>
  /// For <see cref="DateGranularity.Minute"/></term>
  /// <description>
  /// The millisecond and second components of the begin and end dates is set
  /// to zero.
  /// </description>
  /// </item>
  /// <item>
  /// <term>
  /// For <see cref="DateGranularity.Hour"/></term>
  /// <description>
  /// The millisecond, second and minute components of the begin and end dates
  /// is set to zero.
  /// </description>
  /// </item>
  /// <item>
  /// <term>
  /// For <see cref="DateGranularity.Day"/></term>
  /// <description>
  /// The millisecond, second, minute and hour components of the begin and end
  /// dates is set to zero.
  /// </description>
  /// </item>
  /// <item>
  /// <term>
  /// For <see cref="DateGranularity.Month"/></term>
  /// <description>
  /// The millisecond, second, minute and hour components of the begin and end
  /// dates is set to zero and its day component is set to one.
  /// </description>
  /// </item>
  /// <item>
  /// <term>
  /// For <see cref="DateGranularity.Year"/></term>
  /// <description>
  /// The millisecond, second, minute and hour components of the begin and end
  /// dates is set to zero and its day and month components is set to one.
  /// </description>
  /// </item>
  /// </list>
  /// </remarks>
  public class StrictDateRange : IEnumerable<DateTime>
  {
    readonly DateRange interval_;

    /// <summary>
    /// Initializes a new instance of the <see cref="StrictDateRange"/> class.
    /// </summary>
    public StrictDateRange() {
      interval_ = new DateRange();
    }

    /// <summary>
    /// Initializes the <see cref="DateRange"/> by using the given first and
    /// last dates of the range.
    /// </summary>
    /// <param name="begin">
    /// The first date of the range.
    /// </param>
    /// <param name="end">
    /// The last date if the range
    /// </param>
    /// <remarks>
    /// The dates of the range is enumerated using one unit of
    /// <see cref="DateGranularity.Day"/>.
    /// </remarks>
    public StrictDateRange(DateTime begin, DateTime end)
      : this(begin, end, 1, DateGranularity.Day) {
    }

    /// <summary>
    /// Initializes the <see cref="DateRange"/> by using the given first and
    /// last dates of the range.
    /// </summary>
    /// <param name="begin">
    /// The first date of the range.
    /// </param>
    /// <param name="end">
    /// The last date if the range
    /// </param>
    /// <param name="granularity">
    /// The granularity of the step. This represents the number of steps to
    /// advance per item enumerated while enumerating the dates of a range.
    /// </param>
    /// <remarks>
    /// The dates of the range is enumerated using
    /// <paramref name="granularity"/> units of
    /// <see cref="DateGranularity.Day"/>.
    /// </remarks>
    public StrictDateRange(DateTime begin, DateTime end, int granularity)
      : this(begin, end, granularity, DateGranularity.Day) {
    }

    /// <summary>
    /// Initializes the <see cref="DateRange"/> by using the given first,
    /// step type and number of steps per unit.
    /// </summary>
    /// <param name="begin">
    /// The first date of the range.
    /// </param>
    /// <param name="end">
    /// The last date if the range
    /// </param>
    /// <param name="granularity">
    /// The granularity of the step. This represents the number of steps to
    /// advance per item enumerated while enumerating the dates of a range.
    /// </param>
    /// <param name="unit">
    /// The type of the <paramref name="granularity"/>.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="begin"/> is greater than
    /// <paramref name="end"/> - or -
    /// <para>
    /// <paramref name="granularity"/> is negative - or -
    /// </para>
    /// </exception>
    /// <remarks>
    /// The dates of the range is enumerated using
    /// <paramref name="granularity"/> units of
    /// <paramref name="unit"/>.
    /// </remarks>
    public StrictDateRange(DateTime begin, DateTime end, int granularity,
      DateGranularity unit) {
      DateTime begin_strict = GetStrictDate(begin, unit);
      DateTime end_strict = GetStrictDate(end, unit);
      interval_ = new DateRange(begin_strict, end_strict, granularity, unit);
    }

    DateTime GetStrictDate(DateTime d, DateGranularity unit) {
      switch (unit) {
        case DateGranularity.Second:
          return new DateTime(d.Year, d.Month, d.Day, d.Hour, d.Minute,
            d.Second, d.Kind);

        case DateGranularity.Minute:
          return new DateTime(d.Year, d.Month, d.Day, d.Hour, d.Minute, 0,
            d.Kind);

        case DateGranularity.Hour:
          return new DateTime(d.Year, d.Month, d.Day, d.Hour, 0, 0, d.Kind);

        case DateGranularity.Day:
          return new DateTime(d.Year, d.Month, d.Day, 0, 0, 0, d.Kind);

        case DateGranularity.Month:
          return new DateTime(d.Year, d.Month, 1, 0, 0, 0, d.Kind);

        case DateGranularity.Year:
          return new DateTime(d.Year, 1, 1, 0, 0, 0, d.Kind);

        default:
          throw new NotReachedException();
      }
    }

    /// <summary>
    /// Gets a <see cref="IEnumerator"/> that enumerates the dates of a
    /// range.
    /// </summary>
    /// <returns>
    /// A <see cref="IEnumerator"/> that enumerates the dates of a range.
    /// </returns>
    /// <remarks>
    /// The enumeration starts at <see cref="FirstDate"/> and increments one
    /// <see cref="Granularity"/> at each enumeration.
    /// </remarks>
    public IEnumerator<DateTime> GetEnumerator() {
      return interval_.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }

    /// <summary>
    /// Gets the first date of the range.
    /// </summary>
    public DateTime FirstDate {
      get { return interval_.FirstDate; }
    }

    /// <summary>
    /// Gets the last date of the range.
    /// </summary>
    public DateTime LastDate {
      get { return interval_.LastDate; }
    }

    /// <summary>
    /// Gets the unit of the granularity.
    /// </summary>
    public DateGranularity GranularityUnit {
      get { return interval_.GranularityUnit; }
    }

    /// <summary>
    /// Gets the granularity that is used to enumerate the dates of a range.
    /// </summary>
    public int Granularity {
      get { return interval_.Granularity; }
    }
  }
}
