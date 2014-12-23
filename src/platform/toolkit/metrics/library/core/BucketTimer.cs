using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Nohros.Extensions;
using Nohros.Extensions.Time;
using System.Linq;

namespace Nohros.Metrics
{
  /// <summary>
  /// A simple <see cref="ITimer"/> implementation providing the total time,
  /// count, min and max for the times that have been recorded.
  /// </summary>
  public class BucketTimer : AbstractMetric, ICompositeMetric, ITimer
  {
    /// <summary>
    /// Configuration options for a <see cref="BucketTimer"/>.
    /// </summary>
    public class Builder
    {
      public Builder(MetricConfig config) {
        Config = config;
        TimeUnit = TimeUnit.Seconds;
        Context = MetricContext.ForCurrentProcess;
        Buckets = new long[0];
      }

      /// <summary>
      /// Sets the time unit for the buckets.
      /// </summary>
      /// <param name="unit">
      /// The time unit to be reported.
      /// </param>
      public Builder WithTimeUnit(TimeUnit unit) {
        TimeUnit = unit;
        return this;
      }

      public Builder WithContext(MetricContext context) {
        Context = context;
        return this;
      }

      /// <summary>
      /// Sets the buckets to be used.
      /// </summary>
      /// <param name="buckets">
      /// The time buckets to be used.
      /// </param>
      /// <remarks>
      /// <list type="list">
      /// <item>
      /// Each bucket must be unique
      /// </item>
      /// <item>
      /// Buckets must be in ascending order (smallest-to-largest).
      /// </item>
      /// <item>
      /// All bucket count will be namespaced under the two "metrics.bucket"
      /// tag.
      /// </item>
      /// <item>
      /// Buckets are incremented in the following way: Given a set of
      /// ordered buckets, let n1 = the first bucket. If a given duration is
      /// less than or equal to n1, the counter for n1 is incremented; else
      /// perform the same check on n2, n3, etc. If the duration is greater
      /// than the largest bucket, it is added to the 'overflow' bucket. The
      /// overflow bucket is automatically created.
      /// </item>
      /// <item>
      /// Each bucket must be unique
      /// </item>
      /// </list>
      /// </remarks>
      public Builder WithBuckets(long[] buckets) {
        if (buckets == null) {
          throw new ArgumentNullException("buckets");
        }

        if (buckets.Length == 0) {
          throw new ArgumentException("Buckets cannot be empty");
        }

        Buckets = new long[buckets.Length];
        long last = buckets[0];
        for (int i = 1; i < buckets.Length; i++) {
          if (buckets[i] <= last) {
            throw new ArgumentException(
              "Buckets must be in ascending order and cannot have duplicates");
          }
          last = Buckets[i] = buckets[i];
        }
        return this;
      }

      public BucketTimer Build() {
        return new BucketTimer(this);
      }

      internal MetricConfig Config { get; private set; }
      internal TimeUnit TimeUnit { get; private set; }
      internal long[] Buckets { get; private set; }

      internal MetricContext Context { get; private set; }
    }

    class BucketCounter : Counter
    {
      public BucketCounter(MetricConfig config) : base(config) {
      }

      public BucketCounter(MetricConfig config, MetricContext context)
        : base(config, context) {
      }

      public override void GetMeasure(Action<Measure> callback) {
        base.GetMeasure(m => {
          Measure measure =
            (m.Value > 0)
              ? m
              : new Measure(m.MetricConfig, m.Value, false);
          callback(measure);
        });
      }

      public override void GetMeasure<T>(Action<Measure, T> callback, T state) {
        base.GetMeasure((m, s) => {
          Measure measure =
            (m.Value > 0)
              ? m
              : new Measure(m.MetricConfig, m.Value, false);
          callback(measure, s);
        }, state);
      }
    }

    const string kBucket = "metrics.bucket";
    const string kStatistic = "statistic";
    const string kTotal = "total";
    const string kCount = "count";
    const string kMin = "min";
    const string kMax = "max";

    readonly TimeUnit unit_;
    readonly BucketCounter count_;
    readonly BucketCounter total_time_;
    readonly BucketCounter overflow_count_;
    readonly BucketCounter[] bucket_count_;
    readonly long[] buckets_;
    readonly ResettableMaxGauge max_;
    readonly ResettableMinGauge min_;
    readonly ReadOnlyCollection<IMetric> metrics_;

