using System;
using Nohros.Concurrent;

namespace Nohros.Metrics
{
  /// <summary>
  /// A meter metric which measures mean throughput and one-, five-, and
  /// fifteen-minute exponetially-weighted moving average throughputs.
  /// </summary>
  /// <remarks>
  /// <para>
  ///   http://en.wikipedia.org/wiki/Moving_average#Exponential_moving_average
  /// </para>
  /// </remarks>
  public class Meter : IMeter, IMetered
  {
    const long kTickInterval = 5000000000; // 5 seconds in nanoseconds
    readonly Mailbox<Action> mailbox_;
    readonly Clock clock_;
    readonly ExponentialWeightedMovingAverage ewma_15_rate_;
    readonly ExponentialWeightedMovingAverage ewma_1_rate_;
    readonly ExponentialWeightedMovingAverage ewma_5_rate_;
    readonly TimeUnit rate_unit_;
    readonly long start_time_;
    long count_;
    long last_tick_;

    /// <summary>
    /// Initializes a new instance of the <see cref=" Meter"/> class by using
    /// the specified meter name, rate unit and clock.
    /// </summary>
    /// <param name="rate_unit">
    /// The rate unit of the new meter.
    /// </param>
    public Meter(TimeUnit rate_unit)
      : this(rate_unit, new UserTimeClock()) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref=" Meter"/> class by using
    /// the specified meter name, rate unit and clock.
    /// </summary>
    /// <param name="rate_unit">
    /// The rate unit of the new meter.
    /// </param>
    public Meter(TimeUnit rate_unit, Clock clock)
      : this(rate_unit, clock, new Mailbox<Action>(runnable => runnable())) {
    }

    public Meter(TimeUnit rate_unit, Clock clock, Mailbox<Action> mailbox) {
      count_ = 0;
      rate_unit_ = rate_unit;
      clock_ = clock;
      start_time_ = clock_.Tick;
      last_tick_ = start_time_;
      ewma_1_rate_ = ExponentialWeightedMovingAverages.OneMinute();
      ewma_5_rate_ = ExponentialWeightedMovingAverages.FiveMinute();
      ewma_15_rate_ = ExponentialWeightedMovingAverages.FifteenMinute();
      mailbox_ = mailbox;
    }

    /// <inheritdoc/>
    public TimeUnit RateUnit {
      get { return rate_unit_; }
    }

    /// <inheritdoc/>
    public void GetFifteenMinuteRate(DoubleMetricCallback callback) {
      // We need to declare a local variable to hold the current value of the
      // clock tick, because the closure will capture the variable and not the
      // value of it.
      var now = DateTime.Now;
      long timestamp = clock_.Tick;
      mailbox_.Send(() => {
        TickIfNecessary(timestamp);
        callback(ewma_15_rate_.Rate(rate_unit_), now);
      });
    }

    /// <inheritdoc/>
    public void GetFiveMinuteRate(DoubleMetricCallback callback) {
      // We need to declare a local variable to hold the current value of the
      // clock tick, because the closure will capture the variable and not the
      // value of it.
      var now = DateTime.Now;
      long timestamp = clock_.Tick;
      mailbox_.Send(() => {
        TickIfNecessary(timestamp);
        callback(ewma_5_rate_.Rate(rate_unit_), now);
      });
    }

    /// <inheritdoc/>
    public void GetOneMinuteRate(DoubleMetricCallback callback) {
      // We need to declare a local variables to hold the current value of the
      // clock tick and count, because the closure will capture the variable
      // and not the value of it.
      var now = DateTime.Now;
      long timestamp = clock_.Tick;
      mailbox_.Send(() => {
        TickIfNecessary(timestamp);
        callback(ewma_1_rate_.Rate(rate_unit_), now);
      });
    }

    /// <inheritdoc/>
    public void GetMeanRate(DoubleMetricCallback callback) {
      // We need to declare a local variables to hold the current value of the
      // clock tick and count, because the closure will capture the variable
      // and not the value of it.
      long timestamp = clock_.Tick;
      mailbox_.Send(() => GetMeanRate(timestamp));
    }

    /// <inheritdoc/>
    public void GetCount(LongMetricCallback callback) {
      // We need to declare a local variables to hold the current value of the
      // clock tick and count, because the closure will capture the variable
      // and not the value of it.
      var now = DateTime.Now;
      mailbox_.Send(() => callback(count_, now));
    }

    /// <summary>
    /// Mark the occurrence of an event.
    /// </summary>
    public void Mark() {
      Mark(1);
    }

    /// <summary>
    /// Mark the occurrence of a given number of events.
    /// </summary>
    /// <param name="n">
    /// The number of events.
    /// </param>
    public void Mark(long n) {
      long timestamp = clock_.Tick;
      mailbox_.Send(() => Mark(n, timestamp));
    }

    void Mark(long n, long timestamp) {
      mailbox_.Send(() => {
        TickIfNecessary(timestamp);
        count_ += n;
        ewma_1_rate_.Update(n);
        ewma_5_rate_.Update(n);
        ewma_15_rate_.Update(n);
      });
    }

    double GetMeanRate(long timestamp) {
      if (count_ == 0) {
        return 0.0;
      }

      long elapsed = timestamp - start_time_;
      double rate = count_/(double) elapsed;
      return ConvertNsRate(rate);
    }

    /// <summary>
    /// Updates the moving average.
    /// </summary>
    void Tick() {
      ewma_1_rate_.Tick();
      ewma_5_rate_.Tick();
      ewma_15_rate_.Tick();
    }

    void TickIfNecessary(long now) {
      long age = now - last_tick_;
      last_tick_ = now;
      if (age > kTickInterval) {
        long required_ticks = age/kTickInterval;
        for (long i = 0; i < required_ticks; i++) {
          Tick();
        }
      }
    }

    double ConvertNsRate(double rate_per_ns) {
      return rate_per_ns*TimeUnitHelper.ToNanos(1, rate_unit_);
    }

    /// <inheritdoc/>
    public void Report<T>(MetricReportCallback<T> callback, T context) {
      long timestamp = clock_.Tick;
      mailbox_.Send(
        () => callback(new MetricValueSet(this, Report(timestamp)), context));
    }

    /// <summary>
    /// Provide unsafe access to the internal counter. This should be called
    /// using the same context that is used to update the histogram.
    /// </summary>
    internal MetricValue[] Report(long timestamp) {
      return new[] {
        new MetricValue(MetricValueType.Count, count_),
        new MetricValue(MetricValueType.FifteenMinuteRate,
          ewma_15_rate_.Rate(rate_unit_)),
        new MetricValue(MetricValueType.FiveMinuteRate,
          ewma_5_rate_.Rate(rate_unit_)),
        new MetricValue(MetricValueType.MeanRate, GetMeanRate(timestamp)),
        new MetricValue(MetricValueType.OneMinuteRate,
          ewma_1_rate_.Rate(rate_unit_))
      };
    }
  }
}
