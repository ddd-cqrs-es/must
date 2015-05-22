using System;
using NUnit.Framework;

namespace Nohros
{
  [TestFixture]
  public class StrictDateRangeTests
  {
    public void enumerate_all_days_in_range(int granularity) {
      var first_date = new DateTime(2014, 03, 10, 10, 5, 9, 630);
      var last_date = new DateTime(2014, 04, 10, 10, 5, 9, 630);
      var range = new StrictDateRange(first_date, last_date, granularity);

      DateTime current = new DateTime(2014, 3, 10);
      foreach (DateTime date in range) {
        Assert.That(date, Is.EqualTo(current));
        current = current.AddDays(granularity);
      }
    }

    [Test]
    public void should_enumerate_all_days_in_range() {
      enumerate_all_days_in_range(1);
    }

    [Test]
    public void should_honor_the_boundaries() {
      var first_date = new DateTime(2014, 01, 01, 10, 5, 9, 630);
      var last_date = new DateTime(2014, 12, 01, 10, 5, 9, 630);

      var range = new StrictDateRange(first_date, last_date, 1,
        DateGranularity.Month);

      DateTime current = new DateTime(2014, 1, 1);
      Assert.That(range.FirstDate, Is.EqualTo(current));

      foreach (DateTime date in range) {
        current = date;
      }
      Assert.That(current, Is.LessThanOrEqualTo(last_date));
      Assert.That(range.LastDate, Is.EqualTo(current));
    }

    [Test]
    public void should_enumerate_all_months_in_range() {
      var first_date = new DateTime(2014, 01, 01, 10, 5, 9, 630);
      var last_date = new DateTime(2014, 12, 01, 10, 5, 9, 630);

      var range = new StrictDateRange(first_date, last_date, 1,
        DateGranularity.Month);

      DateTime current = new DateTime(2014, 1, 1);
      foreach (DateTime date in range) {
        Assert.That(date, Is.EqualTo(current));
        current = current.AddMonths(1);
      }
    }

    [Test]
    public void should_enumerate_all_seconds_in_range() {
      var first_date = new DateTime(2014, 01, 01, 10, 5, 9, 630);
      var last_date = new DateTime(2014, 01, 01, 10, 6, 9, 630);

      var range = new StrictDateRange(first_date, last_date, 10,
        DateGranularity.Second);

      DateTime current = new DateTime(2014, 1, 1, 10, 5, 9);
      foreach (DateTime date in range) {
        Assert.That(date, Is.EqualTo(current));
        current = current.AddSeconds(10);
      }
    }

    [Test]
    public void should_enumerate_all_quarters_in_range() {
      enumerate_all_days_in_range(15);
    }

    [Test]
    public void should_enumerate_all_weeks_in_range() {
      enumerate_all_days_in_range(7);
    }

    [Test]
    public void should_not_accept_negative_granularity() {
      Assert.Throws<ArgumentOutOfRangeException>(
        () => new StrictDateRange(DateTime.Now, DateTime.Now, -10));

      Assert.Throws<ArgumentOutOfRangeException>(
        () =>
          new StrictDateRange(DateTime.Now, DateTime.Now, -10,
            DateGranularity.Day));

      Assert.Throws<ArgumentOutOfRangeException>(
        () =>
          new StrictDateRange(DateTime.Now, DateTime.Now, -10,
            DateGranularity.Month));

      Assert.Throws<ArgumentOutOfRangeException>(
        () =>
          new StrictDateRange(DateTime.Now, DateTime.Now, -10,
            DateGranularity.Year));
    }

    [Test]
    public void should_stop_enumeration_on_last_date() {
      var first_date = new DateTime(2014, 03, 10, 10, 5, 9, 630);
      var last_date = new DateTime(2014, 04, 10, 10, 5, 9, 630);
      var range = new StrictDateRange(first_date, last_date);

      DateTime current = first_date;
      foreach (DateTime date in range) {
        current = date;
      }

      Assert.That(current, Is.EqualTo(new DateTime(2014, 4, 10)));
    }

    [Test]
    public void should_use_day_as_default_granularity_unit() {
      var range = new StrictDateRange();
      Assert.That(range.GranularityUnit, Is.EqualTo(DateGranularity.Day));

      range = new StrictDateRange(DateTime.Now, DateTime.Now);
      Assert.That(range.GranularityUnit, Is.EqualTo(DateGranularity.Day));
    }

    [Test]
    public void should_use_one_as_default_granularity() {
      var range = new StrictDateRange();
      Assert.That(range.Granularity, Is.EqualTo(1));

      range = new StrictDateRange(DateTime.Now, DateTime.Now);
      Assert.That(range.Granularity, Is.EqualTo(1));
    }
  }
}
