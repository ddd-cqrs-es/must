using System;
using System.Collections.Generic;

namespace Nohros.Metrics.Reporting
{
  /// <summary>
  /// Method that is called to report metric's values for a collection of
  /// metrics.
  /// </summary>
  /// <param name="context">
  /// A user-defined object that qualifies or contains information about the
  /// reporting operation.
  /// </param>
  /// <param name="config">
  /// The id of the metric.
  /// </param>
  /// <param name="values">
  /// The values associated with a <see cref="IMetric"/>.
  /// </param>
  public delegate void MetricsReportCallback<in T>(MetricConfig config,
    IEnumerable<Measure> values, T context);
}
