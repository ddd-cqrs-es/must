using System;
using Nohros.Concurrent;

namespace Nohros.Metrics
{
  /// <summary>
  /// An abstract implementation of the <see cref="IMetric"/> interface.
  /// </summary>
  public abstract class AbstractMetric : IMetric
  {
    protected Mailbox<Action> mailbox_;

    /// <summary>
    /// Initializes a new instance of the <see cref="AbstractMetric"/> class
    /// by using the given <paramref name="config"/> object.
    /// </summary>
    /// <param name="config">
    /// A <see cref="MetricConfig"/> containing the configuration settings
    /// for the metric.
    /// </param>
    protected AbstractMetric(MetricConfig config)
      : this(config, new Mailbox<Action>(runnable => runnable())) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AbstractMetric"/> class
    /// by using the given <paramref name="config"/> object.
    /// </summary>
    /// <param name="config">
    /// A <see cref="MetricConfig"/> containing the configuration settings
    /// for the metric.
    /// </param>
    /// <param name="mailbox">
    /// A <see cref="Mailbox{T}"/> that can be used to asynchrously process
    /// metrics operations.
    /// </param>
    protected AbstractMetric(MetricConfig config, Mailbox<Action> mailbox) {
      if (config == null || mailbox == null) {
        throw new ArgumentNullException(config == null ? "config" : "mailbox");
      }
      Config = config;
      mailbox_ = mailbox;
    }

    /// <inheritdoc/>
    public virtual void GetMeasure(Action<Measure> callback) {
      DateTime now = DateTime.Now;
      mailbox_.Send(() => callback(Compute(now)));
    }

    /// <inheritdoc/>
    public virtual void GetMeasure<T>(Action<Measure, T> callback, T context) {
      DateTime now = DateTime.Now;
      mailbox_.Send(() => callback(Compute(now), context));
    }

    /// <summary>
    /// Creates a <see cref="Measure"/> by using <see cref="Config"/> and the
    /// given metric's value.
    /// </summary>
    /// <returns>
    /// A <see cref="Measure"/> containg the current metric's value.
    /// </returns>
    protected virtual Measure CreateMeasure(double measure, DateTime timestamp) {
      return new Measure(Config, measure, timestamp);
    }

    /// <summary>
    /// Computes the current value of a metric, synchrosnouly.
    /// </summary>
    /// <returns>
    /// A <see cref="Measure"/> containg the current metric's value.
    /// </returns>
    /// <param name="timestamp">
    /// The date and time when the measure was requested. This value
    /// represents the date and time when the <see cref="GetMeasure"/> method
    /// was called and not the time when the measured was comptued. This
    /// allows the measure to be delayed computed without losing the timing
    /// properties.
    /// </param>
    protected internal abstract Measure Compute(DateTime timestamp);

    /// <inheritdoc/>
    public MetricConfig Config { get; set; }

#if DEBUG
    public void Run(Action action) {
      mailbox_.Send(action);
    }
#endif
  }
}
