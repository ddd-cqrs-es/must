using System;
using NUnit.Framework;
using Nohros.Extensions.Time;

namespace Nohros.Common
{
  public class TimeUnitHelperTests
  {
    [Test]
    public void ShouldConvertToUnixEpoch() {
      var date = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
      var epoch = TimeUnitHelper.ToUnixEpoch(date);
      Assert.AreEqual(0, epoch);

      date = new DateTime(1970, 1, 1, 23, 0, 0, 0, DateTimeKind.Utc);
      epoch = TimeUnitHelper.ToUnixEpoch(date);
      Assert.AreEqual(23*60*60, epoch);
    }

    [Test]
    public void ShouldConvertToMilliseconds() {
      var millis = TimeUnitHelper.ToMilliseconds(10, TimeUnit.Seconds);
      Assert.That(millis, Is.EqualTo(10*1000));

      var minutes = TimeUnitHelper.ToMilliseconds(10, TimeUnit.Minutes);
      Assert.That(minutes, Is.EqualTo(10*60*1000));
    }

    [Test]
    public void ShouldConvertFromUnixEpochToLocal() {
      Assert.That(TimeUnitHelper.FromUnixEpoch(1369686995),
        Is.EqualTo(new DateTime(2013, 5, 27, 17, 36, 35, DateTimeKind.Local)));
      Assert.That(TimeUnitHelper.FromUnixEpoch(1369686995, DateTimeKind.Local),
        Is.EqualTo(new DateTime(2013, 5, 27, 17, 36, 35, DateTimeKind.Local)));

      Assert.That(TimeUnitHelper.FromUnixEpoch(1369686995),
        Is.EqualTo(new DateTime(2013, 5, 27, 17, 36, 35)));
      Assert.That(TimeUnitHelper.FromUnixEpoch(1369686995),
        Is.EqualTo(new DateTime(2013, 5, 27, 17, 36, 35)));
    }

    [Test]
    public void ShouldConvertFromUnixEpochToUTC() {
      var date_time = TimeUnitHelper.FromUnixEpoch(1369686995, DateTimeKind.Utc);
      Assert.That(date_time,
        Is.EqualTo(new DateTime(2013, 5, 27, 20, 36, 35, DateTimeKind.Utc)));
    }
  }
}
