using Nohros.Metrics.Reporting;

namespace Nohros.Metrics
{
  /// <summary>
  /// Defines a metric that is mapped to a particular step interval, which
  /// means that it should should be automatically reseted by a
  /// <see cref="IMetricsPoller"/> on every poll operation.
  /// </summary>
  public interface IStepMetric
  {
    /// <summary>
    /// Reset the value of the metric.
    /// </summary>
    void Reset();
  }
}