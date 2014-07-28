using System;
using Nohros.Concurrent;

namespace Nohros.Metrics
{
  public class AsyncUniformHistogram : AbstractAsyncHistogram
  {
    // This class is a way to to use the logic implemented by the
    // AbstractHistogram without exposing its synchronous methods to externals.
    class Histogram : AbstractHistogram
    {
      public override Snapshot Snapshot {
        get { throw new NotImplementedException(); }
      }
    }

    readonly Histogram histogram_ = new Histogram();
    readonly UniformSample sample_;
    DateTime last_updated_;

    public AsyncUniformHistogram(UniformSample sample, IExecutor executor)
      : this(sample, executor, new Histogram()) {
    }

    AsyncUniformHistogram(UniformSample sample, IExecutor executor,
      Histogram histogram)
      : base(histogram) {
      histogram_ = histogram;
      sample_ = sample;
      last_updated_ = DateTime.Now;
    }

    /// <inheritdoc/>
    public override void Update(long value) {
      async_tasks_mailbox_.Send(() => {
        histogram_.Update(value);
        sample_.Update(value);
        last_updated_ = DateTime.Now;
      });
    }

    /// <inheritdoc/>
    public override void GetSnapshot(SnapshotCallback callback) {
      var now = DateTime.Now;
      async_tasks_mailbox_.Send(() => callback(sample_.Snapshot, now));
    }

    public override DateTime LastUpdated {
      get { return last_updated_; }
    }
  }
}
