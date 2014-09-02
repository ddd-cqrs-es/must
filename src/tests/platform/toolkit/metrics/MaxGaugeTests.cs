using NUnit.Framework;

namespace Nohros.Metrics.Tests
{
  public class MaxGaugeTests
  {
    [Test]
    public void should_retain_the_max_updated_value() {
      var gauge = new MaxGauge(new MetricConfig("max1"));
      gauge.Update(42L);

      Measure measure = Testing.Sync<Measure>(gauge, gauge.GetMeasure,
        gauge.context_);
      Assert.That(measure.Value, Is.EqualTo(42.0));

      gauge = new MaxGauge(new MetricConfig("max1"));
      gauge.Update(42L);
      gauge.Update(420L);

      measure = Testing.Sync<Measure>(gauge, gauge.GetMeasure, gauge.context_);
      Assert.That(measure.Value, Is.EqualTo(420.0));

      gauge = new MaxGauge(new MetricConfig("max1"));
      gauge.Update(42L);
      gauge.Update(420L);
      gauge.Update(1L);

      measure = Testing.Sync<Measure>(gauge, gauge.GetMeasure, gauge.context_);
      Assert.That(measure.Value, Is.EqualTo(420.0));
    }
  }
}
