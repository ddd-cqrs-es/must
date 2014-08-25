using System;
using System.Collections.Generic;

namespace Nohros.Metrics.Reporting
{
  /// <summary>
  /// A poller that can be used to fetch current values from a
  /// <see cref="IMetricsRegistry"/> on demand and sends the measures to all
  /// associated observers.
  /// </summary>
  public class MetricsRegistryPoller
  {
    readonly TimeSpan interval_;
    readonly List<IMeasureObserver> observers_;
    readonly IMetricsRegistry registry_;

    /// <summary>
    /// Initializes a new <see cref="MetricsRegistryPoller"/> class by using
    /// the default metrics registry and given <paramref name="interval"/>.
    /// </summary>
    /// <param name="interval">
    /// A <see cref="TimeSpan"/> that defines how frequently to poll measures
    /// and report to observers.
    /// </param>
    /// <param name="observers">
    /// A collection of <see cref="IMeasureObserver"/> objects that will
    /// receive the measured values on each poll.
    /// </param>
    /// <seealso cref="AppMetrics"/>
    public MetricsRegistryPoller(TimeSpan interval, IMeasureObserver observers)
      : this(interval, observers, AppMetrics.ForCurrentProcess) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MetricsRegistryPoller"/>
    /// class by using the given <paramref name="registry"/> and polling
    /// <paramref name="interval"/>.
    /// </summary>
    /// <param name="interval">
    /// A <see cref="TimeSpan"/> that defines how frequently to poll measures
    /// and report to observers.
    /// </param>
    /// <param name="observers">
    /// A collection of <see cref="IMeasureObserver"/> objects that will
    /// receive the measured values on each poll.
    /// </param>
    /// <param name="registry">
    /// The <see cref="IMetricsRegistry"/> to be polled at given
    /// <paramref name="interval"/>.
    /// </param>
    public MetricsRegistryPoller(TimeSpan interval, IMeasureObserver observers,
      IMetricsRegistry registry) {
      observers_ = observers;
      registry_ = registry;
    }

    /// <summary>
    /// Fetch the current values the set of metrics that was registered on
    /// a <see cref="IMetricsRegistry"/>.
    /// </summary>
    public void Poll() {
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
      Poll();
    }

    void Poll(IEnumerable<IMetric> metrics, Func<MetricConfig, bool> predicate,
      DateTime timestamp) {
      foreach (var metric in metrics) {
        if (metric is CompositeMetric) {
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
