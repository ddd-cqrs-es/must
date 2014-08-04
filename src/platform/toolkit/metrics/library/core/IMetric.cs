using System;

namespace Nohros.Metrics
{
  /// <summary>
  /// A tag interface to indicate that a class is a metric.
  /// </summary>
  public interface IMetric
  {
    /// <summary>
    /// Gets the the collection of values associated with the metric.
    /// </summary>
    /// <param name="callback">
    /// A <see cref="MetricReportCallback{T}"/> that is called to report the
    /// metric's value.
    /// </param>
    /// <param name="context">
    /// A user-defined object that qualifies or contains information about the
    /// reporting operation.
    /// </param>
    void Report<T>(MetricReportCallback<T> callback, T context);

#if DEBUG
    void Run(Action action);
#endif
  }
}
