using System;

namespace Nohros.Metrics
{
  /// <summary>
  /// A singleton <see cref="IMetricsRegistry"/>.
  /// </summary>
  /// <remarks>
  /// A <see cref="MetricsRegistry"/> is used as the default metrics
  /// registry. If you want to use another <see cref="MetricsRegistry"/>
  /// implementation, set the value of the property
  /// <see cref="ForCurrentProcess"/>.
  /// </remarks>
  public class AppMetrics
  {
    static readonly MetricsRegistry registry_;

    public static IMetricsRegistry ForCurrentProcess { get; set; }
  }
}
