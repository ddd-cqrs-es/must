using System;
using System.Collections.Generic;
using System.Linq;
using Nohros.Metrics.Reporting;

namespace Nohros.Metrics
{
  public abstract class AbstractMetricsRegistry : IMetricsRegistry
  {
    const int kExpectedMetricCount = 1024;
    readonly Dictionary<MetricId, IMetric> metrics_;

    /// <summary>
    /// Initializes a new instance of the <see cref="SyncMetricsRegistry"/> class.
    /// </summary>
    protected AbstractMetricsRegistry() {
      metrics_ = new Dictionary<MetricId, IMetric>(kExpectedMetricCount);
    }

    /// <inheritdoc/>
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

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public virtual void Add(string name, IMetric metric) {
      Add(new MetricId(name), metric);
    }

    /// <inheritdoc/>
    public virtual void Add(MetricId id, IMetric metric) {
      metrics_.Add(id, metric);
      OnMetricAdded(id, metric);
    }

    /// <inheritdoc/>
    public bool HasMetric(MetricId id) {
      return metrics_.ContainsKey(id);
    }

    /// <inheritdoc/>
    public bool TryGetMetric<T>(MetricId id, out T metric) where T : IMetric {
      IMetric mtc;
      if (metrics_.TryGetValue(id, out mtc)) {
        metric = (T) mtc;
        return true;
      }
      metric = default(T);
      return false;
    }

    /// <inheritdoc/>
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

    protected void OnMetricAdded(MetricId id, IMetric metric) {
      Listeners.SafeInvoke(MetricAdded,
        (MetricAddedEventHandler handler) => handler(id, metric));
    }
  }
}
