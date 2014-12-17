using System;

namespace Nohros.Metrics
{
  /// <summary>
  /// Wraps another <see cref="IMetric"/> object providing an alternative
  /// configuration.
  /// </summary>
  internal class MetricWrapper : IMetric
  {
    readonly MetricConfig config_;
    readonly IMetric metric_;

    /// <summary>
    /// Initializes a new instance of the <see cref="MetricWrapper"/> class
    /// by using the given <paramref name="config"/> and
    /// <paramref name="metric"/>.
    /// </summary>
    /// <param name="config">
    /// The alternate configuration.
    /// </param>
    /// <param name="metric">
    /// The metric to be wrapped.
    /// </param>
    public MetricWrapper(MetricConfig config, IMetric metric) {
      config_ = config;
      metric_ = metric;
    }

    /// <inheritdoc/>
    public void GetMeasure(Action<Measure> callback) {
      metric_.GetMeasure(callback);
    }

    /// <inheritdoc/>
    public void GetMeasure<T>(Action<Measure, T> callback, T state) {
      metric_.GetMeasure(callback, state);
    }

    /// <inheritdoc/>
    public MetricConfig Config {
      get { return config_; }
    }
  }
}
