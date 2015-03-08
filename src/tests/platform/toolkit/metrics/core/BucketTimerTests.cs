using System.Threading;
using NUnit.Framework;

namespace Nohros.Metrics.Tests
{
  public class BucketTimerTests
  {
    [Test]
    public void should_measure_the_rate_of_events() {
      BucketTimer timer = BucketTimer.Create("tests", new long[] { 10, 20 });
      timer.Time(() => Thread.Sleep(100));
      timer.Time(() => Thread.Sleep(100));

      foreach (IMetric metric in timer.Metrics) {
        if (metric is Counter) {
        }
        Measure measure = Testing.Sync<Measure>(metric, metric.GetMeasure,
          timer.context_);
        Assert.That(measure.IsObservable, Is.False);
      }
    }
  }
}