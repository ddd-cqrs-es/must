using System;
using Nohros.Concurrent;

namespace Nohros.Metrics
{
  /// <summary>
  /// A timer metric which aggregates timing durations and provides duration
  /// statistics, plus throughput statistics via <see cref="Meter"/>.
  /// </summary>
  public class AsyncTimer : IAsyncTimed
  {
    readonly Mailbox<RunnableDelegate> async_tasks_mailbox_;
    readonly Clock clock_;
    readonly TimeUnit duration_unit_;
    readonly BiasedHistogram histogram_;
    readonly Meter meter_;

    /// <summary>
    /// Creates a new <see cref="Timer"/>.
    /// </summary>
    /// <param name="duration_unit">
    /// The scale unit for this timer's duration metrics.
    /// </param>
    public AsyncTimer(TimeUnit duration_unit, Meter meter,
      BiasedHistogram histogram)
      : this(duration_unit, meter, histogram, new UserTimeClock()) {
    }

    public AsyncTimer(TimeUnit duration_unit, Meter meter,
      BiasedHistogram histogram, Clock clock) {
      duration_unit_ = duration_unit;
      meter_ = meter;
      histogram_ = histogram;
      async_tasks_mailbox_ = new Mailbox<RunnableDelegate>(Run);
      clock_ = clock;
    }

    public void GetFifteenMinuteRate(DoubleMetricCallback callback) {
      var now = DateTime.Now;
      async_tasks_mailbox_.Send(() => callback(meter_.FifteenMinuteRate, now));
    }

    public void GetFiveMinuteRate(DoubleMetricCallback callback) {
      var now = DateTime.Now;
      async_tasks_mailbox_.Send(() => callback(meter_.FiveMinuteRate, now));
    }

    public void GetMeanRate(DoubleMetricCallback callback) {
      var now = DateTime.Now;
      var timestamp = clock_.Tick;
      async_tasks_mailbox_.Send(
        () => callback(meter_.GetMeanRate(timestamp), now));
    }

    public void GetOneMinuteRate(DoubleMetricCallback callback) {
      var now = DateTime.Now;
      async_tasks_mailbox_.Send(() => callback(meter_.OneMinuteRate, now));
    }

    public TimeUnit RateUnit {
      get { return meter_.RateUnit; }
    }

    /// <inheritdoc/>
    public string EventType {
      get { return meter_.EventType; }
    }

    public void GetCount(LongMetricCallback callback) {
      var now = DateTime.Now;
      async_tasks_mailbox_.Send(() => callback(meter_.Count, now));
    }

    /// <inheritdoc/>
    public void GetSnapshot(SnapshotCallback callback) {
      var now = DateTime.Now;
      async_tasks_mailbox_.Send(() => {
        double[] values = histogram_.Snapshot.Values;
        var converted = new double[values.Length];
        for (int i = 0, j = values.Length; i < j; i++) {
          converted[i] = ConvertFromNs(values[i]);
        }
        callback(new Snapshot(converted), now);
      });
    }

    public void GetMax(DoubleMetricCallback callback) {
      var now = DateTime.Now;
      async_tasks_mailbox_.Send(
        () => callback(ConvertFromNs(histogram_.Max), now));
    }

    public void GetMean(DoubleMetricCallback callback) {
      var now = DateTime.Now;
      async_tasks_mailbox_.Send(
        () => callback(ConvertFromNs(histogram_.Mean), now));
    }

    public void GetMin(DoubleMetricCallback callback) {
      var now = DateTime.Now;
      async_tasks_mailbox_.Send(
        () => callback(ConvertFromNs(histogram_.Min), now));
    }

    public void GetStandardDeviation(DoubleMetricCallback callback) {
      var now = DateTime.Now;
      async_tasks_mailbox_.Send(
        () => callback(ConvertFromNs(histogram_.StandardDeviation), now));
    }

