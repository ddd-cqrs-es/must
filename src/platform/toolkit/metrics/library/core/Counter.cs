using System;
using Nohros.Concurrent;

namespace Nohros.Metrics
{
  /// <summary>
  /// A simple counter implementation of the <see cref="ICounter"/> class.
  /// </summary>
  /// <remarks>
  /// The value is the instantaneous value of the counter and it is never
  /// negative. If a <see cref="Decrement(long)"/> causes the counter
  /// do becomes negative its values will be set to zero.
  /// </remarks>
  public class Counter : AbstractMetric, ICounter
  {
    long count_;

    /// <summary>
    /// Initializes a new instance of the <see cref="Counter"/> class that
    /// uses the specified executor to perform the counter updates (
    /// increment/decrement).
    /// </summary>
    public Counter(MetricConfig config) : this(config, 0) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Counter"/> class that
    /// uses the specified executor to perform the counter updates (
    /// increment/decrement).
    /// </summary>
    public Counter(MetricConfig config, long initial)
      : this(config, new Mailbox<Action>(x => x()), initial) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Counter"/> class that
    /// uses the specified executor to perform the counter updates (
    /// increment/decrement).
    /// </summary>
    internal Counter(MetricConfig config, Mailbox<Action> mailbox, long initial)
      : base(config.WithAdditionalTag(MetricType.Counter.AsTag())) {
      count_ = initial;
      mailbox_ = mailbox;
    }


    /// <inheritdoc/>
    public void Increment() {
      Increment(1);
    }

    /// <inheritdoc/>
    public void Increment(long n) {
      mailbox_.Send(() => Update(n));
    }

    /// <inheritdoc/>
    public void Decrement() {
      Decrement(1);
    }

    /// <inheritdoc/>
    public void Decrement(long n) {
      mailbox_.Send(() => Update(-n));
    }

    /// <inheritdoc/>
    protected internal override Measure Compute(DateTime timestamp) {
      return CreateMeasure(count_, timestamp);
    }

    void Update(long delta) {
      count_ += delta;
      if (count_ < 0) {
        count_ = 0;
      }
    }
  }
}
