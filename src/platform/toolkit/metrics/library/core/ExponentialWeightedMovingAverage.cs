using System;
using Nohros.Concurrent;

namespace Nohros.Metrics
{
  /// <summary>
  /// An exponentially-weighted moving average.
  /// </summary>
  /// <remarks>
  /// <para>
  /// The value is the exponentially-weighted moving average of count per time
  /// unit within the specified interval. The default time unit is seconds.
  /// </para>
  /// <para>
  /// UNIX Load Average: How It Works
  ///  http://www.teamquest.com/pdfs/whitepaper/ldavg1.pdf
  /// </para>
  /// <para>
  /// UNIX Load Average: Not Your Average Average
  ///  http://www.teamquest.com/pdfs/whitepaper/ldavg2.pdf
  /// </para>
  /// </remarks>
  public partial class ExponentialWeightedMovingAverage : AbstractMetric, IMeter
  {
    const double kMinutesPerSecond = 60.0;

    const int kOneMinute = 1;
    const int kFiveMinutes = 5;
    const int kFifteenMinutes = 15;

    /// <summary>
    /// Defines a 5 seconds interval
    /// </summary>
    const int kFiveSecondsInterval = 5;

    /// <summary>
    /// The smoothing constant to be used when creating a one minute moving
    /// average.
    /// </summary>
    /// <remarks>
    /// When using this alpha factor the interval should be equals to 5 seconds.
    /// </remarks>
    static readonly double kOneMinuteAlpha = 1 -
      Math.Exp(-kFiveSecondsInterval/kMinutesPerSecond/kOneMinute);

    /// <summary>
    /// The smoothing constant to be used when creating a five minute moving
    /// average.
    /// </summary>
    /// <remarks>
    /// When using this alpha factor the interval should be equals to 5 seconds.
    /// </remarks>
    static readonly double kFiveMinutesAlpha = 1 -
      Math.Exp(-kFiveSecondsInterval/kMinutesPerSecond/kFiveMinutes);

    /// <summary>
    /// The smoothing constant to be used when creating a fifteen minute moving
    /// average.
    /// </summary>
    /// <remarks>
    /// When using this alpha factor the interval should be equals to 5 seconds.
    /// </remarks>
    static readonly double kFifteenMinutesAlpha = 1 -
      Math.Exp(-kFiveSecondsInterval/kMinutesPerSecond/kFifteenMinutes);

    readonly double alpha_;
    readonly long ticks_per_unit_;
    readonly Clock clock_;
    readonly double interval_;
    bool initialized_;

    double rate_;