    /// <summary>
    /// Initializes a new instance of the <see cref="Timer"/> class by using
    /// the given configuration.
    /// </summary>
    /// <param name="builder">
    /// </param>
    public BucketTimer(Builder builder) : base(builder.Config, builder.Context) {
      unit_ = builder.TimeUnit;

      MetricContext context = builder.Context;

      MetricConfig config =
        builder
          .Config
          .WithAdditionalTag("unit", unit_.Name());

      count_ = new BucketCounter(config.WithAdditionalTag(kStatistic, kCount),
        context);
      max_ = new ResettableMaxGauge(config.WithAdditionalTag(kStatistic, kMax));
      min_ = new ResettableMinGauge(config.WithAdditionalTag(kStatistic, kMin));
      total_time_ =
        new BucketCounter(config.WithAdditionalTag(kStatistic, kTotal), context);
      overflow_count_ =
        new BucketCounter(
          config
            .WithAdditionalTag(kStatistic, kCount)
            .WithAdditionalTag(kBucket, "bucket=overflow"));

      buckets_ = builder.Buckets;

      // Compute the size of the maximum bucket name and create a padding
      // format to allow lexicographically sort of buckets.
      int num_digits = buckets_[buckets_.Length - 1].ToString().Length;
      string padding = "".PadLeft(num_digits, '0');

      string label = unit_.Abbreviation();

      bucket_count_ = new BucketCounter[buckets_.Length];
      for (int i = 0; i < buckets_.Length; i++) {
        bucket_count_[i] = new BucketCounter(
          config
            .WithAdditionalTag(kStatistic, kCount)
            .WithAdditionalTag(kBucket,
              "bucket={0}{1}".Fmt(buckets_[i].ToString(padding), label)));
      }

      var metrics = new List<IMetric> {
        new MeasureTransformer(total_time_, ConvertToUnit),
        new MeasureTransformer(min_, ConvertToUnit),
        new MeasureTransformer(max_, ConvertToUnit),
        new MeasureTransformer(overflow_count_, ConvertToUnit),
        count_
      };
      metrics.AddRange(bucket_count_);

      metrics_ = new ReadOnlyCollection<IMetric>(metrics);
    }

    /// <summary>
    /// Creates a new timer by using the specified name and the default
    /// values for resevoir, time unit and snapshot config.
    /// </summary>
    /// <param name="name">
    /// The name of the timer
    /// </param>
    /// <param name="buckets">
    /// The buckets to be used by the timer.
    /// </param>
    /// <returns>
    /// A <see cref="BucketTimer"/> whose name is <paramref name="name"/> and
    /// uses the default resevoir, time unit and snapshot config.
    /// </returns>
    public static BucketTimer Create(string name, long[] buckets) {
      return new Builder(new MetricConfig(name))
        .WithBuckets(buckets)
        .Build();
    }

    /// <summary>
    /// Creates a new timer by using the specified name and time unit and the
    /// default values for resevoir and snapshot config.
    /// </summary>
    /// <param name="name">
    /// The name of the timer
    /// </param>
    /// <param name="unit">
    /// </param>
    /// <param name="buckets">
    /// The buckets to be used by the timer.
    /// </param>
    /// <returns>
    /// A <see cref="BucketTimer"/> whose name is <paramref name="name"/> and
    /// uses the specified time unit and default resevoir and snapshot config.
    /// </returns>
    public static BucketTimer Create(string name, long[] buckets, TimeUnit unit) {
      return new
        Builder(new MetricConfig(name))
        .WithTimeUnit(unit)
        .WithBuckets(buckets)
        .Build();
    }

    /// <summary>
    /// Adds a recorded duration.
    /// </summary>
    /// <param name="duration">
    /// The length of the duration.
    /// </param>
    public void Update(TimeSpan duration) {
      if (duration > TimeSpan.Zero) {
        context_.Send(() => {
          long ticks = duration.Ticks;
          total_time_.Increment(ticks);
          min_.Update(ticks);
          max_.Update(ticks);
          count_.Increment();

          var measure = (long) ConvertToUnit(ticks);
          for (int i = 0; i < buckets_.Length; i++) {
            if (measure <= buckets_[i]) {
              bucket_count_[i].Increment();
              return;
            }
          }
          overflow_count_.Increment();
        });
      }
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
    /// time using the same duration as the parent <see cref="StatsTimer"/>.
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

    double ConvertToUnit(double d) {
      return TimeUnit.Ticks.Convert(d, unit_);
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
  }
}
