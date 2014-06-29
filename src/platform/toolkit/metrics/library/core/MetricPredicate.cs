using System;

namespace Nohros.Metrics
{
  /// <summary>
  /// Represents the method that defines a set of criteria and determines
  /// whether a metric shoudl be included when filtering metrics.
  /// </summary>
  /// <param name="id">
  /// A <see cref="MetricId"/> object that uniquely identifies the metric.
  /// </param>
  /// <param name="value">
  /// The metric's value of the metric associated with the <paramref name="id"/>.
  /// </param>
  /// <returns>
  /// <c>true</c> if <paramref name="id"/> and <paramref name="value"/>
  /// meets the criteria defined whithin the method represented by this
  /// delegate.
  /// </returns>
  /// <remarks>
  /// This delegate is used by implementations of the IMetricsReporter class
  /// to filter metrics while reporting.
  /// </remarks>
  public delegate bool MetricPredicate(MetricId id, MetricValue value);
}
