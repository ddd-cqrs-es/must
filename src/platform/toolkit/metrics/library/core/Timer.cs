using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Nohros.Concurrent;
using Nohros.Extensions.Time;

namespace Nohros.Metrics
{
  /// <summary>
  /// A timer metric which aggregates timing durations and provides duration
  /// statistics, plus throughput statistics via <see cref="Meter"/>.
  /// </summary>
  public class Timer : AbstractMetric, ICompositeMetric, ITimer
  {
    public class Builder
    {
      public Builder(MetricConfig config) {
        Config = config;
        Mailbox = new Mailbox<Action>(x => x());
        TimeUnit = TimeUnit.Seconds;
        Clock = new StopwatchClock();
        Resevoir = new ExponentiallyDecayingResevoir();
      }

      public Builder WithResevoir(IResevoir resevoir) {
        Resevoir = resevoir;
        return this;
      }

      public Builder WithTimeUnit(TimeUnit unit) {
        TimeUnit = unit;
        return this;
      }

      public Builder WithSnapshotConfig(SnapshotConfig config) {
        SnapshotConfig = config;
        return this;
      }

      internal Builder WithClock(Clock clock) {
        Clock = clock;
        return this;
      }

      internal Builder WithMailbox(Mailbox<Action> mailbox) {
        Mailbox = mailbox;
        return this;
      }

      public MetricConfig Config { get; private set; }
      public IResevoir Resevoir { get; private set; }
      public TimeUnit TimeUnit { get; private set; }
      public SnapshotConfig SnapshotConfig { get; internal set; }

      internal Clock Clock { get; private set; }
      internal Mailbox<Action> Mailbox { get; private set; }
    }

    readonly Clock clock_;
    readonly TimeUnit unit_;
    readonly Histogram histogram_;
    readonly Meter meter_;
    readonly ReadOnlyCollection<IMetric> metrics_;

    Timer(Builder builder) : base(builder.Config, builder.Mailbox) {
      unit_ = builder.TimeUnit;
      clock_ = builder.Clock;

      MetricConfig unit_config = Config.WithAdditionalTag("unit", unit_.Name());

      meter_ = new Meter(unit_config, builder.Mailbox, builder.TimeUnit,
        builder.Clock);

      histogram_ = new Histogram(unit_config, builder.Mailbox,
        builder.SnapshotConfig, builder.Resevoir);

      metrics_ = new ReadOnlyCollection<IMetric>(
        new IMetric[] {
          meter_, histogram_
        });
    }

    /// <summary>
    /// Adds a recorded duration.
    /// </summary>
    /// <param name="duration">
    /// The length of the duration.
    /// </param>
    public void Update(TimeSpan duration) {
      long timestamp = clock_.Tick;
      mailbox_.Send(() => Update(duration, timestamp));
    }

    public T Time<T>(Func<T> method) {
      // The time should be mensured even if a exception is throwed.
      var timer = new Stopwatch();
      try {
        timer.Start();
        return method();
      } finally {
        timer.Stop();
        Update(timer.Elapsed);
      }
    }

    public void Time(Action method) {
      // The time should be mensured even if a exception is throwed.
      var timer = new Stopwatch();
      try {
        timer.Start();
        method();
      } finally {
        timer.Stop();
        Update(timer.Elapsed);
      }
    }

    /// <summary>
    /// Gets a timing <see cref="TimerContext"/>, which measures an elapsed
    /// time using the same duration as the parent <see cref="Timer"/>.
    /// </summary>
    /// <returns>
    /// A new <see cref="TimerContext"/>.
    /// </returns>
    public TimerContext Time() {
      return new TimerContext(this);
    }

    /// <inheritdoc/>
    public IEnumerator<IMetric> GetEnumerator() {
      return metrics_.GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }

    /// <inheritdoc/>
    public ICollection<IMetric> Metrics {
      get { return metrics_; }
    }

    /// <inheritdoc/>
    protected internal override Measure Compute() {
      return CreateMeasure(metrics_.Count);
    }

    void Update(TimeSpan duration, long timestamp) {
      if (duration > TimeSpan.Zero) {
        histogram_.Update(duration.ToUnit(unit_), timestamp);
        meter_.Mark(1, timestamp);
      }
    }
  }
}
