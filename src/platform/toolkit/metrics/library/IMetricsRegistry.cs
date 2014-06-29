using System;
using Nohros.Metrics.Reporting;

namespace Nohros.Metrics
{
  /// <summary>
  /// A central registry for metric instances.
  /// </summary>
  public interface IMetricsRegistry
  {
    /// <summary>
    /// Gets the the collection of values associated with all the registered
    /// metrics.
    /// </summary>
    /// <param name="callback">
    /// A <see cref="MetricsReportCallback{T}"/> that is called to
    /// report the metric's value.
    /// </param>
    /// <param name="context">
    /// A user-defined object that qualifies or contains information about the
    /// reporting operation.
    /// </param>
    void Report<T>(MetricsReportCallback<T> callback, T context);

    /// <summary>
    /// Gets the the collection of values associated with the metrics that
    /// matches the criteria defined by the <paramref name="predicate"/>.
    /// </summary>
    /// <param name="callback">
    /// A <see cref="MetricsReportCallback{T}"/> that is called to
    /// report the metric's value.
    /// </param>
    /// <param name="context">
    /// A user-defined object that qualifies or contains information about the
    /// reporting operation.
    /// </param>
    /// <param name="predicate">
    /// A <see cref="MetricPredicate"/> delegate that defines the conditions
    /// of the metrics to report.
    /// </param>
    void Report<T>(MetricsReportCallback<T> callback, T context,
      MetricPredicate predicate);

    /// <summary>
    /// Adds an metric to the metrics collection using the metrics name.
    /// </summary>
    /// <param name="name">
    /// The id of the metric.
    /// </param>
    /// <param name="metric">
    /// The metric to be added.
    /// </param>
    /// <exception cref="ArgumentException">
    /// A metric with the same id already exists in the
    /// <see cref="IMetricsRegistry"/>.
    /// </exception>
    void Add(string name, IMetric metric);

    /// <summary>
    /// Adds an metric to the metrics collection using the metrics id.
    /// </summary>
    /// <param name="id">
    /// The id of the metric.
    /// </param>
    /// <param name="metric">
    /// The metric to be added.
    /// </param>
    /// <exception cref="ArgumentException">
    /// A metric with the same id already exists in the
    /// <see cref="IMetricsRegistry"/>.
    /// </exception>
    void Add(MetricId id, IMetric metric);

    /// <summary>
    /// Indicates if a metric of the given id has been registered.
    /// </summary>
    /// <param name="id">
    /// The id of the metric to check.
    /// </param>
    bool HasMetric(MetricId id);

    /// <summary>
    /// Gets a metric associated with the specified metric's id.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="id">
    /// The id of the metric to get.
    /// </param>
    /// <param name="metric">
    /// When this method returns, contains the metric associated with specified
    /// metric's id, if the id is foundç otherwise, teh default value for the
    /// type <typeparamref name="T"/>
    /// </param>
    /// <returns></returns>
    bool TryGetMetric<T>(MetricId id, out T metric) where T : IMetric;

    /// <summary>
    /// Gets a metric associated with specified metric's id or create a new one
    /// using the given <paramref name="factory"/> if <paramref name="id"/> is
    /// not found in the registry.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the metric to retrieve.
    /// </typeparam>
    /// <param name="id">
    /// The id of the metric to get.
    /// </param>
    /// <param name="factory">
    /// A <see cref="Func{T, Result}"/> that can be used to created a metric of
    /// type <typeparamref name="T"/>.
    /// </param>
    /// <returns>
    /// A metric associated with the <paramref name="id"/>.
    /// </returns>
    T GetMetric<T>(MetricId id, Func<MetricId, T> factory) where T : IMetric;

    /// <summary>
    /// Raised when a new metric is added to the registry.
    /// </summary>
    event MetricAddedEventHandler MetricAdded;
  }
}
