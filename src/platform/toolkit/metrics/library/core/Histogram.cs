using System;
using Nohros.Concurrent;

namespace Nohros.Metrics
{
  public class Histogram : IHistogram
  {
    readonly IResevoir resevoir_;
    readonly Mailbox<Action> mailbox_;
    int count_;

    public Histogram(IResevoir resevoir)
      : this(resevoir, new Mailbox<Action>(runnable => runnable())) {
    }

    internal Histogram(IResevoir resevoir, Mailbox<Action> mailbox) {
      resevoir_ = resevoir;
      mailbox_ = mailbox;
      count_ = 0;
    }

    /// <inheritdoc/>
    public void GetSnapshot(SnapshotCallback callback) {
      var now = DateTime.Now;
      mailbox_.Send(() => callback(resevoir_.Snapshot, now));
    }

    /// <inheritdoc/>
    public void GetCount(LongMetricCallback callback) {
      var now = DateTime.Now;
      mailbox_.Send(() => callback(count_, now));
    }

    /// <inheritdoc/>
    public void Update(long value) {
      count_++;
      resevoir_.Update(value);
    }

    /// <summary>
    /// Provide unsafe access to the internal counter. This should be called
    /// using the same context that is used to update the histogram.
    /// </summary>
    internal long InternalCount {
      get { return count_; }
    }

    /// <inheritdoc/>
    public void Report<T>(MetricReportCallback<T> callback, T context) {
      mailbox_.Send(() => callback(new MetricValueSet(this, Report()), context));
    }

    MetricValue[] Report() {
      var metrics = resevoir_.Snapshot.Report();
      metrics.Add(new MetricValue(MetricValueType.Count, count_));
      return metrics.ToArray();
    }
  }
}
