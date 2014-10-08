using NUnit.Framework;
using Nohros.Extensions.Time;

namespace Nohros.Concurrent
{
  public class TimeConversionTests
  {
    const long C1 = 1L;
    const long C2 = C1*100;
    const long C3 = C2*10;
    const long C4 = C3*1000;
    const long C5 = C4*1000;
    const long C6 = C5*60;
    const long C7 = C6*60;
    const long C8 = C7*24;

    [Test]
    public void should_convert_unit_to_nanoseconds() {
      Assert.That(1.ToNanos(TimeUnit.Nanoseconds), Is.EqualTo(C1));
      Assert.That(1.ToNanos(TimeUnit.Ticks), Is.EqualTo(C2/C1));
      Assert.That(1.ToNanos(TimeUnit.Microseconds), Is.EqualTo(C3/C1));
      Assert.That(1.ToNanos(TimeUnit.Milliseconds), Is.EqualTo(C4/C1));
      Assert.That(1.ToNanos(TimeUnit.Seconds), Is.EqualTo(C5/C1));
      Assert.That(1.ToNanos(TimeUnit.Minutes), Is.EqualTo(C6/C1));
      Assert.That(1.ToNanos(TimeUnit.Hours), Is.EqualTo(C7/C1));
      Assert.That(1.ToNanos(TimeUnit.Days), Is.EqualTo(C8/C1));
    }

    [Test]
    public void should_convert_unit_to_ticks() {
      Assert.That((C2/C1).ToTicks(TimeUnit.Nanoseconds), Is.EqualTo(1));
      Assert.That(1.ToTicks(TimeUnit.Ticks), Is.EqualTo(1));
      Assert.That(1.ToTicks(TimeUnit.Microseconds), Is.EqualTo(C3/C2));
      Assert.That(1.ToTicks(TimeUnit.Milliseconds), Is.EqualTo(C4/C2));
      Assert.That(1.ToTicks(TimeUnit.Seconds), Is.EqualTo(C5/C2));
      Assert.That(1.ToTicks(TimeUnit.Minutes), Is.EqualTo(C6/C2));
      Assert.That(1.ToTicks(TimeUnit.Hours), Is.EqualTo(C7/C2));
      Assert.That(1.ToTicks(TimeUnit.Days), Is.EqualTo(C8/C2));
    }

    [Test]
    public void should_convert_unit_to_miliseconds() {
      Assert.That((C4/C1).ToMilliseconds(TimeUnit.Nanoseconds), Is.EqualTo(1));
      Assert.That((C4/C2).ToMilliseconds(TimeUnit.Ticks), Is.EqualTo(1));
      Assert.That((C4/C3).ToMilliseconds(TimeUnit.Microseconds), Is.EqualTo(1));
      Assert.That(1.ToMilliseconds(TimeUnit.Milliseconds), Is.EqualTo(1));
      Assert.That(1.ToMilliseconds(TimeUnit.Seconds), Is.EqualTo(C5/C4));
      Assert.That(1.ToMilliseconds(TimeUnit.Minutes), Is.EqualTo(C6/C4));
      Assert.That(1.ToMilliseconds(TimeUnit.Hours), Is.EqualTo(C7/C4));
      Assert.That(1.ToMilliseconds(TimeUnit.Days), Is.EqualTo(C8/C4));
    }

    [Test]
    public void should_convert_unit_to_seconds() {
      Assert.That((C5/C1).ToSeconds(TimeUnit.Nanoseconds), Is.EqualTo(1));
      Assert.That((C5/C2).ToSeconds(TimeUnit.Ticks), Is.EqualTo(1));
      Assert.That((C5/C3).ToSeconds(TimeUnit.Microseconds), Is.EqualTo(1));
      Assert.That((C5/C4).ToSeconds(TimeUnit.Milliseconds), Is.EqualTo(1));
      Assert.That((C5/C5).ToSeconds(TimeUnit.Seconds), Is.EqualTo(1));
      Assert.That(1.ToSeconds(TimeUnit.Minutes), Is.EqualTo(C6/C5));
      Assert.That(1.ToSeconds(TimeUnit.Hours), Is.EqualTo(C7/C5));
      Assert.That(1.ToSeconds(TimeUnit.Days), Is.EqualTo(C8/C5));
    }
  }
}
