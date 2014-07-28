using System;
using Nohros.Concurrent;

namespace Nohros.Metrics
{
  public class AsyncBiasedHistogram : AbstractAsyncHistogram
  {
    // This class is a way to to use the logic implemented by the
    // AbstractHistogram without exposing its synchronous methods to externals.
    class Histogram : AbstractHistogram
    {
      public override Snapshot Snapshot {
        get { throw new NotImplementedException(); }
      }
    }

    readonly Histogram histogram_;
    readonly ExponentiallyDecayingSample sample_;
    DateTime last_updated_;

    public AsyncBiasedHistogram(ExponentiallyDecayingSample sample)
      : this(sample, new Histogram()) {
    }

    AsyncBiasedHistogram(ExponentiallyDecayingSample sample, Histogram histogram)
      : base(histogram) {
      sample_ = sample;
      histogram_ = histogram;
      last_updated_ = DateTime.Now;
    }

    /// <inheritdoc/>
    public override void Update(long value) {
      long timestamp = TimeUnitHelper
        .ToSeconds(Clock.CurrentTimeMilis, TimeUnit.Milliseconds);
      async_tasks_mailbox_.Send(() => {
        histogram_.Update(value);
        sample_.Update(value, timestamp);
        last_updated_ = DateTime.Now;
      });
    }

    /// <inheritdoc/>
    public override void GetSnapshot(SnapshotCallback callback) {
      var now = DateTime.Now;
      async_tasks_mailbox_.Send(() => callback(sample_.Snapshot, now));
    }

    /// <inheritdoc/>
    public override DateTime LastUpdated {
      get { return last_updated_; }
    }
  }
}
