namespace Nohros.Metrics
{
  /// <summary>
  /// Indicates the type of a metric and determine how it will be measured.
  /// </summary>
  public enum MetricType
  {
    /// <summary>
    /// A gauge is for numeric values that can be sampled without modification.
    /// </summary>
    /// <remarks>
    /// Current temperature, number of open connections and disk usage are
    /// examples of metrics that should be gauges.
    /// </remarks>
    Gauge = 1,

    /// <summary>
    /// A counter is for numeric values that get incremented when some event
    /// occurs.
    /// </summary>
    /// <remarks>
    /// Counters could be sampled and converted into a rate of change
    /// per time unit.
    /// </remarks>
    Counter = 2,

    /// <summary>
    /// A rate if for numeric value that represents a rate per time unit.
    /// </summary>
    Rate = 3
  }
}