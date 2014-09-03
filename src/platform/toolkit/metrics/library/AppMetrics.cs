using System;
using System.Collections.Generic;
using System.Configuration;

namespace Nohros.Metrics
{
  /// <summary>
  /// A default registry that delegates all actions to a class specified by
  /// the key "DefaultMetricsRegistry" in the
  /// <see cref="ConfigurationManager.AppSettings"/> configuration section. The
  /// specified registry class must have a cinstructor with no arguments. If
  /// the property is not specified ir the class cannot be loaded an instance
  /// of the <see cref="MetricsRegistry"/> will be used.
  /// </summary>
  public class AppMetrics
  {
    const string kDefaultMetricsRegistryKey = "DefaultMetricsRegistry";

    static AppMetrics() {
      string default_metrics_registry_class =
        ConfigurationManager.AppSettings[kDefaultMetricsRegistryKey];

      if (default_metrics_registry_class != null) {
        var runtime_type = new RuntimeType(default_metrics_registry_class);

        Type type = RuntimeType.GetSystemType(runtime_type);

        if (type != null) {
          ForCurrentProcess =
            RuntimeTypeFactory<IMetricsRegistry>
              .CreateInstanceFallback(runtime_type);
        }
      }

      if (ForCurrentProcess == null) {
        ForCurrentProcess = new MetricsRegistry();
      }
    }

    /// <inheritdoc/>
    public static AppMetrics Register(IMetric metric) {
      ForCurrentProcess.Register(metric);
    }

    public static void Register(IEnumerable<IMetric> metrics) {
      foreach (var metric in metrics) {
        Register(metric);
      }
    }

    /// <inheritdoc/>
    public static void Unregister(IMetric metric) {
      ForCurrentProcess.Unregister(metric);
    }

    /// <inheritdoc/>
    public ICollection<IMetric> Metrics { get; private set; }

    /// <summary>
    /// Gets the default configured <see cref="IMetricsRegistry"/>.
    /// </summary>
    public static IMetricsRegistry ForCurrentProcess { get; private set; }
  }
}
