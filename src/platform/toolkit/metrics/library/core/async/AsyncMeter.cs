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
  public class AsyncMeter : IAsyncMeter, IMetered
  {
    const long kTickInterval = 5000000000; // 5 seconds in nanoseconds
    readonly Mailbox<RunnableDelegate> async_tasks_mailbox_;
    readonly Clock clock_;
    readonly string event_type_;
    readonly ExponentialWeightedMovingAverage ewma_15_rate_;
    readonly ExponentialWeightedMovingAverage ewma_1_rate_;
    readonly ExponentialWeightedMovingAverage ewma_5_rate_;
    readonly TimeUnit rate_unit_;
    readonly long start_time_;
    DateTime last_updated_;
    long count_;
    long last_tick_;

    /// <summary>
    /// Initializes a new instance of the <see cref=" Meter"/> class by using
    /// the specified meter name, rate unit and clock.
    /// </summary>
    /// <param name="rate_unit">
    /// The rate unit of the new meter.
    /// </param>
    /// <param name="event_type">
    /// The plural name of the event meter is measuring
    /// <example>
    /// <code>
    /// "requests"
    /// </code>
    /// </example>
    /// </param>
    public AsyncMeter(string event_type, TimeUnit rate_unit, IExecutor executor)
      : this(event_type, rate_unit, executor, new UserTimeClock()) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref=" Meter"/> class by using
    /// the specified meter name, rate unit and clock.
    /// </summary>
    /// <param name="rate_unit">
    /// The rate unit of the new meter.
    /// </param>
    /// <param name="event_type">
    /// The plural name of the event meter is measuring
    /// <example>
    /// <code>
    /// "requests"
    /// </code>
    /// </example>
    /// </param>
    public AsyncMeter(string event_type, TimeUnit rate_unit, IExecutor executor,
      Clock clock) {
      count_ = 0;
      rate_unit_ = rate_unit;
      event_type_ = event_type;
      clock_ = clock;
      start_time_ = clock_.Tick;
      last_tick_ = start_time_;
      ewma_1_rate_ = ExponentialWeightedMovingAverages.OneMinute();
      ewma_5_rate_ = ExponentialWeightedMovingAverages.FiveMinute();
      ewma_15_rate_ = ExponentialWeightedMovingAverages.FifteenMinute();
      async_tasks_mailbox_ = new Mailbox<RunnableDelegate>(Run);
    }

    /// <inheritdoc/>
    public TimeUnit RateUnit {
      get { return rate_unit_; }
    }

    /// <inheritdoc/>
    public string EventType {
      get { return event_type_; }
    }

    /// <inheritdoc/>
    public void GetFifteenMinuteRate(DoubleMetricCallback callback) {
      // We need to declare a local variable to hold the current value of the
      // clock tick, because the closure will capture the variable and not the
      // value of it.
      var now = DateTime.Now;
      long timestamp = clock_.Tick;
      async_tasks_mailbox_.Send(() => {
        TickIfNecessary(timestamp);
        callback(FifteenMinuteRate, now);
      });
    }

    /// <inheritdoc/>
    public void GetFiveMinuteRate(DoubleMetricCallback callback) {
      // We need to declare a local variable to hold the current value of the
      // clock tick, because the closure will capture the variable and not the
      // value of it.
      var now = DateTime.Now;
      long timestamp = clock_.Tick;
      async_tasks_mailbox_.Send(() => {
        TickIfNecessary(timestamp);
        callback(FiveMinuteRate, now);
      });
    }

    /// <inheritdoc/>
    public void GetOneMinuteRate(DoubleMetricCallback callback) {
      // We need to declare a local variables to hold the current value of the
      // clock tick and count, because the closure will capture the variable
      // and not the value of it.
      var now = DateTime.Now;
      long timestamp = clock_.Tick;
      async_tasks_mailbox_.Send(() => {
        TickIfNecessary(timestamp);
        callback(OneMinuteRate, now);
      });
    }

    /// <inheritdoc/>
    public void GetMeanRate(DoubleMetricCallback callback) {
      // We need to declare a local variables to hold the current value of the
      // clock tick and count, because the closure will capture the variable
      // and not the value of it.
      var now = DateTime.Now;
      long timestamp = clock_.Tick;
      async_tasks_mailbox_.Send(() => GetMeanRate(timestamp));
    }

    /// <inheritdoc/>
    public void GetCount(LongMetricCallback callback) {
      // We need to declare a local variables to hold the current value of the
      // clock tick and count, because the closure will capture the variable
      // and not the value of it.
      var now = DateTime.Now;
      async_tasks_mailbox_.Send(() => callback(Count, now));
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
      async_tasks_mailbox_.Send(() => Mark(n, timestamp));
    }

    /// <summary>
    /// Mark the occurrence of an event.
    /// </summary>
    public void Mark(MeteredCallback callback) {
      Mark(1, callback);
    }

    /// <summary>
    /// Mark the occurrence of a given number of events.
    /// </summary>
    /// <param name="n">
    /// The number of events.
    /// </param>
    /// <param name="callback">
    /// A <see cref="MeteredCallback"/> that will be executed before the
    /// mark operation completes.
    /// </param>
    public void Mark(long n, MeteredCallback callback) {
      long timestamp = clock_.Tick;
      async_tasks_mailbox_.Send(() => {
        Mark(n, timestamp);
        callback(this);
      });
    }

    void Mark(long n, long timestamp) {
      async_tasks_mailbox_.Send(() => {
        TickIfNecessary(timestamp);
        count_ += n;
        ewma_1_rate_.Update(n);
        ewma_5_rate_.Update(n);
        ewma_15_rate_.Update(n);
        last_updated_ = DateTime.Now;
      });
    }

    public void Report<T>(MetricReportCallback<T> callback, T context) {
      long timestamp = clock_.Tick;
      async_tasks_mailbox_.Send(() => callback(Report(timestamp), context));
    }

    double GetMeanRate(long timestamp) {
      if (count_ == 0) {
        return 0.0;
      }

      long elapsed = timestamp - start_time_;
      double rate = count_/(double) elapsed;
      return ConvertNsRate(rate);
    }

    void Run(RunnableDelegate runnable) {
      runnable();
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

    protected virtual MetricValueSet Report(long timestamp) {
      string rate_unit = UnitHelper.FromRate(EventType, RateUnit);
      var values = new[] {
        new MetricValue(MetricValueType.Count, count_, EventType),
        new MetricValue(MetricValueType.MeanRate, GetMeanRate(timestamp),
          rate_unit),
        new MetricValue(MetricValueType.OneMinuteRate, OneMinuteRate, rate_unit)
        ,
        new MetricValue(MetricValueType.FiveMinuteRate, FiveMinuteRate,
          rate_unit),
        new MetricValue(MetricValueType.FifteenMinuteRate, FifteenMinuteRate,
          rate_unit)
      };
      return new MetricValueSet(this, values);
    }

    double IMetered.MeanRate {
      get { return GetMeanRate(clock_.Tick); }
    }

    double IMetered.FifteenMinuteRate {
      get { return FifteenMinuteRate; }
    }

    double IMetered.FiveMinuteRate {
      get { return FiveMinuteRate; }
    }

    double IMetered.OneMinuteRate {
      get { return OneMinuteRate; }
    }

    long IMetered.Count {
      get { return count_; }
    }

    double FifteenMinuteRate {
      get { return ewma_15_rate_.Rate(rate_unit_); }
    }

    double FiveMinuteRate {
      get { return ewma_5_rate_.Rate(rate_unit_); }
    }

    double OneMinuteRate {
      get { return ewma_1_rate_.Rate(rate_unit_); }
    }

    long Count {
      get { return count_; }
    }

    /// <inheritdoc/>
    public DateTime LastUpdated {
      get { return last_updated_; }
    }
  }
}