    long uncounted_;
    long last_tick_;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="ExponentialWeightedMovingAverage"/> class by using the
    /// given smoothing constant, a 5 seconds tick interval and and seconds as
    /// the time unit.
    /// </summary>
    /// <param name="config">
    /// A <see cref="MetricConfig"/> containing the configuration settings
    /// for the metric.
    /// </param>
    /// <param name="alpha">
    /// The smoothing constant.
    /// </param>
    public ExponentialWeightedMovingAverage(MetricConfig config, double alpha)
      : this(config, alpha, TimeSpan.FromSeconds(kFiveSecondsInterval),
        TimeUnit.Seconds) {
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="ExponentialWeightedMovingAverage"/> class by using the
    /// given smoothing constant, expected tick interval and seconds as the
    /// time unit.
    /// </summary>
    /// <param name="config">
    /// A <see cref="MetricConfig"/> containing the configuration settings
    /// for the metric.
    /// </param>
    /// <param name="alpha">
    /// The smoothing constant.
    /// </param>
    /// <param name="interval">
    /// The expected tick interval.
    /// </param>
    public ExponentialWeightedMovingAverage(MetricConfig config, double alpha,
      TimeSpan interval) : this(config, alpha, interval, TimeUnit.Seconds) {
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="ExponentialWeightedMovingAverage"/> class by using the
    /// specified smoothing constant, expected tick interval and time unit of
    /// the tick interval.
    /// </summary>
    /// <param name="config">
    /// A <see cref="MetricConfig"/> containing the configuration settings
    /// for the metric.
    /// </param>
    /// <param name="alpha">
    /// The smoothing constant.
    /// </param>
    /// <param name="interval">
    /// The expected tick interval.
    /// </param>
    /// <param name="unit">
    /// The time unit that should be used to compute the rate.
    /// </param>
    public ExponentialWeightedMovingAverage(MetricConfig config, double alpha,
      TimeSpan interval, TimeUnit unit)
      : this(config, new Mailbox<Action>(runnable => runnable()), alpha,
        interval, unit, new StopwatchClock()) {
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="ExponentialWeightedMovingAverage"/> class by using the
    /// specified smoothing constant, expected tick interval and time unit of
    /// the tick interval.
    /// </summary>
    /// <param name="config">
    /// A <see cref="MetricConfig"/> containing the configuration settings
    /// for the metric.
    /// </param>
    /// <param name="mailbox">
    /// A <see cref="Mailbox{T}"/> that can be used to asynchrously process
    /// metrics operations.
    /// </param>
    /// <param name="alpha">
    /// The smoothing constant.
    /// </param>
    /// <param name="interval">
    /// The expected tick interval.
    /// </param>
    /// <param name="unit">
    /// The time unit that should be used to compute the rate.
    /// </param>
    /// <param name="clock">
    /// The clock that should be used to mark the passage of time.
    /// </param>
    internal ExponentialWeightedMovingAverage(MetricConfig config,
      Mailbox<Action> mailbox, double alpha, TimeSpan interval, TimeUnit unit,
      Clock clock) : base(config, mailbox) {
      interval_ = interval.Ticks;
      alpha_ = alpha;
      ticks_per_unit_ = TimeUnitHelper.ToTicks(1, unit);
      clock_ = clock;
      uncounted_ = 0;
      rate_ = 0.0;
      last_tick_ = clock_.Tick;
      initialized_ = false;
    }

    /// <inheritdoc/>
    public void Mark() {
      Mark(1);
    }

    /// <inheritdoc/>
    public void Mark(long n) {
      long timestamp = clock_.Tick;
      mailbox_.Send(() => Mark(n, timestamp));
    }

    /// <inheritdoc/>
    public override void GetMeasure(Action<Measure> callback) {
      long timestamp = clock_.Tick;
      mailbox_.Send(() => callback(Compute(timestamp)));
    }

    /// <inheritdoc/>
    public override void GetMeasure<T>(Action<Measure, T> callback, T context) {
      long timestamp = clock_.Tick;
      mailbox_.Send(() => callback(Compute(timestamp), context));
    }

    /// <inheritdoc/>
    protected internal override Measure Compute() {
      throw new NotSupportedException();
    }

    internal Measure Compute(long timestamp) {
      TickIfNecessary(timestamp);
      return CreateMeasure(rate_*ticks_per_unit_);
    }

    /// <summary>
    /// Update the moving average with the new value at the given timestamp.
    /// </summary>
    /// <param name="n">
    /// The new value.
    /// </param>
    /// <param name="timestamp">
    /// The time when the update should be performed.
    /// </param>
    /// <remarks>
    /// This method is internally visible to allow composite metrics to
    /// update multiple EWMA at the same time.
    /// </remarks>
    internal void Mark(long n, long timestamp) {
      TickIfNecessary(timestamp);
      uncounted_ += n;
    }

    void TickIfNecessary(long now) {
      long age = now - last_tick_;
      last_tick_ = now;
      if (age > interval_) {
        long required_ticks = (long) (age/interval_);
        for (long i = 0; i < required_ticks; i++) {
          Tick();
        }
      }
    }

    /// <summary>
    /// Mark the passage of time and decay the current rate accordingly.
    /// </summary>
    void Tick() {
      double instant_rate = uncounted_/interval_;
      if (initialized_) {
        rate_ += alpha_*(instant_rate - rate_);
      } else {
        rate_ = instant_rate;
        initialized_ = true;
      }
      uncounted_ = 0;
    }
  }
}
