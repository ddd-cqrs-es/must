using System;
using System.Collections;
using System.Collections.Generic;
using Nohros.Concurrent;

namespace Nohros.Metrics
{
  public class Histogram : AbstractMetric, IHistogram, ICompositeMetric
  {
    class CallableGaugeWrapper
    {
      readonly MetricConfig config_;
      readonly Func<Snapshot, double> callable_;

      public CallableGaugeWrapper(MetricConfig config,
        Func<Snapshot, double> callable) {
        config_ = config;
        callable_ = callable;
      }

      public IMetric Wrap(Snapshot snapshot) {
        return new CallableGauge(config_, () => callable_(snapshot));
      }
    }

    readonly SnapshotConfig stats_;
    readonly IResevoir resevoir_;
    readonly List<CallableGaugeWrapper> gauges_;


    /// <summary>
    /// Initializes a new instance of the <see cref="Histogram"/> by using the
    /// given <see cref="IResevoir"/>.
    /// </summary>
    /// <param name="config">
    /// A <see cref="MetricConfig"/> containing the configuration settings
    /// for the metric.
    /// </param>
    /// <param name="stats">
    /// A <see cref="SnapshotConfig"/> that defines the statistics that should
    /// be computed.
    /// </param>
    /// <param name="resevoir">
    /// A <see cref="IResevoir"/> that can be used to store the computed
    /// values.
    /// </param>
    public Histogram(MetricConfig config, SnapshotConfig stats,
      IResevoir resevoir)
      : this(config, new Mailbox<Action>(x => x()), stats, resevoir) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Histogram"/> by using the
    /// given <see cref="IResevoir"/>.
    /// </summary>
    /// <param name="config">
    /// A <see cref="MetricConfig"/> containing the configuration settings
    /// for the metric.
    /// </param>
    /// <param name="stats">
    /// A <see cref="SnapshotConfig"/> that defines the statistics that should
    /// be computed.
    /// </param>
    /// <param name="resevoir">
    /// A <see cref="IResevoir"/> that can be used to store the computed
    /// values.
    /// </param>
    /// <param name="context">
    /// A <see cref="MetricContext"/> that contains the shared
    /// <see cref="Mailbox{T}"/> and <see cref="Clock"/>.
    /// </param>
    public Histogram(MetricConfig config, SnapshotConfig stats,
      IResevoir resevoir, MetricContext context)
      : base(config, context) {
      stats_ = stats;
      resevoir_ = resevoir;

      gauges_ = new List<CallableGaugeWrapper>();

      if (stats.ComputeCount) {
        gauges_.Add(CountGauge(config));
      }

      if (stats.ComputeMax) {
        gauges_.Add(MaxGauge(config));
      }

      if (stats.ComputeMean) {
        gauges_.Add(MeanGauge(config));
      }

      if (stats.ComputeMedian) {
        gauges_.Add(MedianGauge(config));
      }

      if (stats.ComputeMin) {
        gauges_.Add(MinGauge(config));
      }

      if (stats.ComputeStdDev) {
        gauges_.Add(StdDevGauge(config));
      }

      foreach (var percentile in stats.Percentiles) {
        gauges_.Add(PercentileGauge(config, percentile));
      }
    }

    CallableGaugeWrapper MinGauge(MetricConfig config) {
      return
        new CallableGaugeWrapper(
          config.WithAdditionalTag("statistic", "min"),
          snapshot => snapshot.Min);
    }

    CallableGaugeWrapper MedianGauge(MetricConfig config) {
      return
        new CallableGaugeWrapper(
          config.WithAdditionalTag("statistic", "median"),
          snapshot => snapshot.Median);
    }

    CallableGaugeWrapper MeanGauge(MetricConfig config) {
      return
        new CallableGaugeWrapper(
          config.WithAdditionalTag("statistic", "mean"),
          snapshot => snapshot.Mean);
    }

    CallableGaugeWrapper MaxGauge(MetricConfig config) {
      return
        new CallableGaugeWrapper(
          config.WithAdditionalTag("statistic", "max"),
          snapshot => snapshot.Max);
    }

    CallableGaugeWrapper CountGauge(MetricConfig config) {
      return
        new CallableGaugeWrapper(
          config.WithAdditionalTag("statistic", "count"),
          snapshot => snapshot.Size);
    }

    CallableGaugeWrapper StdDevGauge(MetricConfig config) {
      return
        new CallableGaugeWrapper(
          config.WithAdditionalTag("statistic", "stddev"),
          snapshot => snapshot.StdDev);
    }

    CallableGaugeWrapper PercentileGauge(MetricConfig config, double percentile) {
      return
        new CallableGaugeWrapper(
          config.WithAdditionalTag("statistic",
            "percentile_" + percentile.ToString("#.####")),
          snapshot => snapshot.Quantile(percentile));
    }

    /// <inheritdoc/>
    public void Update(long value) {
      long timestamp = resevoir_.Timestamp;
      context_.Send(() => Update(value, timestamp));
    }

    public void Update(long value, long timestamp) {
      context_.Send(() => resevoir_.Update(value, timestamp));
    }

    /// <inheritdoc/>
    public ICollection<IMetric> Metrics {
      get {
        Snapshot snapshot = resevoir_.Snapshot;
        var metrics = new IMetric[gauges_.Count];
        for (int i = 0; i < gauges_.Count; i++) {
          metrics[i] = gauges_[i].Wrap(snapshot);
        }
        return metrics;
      }
    }

    IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }

    public IEnumerator<IMetric> GetEnumerator() {
      return Metrics.GetEnumerator();
    }

    /// <inheritdoc/>
    protected internal override Measure Compute() {
      return CreateMeasure(gauges_.Count);
    }
  }
}
