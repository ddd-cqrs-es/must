using System;

namespace Nohros.Metrics
{
  /// <summary>
  /// A timing context
  /// </summary>
  /// <seealso cref="Timer.Time{T}"/>
  public class TimerContext
  {
    readonly ITimer timer_;
    readonly long start_time_;
    readonly Clock clock_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="TimerContext"/> with the
    /// current time as its starting value and with the given
    /// <see cref="Timer"/>.
    /// </summary>
    /// <param name="timer">
    /// The <see cref="Timer"/> to report elapsed time.
    /// </param>
    /// <param name="clock">
    /// A <see cref="Clock"/> that can be used to mark the passage of time.
    /// </param>
    public TimerContext(ITimer timer, Clock clock) {
      timer_ = timer;
      clock_ = clock;
      start_time_ = clock.Tick;
    }
    #endregion

    /// <summary>
    /// Stops recording the elapsed time, updates the timer and returns the
    /// elapsed time.
    /// </summary>
    public long Stop() {
      long elapsed_nanos = clock_.Tick - start_time_;
      timer_.Update(elapsed_nanos);
      return elapsed_nanos;
    }
  }
}
