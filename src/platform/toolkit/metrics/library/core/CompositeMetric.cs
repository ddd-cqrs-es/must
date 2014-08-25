using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Nohros.Metrics
{
  /// <summary>
  /// A simple composite metric type with a static list of sub-metrics.
  /// </summary>
  /// <remarks>
  /// The value for the composite is the numner of sub-metrics.
  /// </remarks>
  public class CompositeMetric : AbstractMetric, ICompositeMetric
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="AbstractMetric"/> class
    /// by using the given <paramref name="config"/> object and list of
    /// sub-metrics.
    /// </summary>
    /// <param name="config">
    /// A <see cref="MetricConfig"/> containing the configuration settings
    /// for the metric.
    /// <para>
    /// It is recommended that the configuration shares common tags with the
    /// sub-metrics, but it is not enforced.
    /// </para>
    /// </param>
    /// <param name="metrics">
    /// The list of sub-metrics that composes this composite metric.
    /// </param>
    public CompositeMetric(MetricConfig config, IEnumerable<IMetric> metrics)
      : base(config) {
      Metrics = new ReadOnlyCollection<IMetric>(metrics.ToArray());
    }

    /// <inheritdoc/>
    protected internal override Measure Compute(DateTime timestamp) {
      return CreateMeasure(Metrics.Count, timestamp);
    }

    /// <inheritdoc/>
    public ICollection<IMetric> Metrics { get; private set; }
  }
}
