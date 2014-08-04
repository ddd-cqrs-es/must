using System;
using System.Collections.Generic;
using System.Linq;
using Nohros.Metrics.Reporting;

namespace Nohros.Metrics
{
  public class MetricsRegistry
  {
    const int kExpectedMetricCount = 1024;

    readonly Dictionary<MetricId, IMetric> metrics_;

    /// <summary>
    /// Initializes a new instance of the <see cref="MetricsRegistry"/> class.
    /// </summary>
    public MetricsRegistry() {
      metrics_ = new Dictionary<MetricId, IMetric>(kExpectedMetricCount);
    }

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
    public void Report<T>(MetricsReportCallback<T> callback, T context) {
      // The code above is the same as the Report method that accepts a
      // predicate, and it is duplicated to avoid overhead of calling the
      // predicate method when there is no need.
      foreach (KeyValuePair<MetricId, IMetric> metric in metrics_) {
        MetricId id = metric.Key;
        metric.Value.Report((metrics, ctx) =>
          callback(id, metrics.Values, context), context);
      }
    }

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
    public void Report<T>(MetricsReportCallback<T> callback, T context,
      MetricPredicate predicate) {
      foreach (KeyValuePair<MetricId, IMetric> metric in metrics_) {
        MetricId id = metric.Key;
        metric.Value.Report((metrics, ctx) => {
          var list = metrics.Where(m => predicate(id, m));
          callback(id, list.ToArray(), context);
        }, context);
      }
    }

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
    public virtual void Add(string name, IMetric metric) {
      Add(new MetricId(name), metric);
    }

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
    public virtual void Add(MetricId id, IMetric metric) {
      metrics_.Add(id, metric);
      OnMetricAdded(id, metric);
    }

    /// <inheritdoc/>
    public bool HasMetric(MetricId id) {
      return metrics_.ContainsKey(id);
    }

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
    public bool TryGetMetric<T>(MetricId id, out T metric) where T : IMetric {
      IMetric mtc;
      if (metrics_.TryGetValue(id, out mtc)) {
        metric = (T) mtc;
        return true;
      }
      metric = default(T);
      return false;
    }

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
    public T GetMetric<T>(MetricId id, Func<MetricId, T> factory)
      where T : IMetric {
      T metric;
      if (!TryGetMetric(id, out metric)) {
        metric = factory(id);
        Add(id, metric);
      }
      return metric;
    }

    /// <summary>
    /// Raised when a new metric is added to the registry.
    /// </summary>
    public virtual event MetricAddedEventHandler MetricAdded;

    void OnMetricAdded(MetricId id, IMetric metric) {
      Listeners.SafeInvoke(MetricAdded,
        (MetricAddedEventHandler handler) => handler(id, metric));
    }
  }
}
