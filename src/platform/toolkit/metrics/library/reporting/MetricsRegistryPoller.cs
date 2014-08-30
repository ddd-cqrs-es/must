using System;
using System.Collections.Generic;

namespace Nohros.Metrics.Reporting
{
  /// <summary>
  /// A poller that can be used to fetch current values from a
  /// <see cref="IMetricsRegistry"/> on demand and sends the measures to all
  /// associated observers.
  /// </summary>
  public class MetricsRegistryPoller : IMetricsPoller
  {
    readonly List<IMeasureObserver> observers_;
    readonly IMetricsRegistry registry_;

    public MetricsRegistryPoller(IMeasureObserver observer,
      IMetricsRegistry registry) : this(new[] {observer}, registry) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MetricsRegistryPoller"/>
    /// class by using the given <paramref name="registry"/> to be polled.
    /// </summary>
    /// <param name="observers">
    /// A collection of <see cref="IMeasureObserver"/> objects that will
    /// receive the measured values on each poll.
    /// </param>
    /// <param name="registry">
    /// The <see cref="IMetricsRegistry"/> to be polled.
    /// </param>
    public MetricsRegistryPoller(IEnumerable<IMeasureObserver> observers,
      IMetricsRegistry registry) {
      observers_ = new List<IMeasureObserver>(observers);
      registry_ = registry;
    }

    /// <summary>
    /// Fetch the current values the set of metrics that was registered on
    /// a <see cref="IMetricsRegistry"/>.
    /// </summary>
    public void Poll() {
      Poll(config => true);
    }

    /// <summary>
    /// Fetch the current values the set of metrics that was registered on
    /// a <see cref="IMetricsRegistry"/> and match the provided
    /// <paramref name="predicate"/>.
    /// </summary>
    /// <param name="predicate">
    /// A <see cref="Func{TResult}"/> that restricts the measures that should
    /// be fetched.
    /// </param>
    public void Poll(Func<MetricConfig, bool> predicate) {
      DateTime now = DateTime.Now;
      Poll(registry_.Metrics, predicate, now);
    }

    void Poll(IEnumerable<IMetric> metrics, Func<MetricConfig, bool> predicate,
      DateTime timestamp) {
      foreach (var metric in metrics) {
        if (metric is ICompositeMetric) {
          Poll((ICompositeMetric) metric, predicate, timestamp);
        } else if (predicate(metric.Config)) {
          metric.GetMeasure(Observe, timestamp);
        }
      }
    }

    void Observe(Measure measure, DateTime timestamp) {
      foreach (var observer in observers_) {
        observer.Observe(measure, timestamp);
      }
    }
  }
}
