using System;
using System.Threading;
using NUnit.Framework;
using Nohros.Concurrent;

namespace Nohros.Metrics
{
  public class AsyncTimerTests
  {
    class ClockMock : Clock
    {
      long val_ = 0;

      public override long Tick {
        get { return val_ += 50000000; }
      }
    }

    [Test]
    public void ShouldHasDurationUnit() {
      var timer = AsyncTimer;
      Assert.That(timer.DurationUnit, Is.EqualTo(TimeUnit.Milliseconds));
    }

    [Test]
    public void ShouldCreateEmptyTimer() {
      var signaler = new ManualResetEvent(false);
      var timer = AsyncTimer;
      timer.GetCount((metric, context) => Assert.That(metric, Is.EqualTo(0)));
      timer.GetMax((metric, context) =>
        Assert.That(metric, Is.InRange(0, 0.0001)));
      timer.GetMin((metric, context) =>
        Assert.That(metric, Is.InRange(0, 0.0001)));
      timer.GetMean((metric, context) =>
        Assert.That(metric, Is.InRange(0, 0.0001)));
      timer.GetStandardDeviation((metric, context) =>
        Assert.That(metric, Is.InRange(0, 0.0001)));

      timer.GetSnapshot((snapshot, context) => {
        Assert.That(snapshot.Median, Is.InRange(0, 0.0001));
        Assert.That(snapshot.Percentile75, Is.InRange(0, 0.0001));
        Assert.That(snapshot.Percentile99, Is.InRange(0, 0.0001));
        Assert.That(snapshot.Size, Is.EqualTo(0));
        signaler.Set();
      });

      signaler.WaitOne(3000);
    }

    [Test]
    public void ShouldTimeSeriesOfEvents() {
      var signaler = new ManualResetEvent(false);
      var timer = AsyncTimer;
      timer.Update(10, TimeUnit.Milliseconds);
      timer.Update(20, TimeUnit.Milliseconds);
      timer.Update(20, TimeUnit.Milliseconds);
      timer.Update(30, TimeUnit.Milliseconds);
      timer.Update(40, TimeUnit.Milliseconds);

      timer.GetCount((metric, context) => Assert.That(metric, Is.EqualTo(5)));
      timer.GetMax((metric, context) =>
        Assert.That(metric, Is.InRange(40.0, 40.0001)));
      timer.GetMin((metric, context) =>
        Assert.That(metric, Is.InRange(10, 10.0001)));
      timer.GetMean((metric, context) =>
        Assert.That(metric, Is.InRange(24, 24.0001)));
      timer.GetStandardDeviation((metric, context) =>
        Assert.That(metric, Is.InRange(11.401, 11.402)));

      timer.GetSnapshot((snapshot, context) => {
        Assert.That(snapshot.Median, Is.InRange(20, 20.0001));
        Assert.That(snapshot.Percentile75, Is.InRange(35, 35.0001));
        Assert.That(snapshot.Percentile99, Is.InRange(40, 40.0001));
        Assert.That(snapshot.Values,
          Is.EqualTo(new double[] {10, 20, 20, 30, 40}));
        signaler.Set();
      });
    }

    [Test]
    public void ShoulTimeVariantValues() {
      var signaler = new ManualResetEvent(false);
      var timer = AsyncTimer;
      timer.Update(long.MaxValue, TimeUnit.Nanoseconds);
      timer.Update(0, TimeUnit.Nanoseconds);

      timer.GetStandardDeviation((metric, context) => {
        Assert.That(metric, Is.InRange(6.521e12, 6.522e12));
        signaler.Set();
      });
      if (!signaler.WaitOne(3000)) {
        Assert.Fail();
      }
    }

    [Test]
    public void ShouldTimeCallableInstances() {
      var signaler = new ManualResetEvent(false);
      var timer = AsyncTimer;
      string value = timer.Time(() => "one");

      timer.GetCount((metric, context) =>
        Assert.That(metric, Is.EqualTo(1), "the timer has count of 1"));
      Assert.That(value, Is.EqualTo("one"), "returns the result of callable");
      timer.GetMax((metric, context) => {
        Assert.That(metric, Is.InRange(50.0, 50.001),
          "records the duration of the method call");
        signaler.Set();
      });

      if (!signaler.WaitOne(3000)) {
        Assert.Fail();
      }
    }

    [Test]
    public void ShouldTimeThroughContext() {
      var signaler = new ManualResetEvent(false);
      var timer = AsyncTimer;
      timer.Time().Stop();

      timer.GetCount((metric, context) =>
        Assert.That(metric, Is.EqualTo(1), "the timer has count of 1"));

      timer.GetMax((metric, context) => {
        Assert.That(metric, Is.InRange(50.0, 50.001),
          "record the duration of the context");
        signaler.Set();
      });
      if (!signaler.WaitOne(3000)) {
        Assert.Fail();
      }
    }

    public AsyncTimer AsyncTimer {
      get {
        return new AsyncTimer(TimeUnit.Milliseconds,
          new Meter("calls", TimeUnit.Seconds),
          Histograms.Biased(),
          new ClockMock());
      }
    }
  }
}
