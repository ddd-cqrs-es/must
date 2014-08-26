using System;

namespace Nohros.Metrics
{
  /// <summary>
  /// A singleton <see cref="IMetricsRegistry"/> that fowards its methods to
  /// another <see cref="IMetricsRegistry"/> and uses the
  /// <see cref="MetricsRegistry"/> as the default registry.
  /// </summary>
  public class AppMetrics : ForwardingMetricsRegistry
  {
    static AppMetrics() {
      ForCurrentProcess = new AppMetrics(new MetricsRegistry());
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AppMetrics"/> class
    /// by using the given registry.
    /// </summary>
    public AppMetrics(IMetricsRegistry registry) : base(registry) {
    }

    /// <summary>
    /// Gets the current <see cref="IMetricsRegistry"/>.
    /// </summary>
    public static AppMetrics ForCurrentProcess { get; set; }
  }
}
