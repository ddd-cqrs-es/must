using System;

namespace Nohros.Metrics
{
  /// <summary>
  /// Represents the instantaneous value of a metric.
  /// </summary>
  public class Measure
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="Measure"/> by using
    /// the given measured value.
    /// </summary>
    /// <param name="value">
    /// The measured value.
    /// </param>
    /// <param name="config">
    /// The <see cref="MetricConfig"/> object that has produced the measure.
    /// </param>
    public Measure(double value, MetricConfig config) {
      MetricConfig = config;
      Value = value;
    }

    /// <summary>
    /// Gets the <see cref="MetricConfig"/> that is associated with the object
    /// that has produced the measure.
    /// </summary>
    public MetricConfig MetricConfig { get; private set; }

    /// <summary>
    /// Gets a collection of <see cref="Tag"/> object that is associated with
    /// the measure.
    /// </summary>
    /// <remarks>
    /// The <see cref="Tags"/> property contain only the metrics that is
    /// directly related with the measure. If you want to get the tags that is
    /// associated with the source <see cref="IMetric"/> object use the
    /// <see cref="MetricConfig"/> property to obtain them.
    /// </remarks>
    //public Tags Tags { get; private set; }

    /// <summary>
    /// Gets the instantaneous metric's value
    /// </summary>
    public double Value { get; private set; }
  }
}
