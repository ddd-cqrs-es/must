using System;

namespace Nohros.Metrics.Reporting
{
  /// <summary>
  /// A simple reporter which prints out application metrics to the
  /// <see cref="System.Console"/> periodically.
  /// </summary>
  public class ConsoleReporter : AbstractPollingReporter
  {
    public ConsoleReporter(MetricsRegistry registry) : base(registry) {
    }

    /// <inheritdoc/>
    public override void Run(MetricPredicate predicate) {
      var now = DateTime.UtcNow;
      MetricsRegistry.Report(Report, now, predicate);
    }

    /// <inheritdoc/>
    public override void Run() {
      var registry = MetricsRegistry;
      var now = DateTime.UtcNow;
      registry.Report(Report, now);
    }

    void Report(MetricId metric_id, MetricValue[] metrics,
      DateTime timestamp) {
      foreach (MetricValue mtc in metrics) {
        Console.Write(timestamp.ToString("yyyy-MM-ddTHH:mm:ssZ") + ":"
          + metric_id + ".");
        Console.Write(GetMetricName(mtc.Type) + "=" + mtc.Value.ToString("f4"));
        Console.WriteLine(" " + mtc.Unit);
      }
    }

    string GetMetricName(int type) {
      return string.Empty;
    }
  }
}
