using System;
using Nohros.Concurrent;
using Nohros.Extensions.Time;

namespace Nohros.Metrics
{
  public class MeanRate : AbstractMetric, IMeter
  {
    readonly Clock clock_;
    readonly long ticks_per_unit_;
    readonly long start_time_;
    readonly Counter count_;

    /// <summary>
    /// Initializes a new instance of the <see cref=" MeanRate"/> class by
    /// using the specified config, rate unit and <see cref="StopwatchClock"/>
    /// as the clock.
    /// </summary>
    /// <param name="config">
    /// A <see cref="MetricConfig"/> containing the configuration settings
    /// for the metric.
    /// </param>
    /// <param name="rate_unit">
    /// The time unit of the meter's rate.
    /// </param>
    public MeanRate(MetricConfig config, TimeUnit rate_unit)
      : this(config, rate_unit, new Mailbox<Action>(runnable => runnable()),
        new StopwatchClock()) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref=" MeanRate"/> class by using
    /// the specified config and rate unit.
    /// </summary>
    /// <param name="config">
    /// A <see cref="MetricConfig"/> containing the configuration settings
    /// for the metric.
    /// </param>
    /// <param name="rate_unit">
    /// The time unit of the meter's rate.
    /// </param>
    /// <param name="clock">
    /// The clock that should be used to mark the passage of time.
    /// </param>
    public MeanRate(MetricConfig config, TimeUnit rate_unit, Clock clock)
      : this(config, rate_unit, new Mailbox<Action>(runnable => runnable()),
        clock) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref=" MeanRate"/> class by using
    /// the specified meter name, rate unit and clock.
    /// </summary>
    /// <param name="config">
    /// A <see cref="MetricConfig"/> containing the configuration settings
    /// for the metric.
    /// </param>
    /// <param name="unit">
    /// The time unit of the meter's rate.
    /// </param>
    /// <param name="clock">
    /// The clock that should be used to mark the passage of time.
    /// </param>
    internal MeanRate(MetricConfig config, TimeUnit unit,
      Mailbox<Action> mailbox, Clock clock) : base(config, mailbox) {
      clock_ = clock;
      start_time_ = clock.Tick;
      count_ = new Counter(config, mailbox, 0);
      ticks_per_unit_ = 1.ToTicks(unit);
    }

    /// <inheritdoc/>
    public void Mark() {
      Mark(1);
    }

    /// <inheritdoc/>
    public void Mark(long n) {
      count_.Increment(n);
    }

    /// <inheritdoc/>
    public override void GetMeasure(Action<Measure> callback) {
      long timestamp = clock_.Tick;
      mailbox_.Send(() => callback(Compute(timestamp)));
    }

    /// <inheritdoc/>
    public override void GetMeasure<T>(Action<Measure, T> callback, T context) {
      long timestamp = clock_.Tick;
      mailbox_.Send(() => callback(Compute(timestamp), context));
    }

    internal Measure Compute(long timestamp) {
      Measure count = count_.Compute();

      long elapsed = timestamp - start_time_;
      double rate = count.Value/elapsed;
      return CreateMeasure(rate*ticks_per_unit_);
    }

    /// <inheritdoc/>
    protected internal override Measure Compute() {
      throw new NotSupportedException();
    }
  }
}
