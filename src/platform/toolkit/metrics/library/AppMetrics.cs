using System;
using System.Collections.Generic;

namespace Nohros.Metrics
{
  /// <summary>
  /// A singleton <see cref="IMetricsRegistry"/> that fowards its methods to
  /// another <see cref="IMetricsRegistry"/> and uses the
  /// <see cref="MetricsRegistry"/> as the default registry.
  /// </summary>
  public class AppMetrics
  {
    static AppMetrics() {
      ForCurrentProcess = new MetricsRegistry();
    }

    /// <inheritdoc/>
    public static void Register(IMetric metric) {
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
    /// Gets the current <see cref="IMetricsRegistry"/>.
    /// </summary>
    public static IMetricsRegistry ForCurrentProcess { get; set; }
  }
}
