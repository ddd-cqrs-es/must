using System;

namespace Nohros.Metrics.Reporting
{
  /// <summary>
  /// A reporter that receives updates about measures.
  /// </summary>
  public interface IMeasureObserver
  {
    /// <summary>
    /// Observe the most recent measure of a metric.
    /// </summary>
    /// <param name="measure">
    /// The most recent measure of a metrics.
    /// </param>
    void Observe(Measure measure);
  }
}
