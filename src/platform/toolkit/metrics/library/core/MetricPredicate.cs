using System;

namespace Nohros.Metrics
{
  /// <summary>
  /// Represents the method that defines a set of criteria and determines
  /// whether a metric shoudl be included when filtering metrics.
  /// </summary>
  /// <param name="config">
  /// A <see cref="MetricConfig"/> object that uniquely identifies the metric.
  /// </param>
  /// <param name="value">
  /// The metric's value of the metric associated with the
  /// <paramref name="config"/>.
  /// </param>
  /// <returns>
  /// <c>true</c> if <paramref name="config"/> and <paramref name="value"/>
  /// meets the criteria defined whithin the method represented by this
  /// delegate.
  /// </returns>
  /// <remarks>
  /// This delegate is used by implementations of the IMeasureObserver class
  /// to filter metrics while reporting.
  /// </remarks>
  public delegate bool MetricPredicate(MetricConfig config, Measure value);
}
