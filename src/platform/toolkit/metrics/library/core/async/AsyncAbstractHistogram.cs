using System;
using System.Collections.Generic;
using Nohros.Concurrent;

namespace Nohros.Metrics
{
  public abstract class AbstractAsyncHistogram : IAsyncHistogram
  {
    protected readonly Mailbox<RunnableDelegate> async_tasks_mailbox_;
    readonly ISyncHistogram histogram_;

    protected AbstractAsyncHistogram(ISyncHistogram histogram) {
      histogram_ = histogram;
      async_tasks_mailbox_ = new Mailbox<RunnableDelegate>(
        runnable => runnable());

      Tags = new Dictionary<string, string>();
    }

    /// <inheritdoc/>
    public abstract void GetSnapshot(SnapshotCallback callback);

    /// <inheritdoc/>
    public virtual void GetMax(DoubleMetricCallback callback) {
      var now = DateTime.Now;
      async_tasks_mailbox_.Send(() => callback(histogram_.Max, now));
    }

    /// <inheritdoc/>
    public virtual void GetMin(DoubleMetricCallback callback) {
      var now = DateTime.Now;
      async_tasks_mailbox_.Send(() => callback(histogram_.Min, now));
    }

    /// <inheritdoc/>
    public virtual void GetMean(DoubleMetricCallback callback) {
      var now = DateTime.Now;
      async_tasks_mailbox_.Send(() => callback(histogram_.Mean, now));
    }

    /// <inheritdoc/>
    public virtual void GetStandardDeviation(DoubleMetricCallback callback) {
      var now = DateTime.Now;
      async_tasks_mailbox_.Send(
        () => callback(histogram_.StandardDeviation, now));
    }

    /// <inheritdoc/>
    public virtual void GetCount(LongMetricCallback callback) {
      var now = DateTime.Now;
      async_tasks_mailbox_.Send(() => callback(histogram_.Count, now));
    }

    /// <inheritdoc/>
    public abstract void Update(long value);

    public virtual void Report<T>(MetricReportCallback<T> callback, T context) {
      async_tasks_mailbox_.Send(() => callback(Report(), context));
    }

    public abstract DateTime LastUpdated { get; }

    protected MetricValueSet Report() {
      Snapshot snapshot = histogram_.Snapshot;
      var values = new[] {
        new MetricValue(MetricValueType.Min, histogram_.Min),
        new MetricValue(MetricValueType.Max, histogram_.Max),
        new MetricValue(MetricValueType.Mean, histogram_.Mean),
        new MetricValue(MetricValueType.StandardDeviation,
          histogram_.StandardDeviation),
        new MetricValue(MetricValueType.Median, snapshot.Median),
        new MetricValue(MetricValueType.Percentile75, snapshot.Percentile75),
        new MetricValue(MetricValueType.Percentile95, snapshot.Percentile95),
        new MetricValue(MetricValueType.Percentile98, snapshot.Percentile98),
        new MetricValue(MetricValueType.Percentile99, snapshot.Percentile99),
        new MetricValue(MetricValueType.Percentile999, snapshot.Percentile999)
      };
      return new MetricValueSet(this, values);
    }

    public IDictionary<string, string> Tags { get; private set; }
  }
}