    /// <summary>
    /// Adds a recorded duration.
    /// </summary>
    /// <param name="duration">
    /// The length of the duration.
    /// </param>
    /// <param name="unit">
    /// The scale unit of <paramref name="duration"/>
    /// </param>
    public void Update(long duration, TimeUnit unit) {
      Update(TimeUnitHelper.ToNanos(duration, unit));
    }

    public void Report<T>(MetricReportCallback<T> callback, T context) {
      long timestamp = clock_.Tick;
      async_tasks_mailbox_.Send(() => callback(Report(timestamp), context));
    }

    public T Time<T>(CallableDelegate<T> method) {
      long start_time = clock_.Tick;

      // The time should be mensured even if a exception is throwed.
      try {
        return method();
      } finally {
        Update(clock_.Tick - start_time);
      }
    }

    public void Time(RunnableDelegate runnable) {
      long start_time = clock_.Tick;

      // The time should be mensured even if a exception is throwed.
      try {
        runnable();
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

    protected virtual MetricValueSet Report(long timestamp) {
      Snapshot snapshot = histogram_.Snapshot;
      string duration_unit = UnitHelper.FromTimeUnit(duration_unit_);
      string rate_unit = UnitHelper.FromRate(EventType, RateUnit);
      var values = new[] {
        new MetricValue(MetricValueType.Min, ConvertFromNs(histogram_.Min),
          duration_unit),
        new MetricValue(MetricValueType.Max, ConvertFromNs(histogram_.Max),
          duration_unit),
        new MetricValue(MetricValueType.Mean, ConvertFromNs(histogram_.Mean),
          duration_unit),
        new MetricValue(MetricValueType.StandardDeviation,
          ConvertFromNs(histogram_.StandardDeviation), duration_unit),
        new MetricValue(MetricValueType.Median, ConvertFromNs(snapshot.Median),
          duration_unit),
        new MetricValue(MetricValueType.Percentile75,
          ConvertFromNs(snapshot.Percentile75), duration_unit),
        new MetricValue(MetricValueType.Percentile95,
          ConvertFromNs(snapshot.Percentile95), duration_unit),
        new MetricValue(MetricValueType.Percentile98,
          ConvertFromNs(snapshot.Percentile98), duration_unit),
        new MetricValue(MetricValueType.Percentile99,
          ConvertFromNs(snapshot.Percentile99), duration_unit),
        new MetricValue(MetricValueType.Percentile999,
          ConvertFromNs(snapshot.Percentile999), duration_unit),
        new MetricValue(MetricValueType.Count, meter_.Count, EventType),
        new MetricValue(MetricValueType.MeanRate, meter_.GetMeanRate(timestamp),
          rate_unit),
        new MetricValue(MetricValueType.OneMinuteRate, meter_.OneMinuteRate,
          rate_unit),
        new MetricValue(MetricValueType.FiveMinuteRate, meter_.FiveMinuteRate,
          rate_unit),
        new MetricValue(MetricValueType.FifteenMinuteRate,
          meter_.FifteenMinuteRate, rate_unit)
      };
      return new MetricValueSet(this, values);
    }

    void Run(RunnableDelegate runnable) {
      runnable();
    }

    /// <summary>
    /// Adds a recorded duration.
    /// </summary>
    /// <param name="duration">
    /// The length of the duration.
    /// </param>
    public void Update(long duration) {
      if (duration >= 0) {
        long timestamp = TimeUnitHelper
          .ToSeconds(clock_.Time, TimeUnit.Milliseconds);
        async_tasks_mailbox_.Send(
          () => histogram_.Update(duration, timestamp));
        meter_.Mark();
      }
    }

    double ConvertFromNs(double ns) {
      return ns/TimeUnitHelper.ToNanos(1, duration_unit_);
    }

    /// <summary>
    /// Gets the timer's duration scale unit.
    /// </summary>
    public TimeUnit DurationUnit {
      get { return duration_unit_; }
    }
  }
}
