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
        TimeUnit = TimeUnit.Seconds;
        Context = MetricContext.ForCurrentProcess;
        Resevoir = new ExponentiallyDecayingResevoir();
        SnapshotConfig = new SnapshotConfig.Builder().Build();
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

      public Builder WithContext(MetricContext context) {
        Context = context;
        return this;
      }

      public Timer Build() {
        return new Timer(this);
      }

      public MetricConfig Config { get; private set; }
      public IResevoir Resevoir { get; private set; }
      public TimeUnit TimeUnit { get; private set; }
      public SnapshotConfig SnapshotConfig { get; internal set; }

      internal MetricContext Context { get; private set; }
    }

    readonly TimeUnit unit_;
    readonly Histogram histogram_;
    readonly Meter meter_;
    readonly ReadOnlyCollection<IMetric> metrics_;

    Timer(Builder builder) : base(builder.Config, builder.Context) {
      unit_ = builder.TimeUnit;

      MetricConfig unit_config = Config.WithAdditionalTag("unit", unit_.Name());

      meter_ = new Meter(unit_config, builder.TimeUnit, context_);

      histogram_ = new Histogram(unit_config, builder.SnapshotConfig,
        builder.Resevoir, context_);

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
      long timestamp = context_.Tick;
      context_.Send(() => Update(duration, timestamp));
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
