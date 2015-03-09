using System;
using Nohros.Concurrent;

namespace Nohros.Metrics
{
  /// <summary>
  /// An abstract implementation of the <see cref="IMetric"/> interface.
  /// </summary>
  public abstract class AbstractMetric : IMetric
  {
    protected internal MetricContext context_;

    /// <summary>
    /// Initializes a new instance of the <see cref="AbstractMetric"/> class
    /// by using the given <paramref name="config"/> object.
    /// </summary>
    /// <param name="config">
    /// A <see cref="MetricConfig"/> containing the configuration settings
    /// for the metric.
    /// </param>
    protected AbstractMetric(MetricConfig config)
      : this(config, MetricContext.ForCurrentProcess) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AbstractMetric"/> class
    /// by using the given <paramref name="config"/> object.
    /// </summary>
    /// <param name="config">
    /// A <see cref="MetricConfig"/> containing the configuration settings
    /// for the metric.
    /// </param>
    /// <param name="context">
    /// A <see cref="MetricContext"/> that contains the shared
    /// <see cref="Mailbox{T}"/> and <see cref="Clock"/>.
    /// </param>
    protected AbstractMetric(MetricConfig config, MetricContext context) {
      if (config == null || context == null) {
        throw new ArgumentNullException(config == null ? "config" : "context");
      }
      Config = config;
      context_ = context;
    }

    /// <inheritdoc/>
    public virtual void GetMeasure(Action<Measure> callback) {
      context_.Send(() => callback(Compute()));
    }

    /// <inheritdoc/>
    public virtual void GetMeasure<T>(Action<Measure, T> callback, T state) {
      context_.Send(() => callback(Compute(), state));
    }

    /// <summary>
    /// Creates a <see cref="Measure"/> by using <see cref="Config"/> and the
    /// given metric's value.
    /// </summary>
    /// <returns>
    /// A <see cref="Measure"/> containg the current metric's value.
    /// </returns>
    protected virtual Measure CreateMeasure(double measure) {
      return new Measure(Config, measure);
    }

    /// <summary>
    /// Computes the current value of a metric, synchrosnouly.
    /// </summary>
    /// <returns>
    /// A <see cref="Measure"/> containg the current metric's value.
    /// </returns>
    protected internal abstract Measure Compute();

    /// <inheritdoc/>
    public MetricConfig Config { get; private set; }
  }
}
