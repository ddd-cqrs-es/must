using NUnit.Framework;

namespace Nohros.Metrics.Tests
{
  public class MinGaugeTests
  {
    [Test]
    public void should_retain_the_min_updated_value() {
      var gauge = new MinGauge(new MetricConfig("min1"));
      gauge.Update(42L);

      Measure measure = Testing.Sync<Measure>(gauge, gauge.GetMeasure);
      Assert.That(measure.Value, Is.EqualTo(42.0));

      gauge = new MinGauge(new MetricConfig("min1"));
      gauge.Update(42L);
      gauge.Update(420L);

      measure = Testing.Sync<Measure>(gauge, gauge.GetMeasure);
      Assert.That(measure.Value, Is.EqualTo(42.0));

      gauge = new MinGauge(new MetricConfig("min1"));
      gauge.Update(42L);
      gauge.Update(420L);
      gauge.Update(1L);

      measure = Testing.Sync<Measure>(gauge, gauge.GetMeasure);
      Assert.That(measure.Value, Is.EqualTo(1.0));
    }
  }
}
