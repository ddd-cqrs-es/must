using System;
using System.Collections.Generic;
using Nohros.Metrics.Reporting;

namespace Nohros.Metrics
{
  /// <summary>
  /// A set of factory methods for creating centrally registered metric
  /// instances.
  /// </summary>
  /// <remarks>
  /// A <see cref="SyncMetricsRegistry"/> is used as the default metrics
  /// registry. If you want to use another <see cref="IMetricsRegistry"/>
  /// implementation, set the value of the property <see cref="Registry"/>.
  /// </remarks>
  public class AppMetrics
  {
    static IMetricsRegistry registry_;
    static readonly IDictionary<string, IMetricsReporter> reporters_;

    /// <summary>
    /// Initializes the siatic members.
    /// </summary>
    static AppMetrics() {
      registry_ = new SyncMetricsRegistry();
      reporters_ = new Dictionary<string, IMetricsReporter>();
    }

    /// <summary>
    /// Stops all the registered reporters and deallocates any associated
    /// resources.
    /// </summary>
    public static void Shutdown() {
      foreach (IMetricsReporter reporter in reporters_.Values) {
        reporter.Shutdown();
      }
    }

    /// <summary>
    /// Register a <see cref="IMetricsReporter"/> under the given reporter
    /// name.
    /// </summary>
    /// <param name="name">
    /// The name of the reporter.
    /// </param>
    /// <param name="reporter">
    /// The reporter to be registerd.
    /// </param>
    /// <remarks>
    /// This method allows a <see cref="IMetricsReporter"/> object to have the
    /// same lifecycle of the <see cref="AppMetrics"/> class without needing
    /// to be referenced by other classes.
    /// <para>
    /// All registered <see cref="IMetricsReporter"/> are shutedown when the
    /// <see cref="Shutdown"/> method is called.
    /// </para>
    /// </remarks>
    public static void AddReporter(string name, IMetricsReporter reporter) {
      reporters_[name] = reporter;
    }

    /// <summary>
    /// Adds an metric to the metrics collection using the metrics id.
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
    public static void AddMetric(string name, IMetric metric) {
      registry_.Add(name, metric);
    }

    public static bool HasMetric(MetricId id) {
      return registry_.HasMetric(id);
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
    public static void AddMetric(MetricId id, IMetric metric) {
      registry_.Add(id, metric);
    }

    /// <summary>
    /// Raised when a new metric is added to the registry.
    /// </summary>
    public static event MetricAddedEventHandler MetricAdded {
      add { registry_.MetricAdded += value; }
      remove { registry_.MetricAdded -= value; }
    }

    /// <summary>
    /// Gets or set the singleton metrics object.
    /// </summary>
    /// <remarks>
    /// The default value of <see cref="Registry"/> is
    /// <see cref="SyncMetricsRegistry"/>.
    /// </remarks>
    public static IMetricsRegistry Registry {
      get { return registry_; }
      set {
        if (registry_ == null) {
          throw new ArgumentNullException("value");
        }
        registry_ = value;
      }
    }
  }
}
