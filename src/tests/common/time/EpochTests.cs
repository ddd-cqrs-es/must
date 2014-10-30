using System;
using NUnit.Framework;
using Nohros.Extensions.Time;

namespace Nohros.time
{
  public class EpochTests
  {
    [Test]
    public void shoultd_convert_epoch_to_local_time() {
      var date = new DateTime(2014, 9, 24, 0, 0, 0, DateTimeKind.Local);
      long epoch = 1411527600; // 24/09/2014 00:00:00 (dd/mm/yyyy HH:mm:ss)
      Assert.That(epoch.FromUnixEpoch(), Is.EqualTo(date));
      Assert.That(epoch.FromUnixEpoch(DateTimeKind.Local), Is.EqualTo(date));
    }

    [Test]
    public void should_convert_epoch_to_utc() {
      var date = new DateTime(2014, 9, 24, 0, 0, 0, DateTimeKind.Local)
        .ToUniversalTime();
      long epoch = 1411527600; // 24/09/2014 00:00:00 (dd/mm/yyyy HH:mm:ss)
      Assert.That(epoch.FromUnixEpoch(DateTimeKind.Utc), Is.EqualTo(date));
    }

    [Test]
    public void should_convert_local_date_to_epoch() {
      var date = new DateTime(2014, 9, 24, 0, 0, 0, DateTimeKind.Local);
      long epoch = 1411527600; // 24/09/2014 00:00:00 (dd/mm/yyyy HH:mm:ss)
      Assert.That(date.ToUnixEpoch(), Is.EqualTo(epoch));
    }

    [Test]
    public void should_convert_utc_date_to_epoch() {
      var date = new DateTime(2014, 9, 24, 0, 0, 0, DateTimeKind.Local)
        .ToUniversalTime();
      long epoch = 1411527600; // 24/09/2014 00:00:00 (dd/mm/yyyy HH:mm:ss)
      Assert.That(date.ToUnixEpoch(), Is.EqualTo(epoch));
    }
  }
}
