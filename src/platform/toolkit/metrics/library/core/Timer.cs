using System;
using Nohros.Concurrent;

namespace Nohros.Metrics
{
  /// <summary>
  /// A timer metric which aggregates timing durations and provides duration
  /// statistics, plus throughput statistics via <see cref="Meter"/>.
  /// </summary>
  public class Timer : ITimed, ITimer
  {
    readonly IResevoir resevoir_;
    readonly Clock clock_;
    readonly TimeUnit duration_unit_;
    readonly Histogram histogram_;
    readonly Meter meter_;
    readonly Mailbox<Action> mailbox_;

    /// <summary>
    /// Initializes a new instance of the <see cref="Timer"/> class by using
    /// <see cref="ExponentiallyDecayingResevoir"/> and
    /// <see cref="UserTimeClock"/> as default resevoir and clock.
    /// </summary>
    public Timer() : this(TimeUnit.Seconds, new ExponentiallyDecayingResevoir()) {
    }

    /// <summary>
    /// Creates a new <see cref="Timer"/>.
    /// </summary>
    /// <param name="duration_unit">
    /// The scale unit for this timer's duration metrics.
    /// </param>
    /// <param name="resevoir">
    /// The <see cref="IResevoir"/> implementation the timer should use.
    /// </param>
    public Timer(TimeUnit duration_unit, IResevoir resevoir)
      : this(duration_unit, resevoir, new UserTimeClock()) {
      resevoir_ = resevoir;
    }

    /// <summary>
    /// Creates a new <see cref="Timer"/>.
    /// </summary>
    /// <param name="duration_unit">
    /// The scale unit for this timer's duration metrics.
    /// </param>
    /// <param name="resevoir">
    /// The <see cref="IResevoir"/> implementation the timer should use.
    /// </param>
    /// <param name="clock">
    /// The <see cref="Clock"/> implementation the timer should use.
    /// </param>
    public Timer(TimeUnit duration_unit, IResevoir resevoir, Clock clock) {
      duration_unit_ = duration_unit;
      clock_ = clock;
      mailbox_ = new Mailbox<Action>(runnable => runnable());
      meter_ = new Meter(duration_unit, clock, mailbox_);
      histogram_ = new Histogram(resevoir, mailbox_);
    }

    public void GetFifteenMinuteRate(DoubleMetricCallback callback) {
      meter_.GetFifteenMinuteRate(callback);
    }

    public void GetFiveMinuteRate(DoubleMetricCallback callback) {
      meter_.GetFiveMinuteRate(callback);
    }

    public void GetMeanRate(DoubleMetricCallback callback) {
      meter_.GetMeanRate(callback);
    }

    public void GetOneMinuteRate(DoubleMetricCallback callback) {
      meter_.GetOneMinuteRate(callback);
    }

    public void GetCount(LongMetricCallback callback) {
      meter_.GetCount(callback);
    }

    /// <inheritdoc/>
    public void GetSnapshot(SnapshotCallback callback) {
      histogram_.GetSnapshot(callback);
    }

    public TimeUnit RateUnit {
      get { return meter_.RateUnit; }
    }

    /// <summary>
    /// Adds a recorded duration.
    /// </summary>
    /// <param name="duration">The length of the duration.</param>
    /// <param name="unit">The scale unit of <paramref name="duration"/></param>
    public void Update(long duration, TimeUnit unit) {
      Update(TimeUnitHelper.ToNanos(duration, unit));
    }

    public T Time<T>(Func<T> method) {
      long start_time = clock_.Tick;

      // The time should be mensured even if a exception is throwed.
      try {
        return method();
      } finally {
        Update(clock_.Tick - start_time);
      }
    }

    public void Time(Action method) {
      long start_time = clock_.Tick;

      // The time should be mensured even if a exception is throwed.
      try {
        method();
      } finally {
        Update(clock_.Tick - start_time);
      }
    }

    /// <summary>
    /// Gets a timing <see cref="TimerContext"/>, which measures an elapsed
    /// time in nanoseconds.
    /// </summary>
    /// <returns>
    /// A new <see cref="TimerContext"/>.
    /// </returns>
    public TimerContext Time() {
      return new TimerContext(this, clock_);
    }

    /// <summary>
    /// Adds a recorded duration.
    /// </summary>
    /// <param name="duration">The length of the duration.</param>
    void Update(long duration) {
      if (duration >= 0) {
        histogram_.Update(duration);
        meter_.Mark();
      }
    }

    /// <summary>
    /// Gets the timer's duration scale unit.
    /// </summary>
    public TimeUnit DurationUnit {
      get { return duration_unit_; }
    }

    /// <inheritdoc/>
    public void Report<T>(MetricReportCallback<T> callback, T context) {
      long timestamp = clock_.Tick;
      mailbox_.Send(
        () => callback(new MetricValueSet(this, Report(timestamp)), context));
    }

    MetricValue[] Report(long timestamp) {
      var metrics = resevoir_.Snapshot.Report();
      metrics.Add(
        new MetricValue(MetricValueType.Count, histogram_.InternalCount));
      metrics.AddRange(meter_.Report(timestamp));
      return metrics.ToArray();
    }
  }
}
