using System;
using System.Threading;
using Nohros.Concurrent;

namespace Nohros.Metrics
{
  /// <summary>
  /// An exponentially-weighted moving average.
  /// </summary>
  /// <remarks>
  /// <para>
  /// UNIX Load Average: How It Works
  ///  http://www.teamquest.com/pdfs/whitepaper/ldavg1.pdf
  /// </para>
  /// <para>
  /// UNIX Load Average: Not Your Average Average
  ///  http://www.teamquest.com/pdfs/whitepaper/ldavg2.pdf
  /// </para>
  /// </remarks>
  public class ExponentialWeightedMovingAverage
  {
    readonly double alpha_;
    readonly double interval_;
    volatile bool initialized_ = false;
    double rate_ = 0.0;

    long uncounted_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="ExponentialWeightedMovingAverage"/> class by using the
    /// specified smoothing constant, expected tick interval and time unit of
    /// the tick interval.
    /// </summary>
    /// <param name="alpha">The smoothing constant.</param>
    /// <param name="interval">The expected tick interval.</param>
    /// <param name="interval_unit">The time unit of the tick interval.</param>
    public ExponentialWeightedMovingAverage(double alpha, long interval,
      TimeUnit interval_unit) {
      interval_ = TimeUnitHelper.ToNanos(interval, interval_unit);
      alpha_ = alpha;
      uncounted_ = 0;
    }
    #endregion

    /// <summary>
    /// Update the moving average with the new value.
    /// </summary>
    /// <param name="n">The new value.</param>
    public void Update(long n) {
      uncounted_ += n;
    }

    /// <summary>
    /// Mark the passage of time and decay the current rate accordingly.
    /// </summary>
    public void Tick() {
      long count = uncounted_;
      double instant_rate = count/interval_;
      if (initialized_) {
        rate_ += alpha_*(instant_rate - rate_);
      } else {
        rate_ = instant_rate;
        initialized_ = true;
      }
      uncounted_ = 0;
    }

    /// <summary>
    /// Gets the rate in the given units of time
    /// </summary>
    public double Rate(TimeUnit rate_unit) {
      return rate_* TimeUnitHelper.ToNanos(1, rate_unit);
    }
  }
}
