using NUnit.Framework;

namespace Nohros.Metrics.Tests
{
  public class BucketTimerTests
  {
    [Test]
    public void should_create_non_observable_measures_for_not_updated_buckets() {
      BucketTimer timer = BucketTimer.Create("tests", new long[] { 10, 20 });

      foreach (IMetric metric in timer.Metrics) {
        Measure measure = Testing.Sync<Measure>(metric, metric.GetMeasure,
          timer.context_);
        Assert.That(measure.IsObservable, Is.False);
      }
    }
  }
}