using System;
using System.Diagnostics;

namespace Nohros.Metrics
{
  /// <summary>
  /// A timing context
  /// </summary>
  /// <seealso cref="Timer.Time{T}"/>
  public class TimerContext
  {
    readonly ITimer timer_;
    readonly Stopwatch watch_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="TimerContext"/> with the
    /// current time as its starting value and with the given
    /// <see cref="Timer"/>.
    /// </summary>
    /// <param name="timer">
    /// The <see cref="Timer"/> to report elapsed time.
    /// </param>
    public TimerContext(ITimer timer) {
      timer_ = timer;
      watch_ = new Stopwatch();
      watch_.Start();
    }
    #endregion

    /// <summary>
    /// Stops recording the elapsed time, updates the timer and returns the
    /// elapsed time.
    /// </summary>
    public TimeSpan Stop() {
      watch_.Stop();
      timer_.Update(watch_.Elapsed);
      return watch_.Elapsed;
    }
  }
}
