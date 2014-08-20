using System;
using System.Threading;
using Nohros.Logging;

namespace Nohros.Concurrent
{
  /// <summary>
  /// Defines a way to schedule a task in a non-reentrant manner.
  /// </summary>
  /// <remarks>
  /// The <see cref="NonReentrantSchedule"/> is non-reentrant. When a task
  /// is running the <see cref="NonReentrantSchedule"/> wait it to finish
  /// before start a new one. After a task is finished the
  /// <see cref="NonReentrantSchedule"/> waits a predefined 
  /// interval and start the task again.
  /// <para>
  /// The tasks runs in a background thread.
  /// </para>
  /// <para>
  /// Any unhandled exception is logged through the configured
  /// <see cref="MustLogger"/> and swallowed. Exceptions raised by one task
  /// does not stops the scheduling.
  /// </para>
  /// </remarks>
  public class NonReentrantSchedule
  {
    readonly TimeSpan interval_;
    readonly ManualResetEvent signaler_;
    bool already_started_;
    Action<object> task_;

    /// <summary>
    /// Initializes a new instance of the <see cref="NonReentrantSchedule"/>
    /// class by using the given interval.
    /// </summary>
    NonReentrantSchedule(TimeSpan interval) {
      interval_ = interval;
      signaler_ = new ManualResetEvent(false);
      already_started_ = false;
      task_ = null;
    }

    /// <summary>
    /// Returns a <see cref="NonReentrantSchedule"/> class than run a task
    /// at every <see cref="interval"/>.
    /// </summary>
    /// <param name="interval">
    /// The interval to schedule a task.
    /// </param>
    /// <returns>
    /// A <see cref="NonReentrantSchedule"/> object that runs a task at every
    /// <see cref="interval"/>.
    /// </returns>
    /// <remarks>
    /// The returned <see cref="NonReentrantSchedule"/> will use a dedicated
    /// thread to run the task.
    /// </remarks>
    public static NonReentrantSchedule Every(TimeSpan interval) {
      return new NonReentrantSchedule(interval);
    }

    /// <summary>
    /// Defines the action that should run at the associated interval.
    /// </summary>
    /// <param name="task">
    /// The action that should run at the associated interval.
    /// </param>
    public void Runnable(Action task) {
      Runnable(obj => task(), null);
    }

    /// <summary>
    /// Defines the action that should run at the associated interval.
    /// </summary>
    /// <param name="task">
    /// The action that should run at the associated interval.
    /// </param>
    /// <param name="state">
    /// </param>
    public void Runnable(Action<object> task, object state) {
      if (already_started_) {
        throw new InvalidOperationException("The task is already defined.");
      }
      already_started_ = true;
      task_ = task;

      new BackgroundThreadFactory()
        .CreateThread(ThreadMain)
        .Start(state);
    }


    void ThreadMain(object obj) {
      while (!signaler_.WaitOne(interval_)) {
        task_(obj);
      }
    }

    /// <summary>
    /// Stops scheduling tasks and returns an <seealso cref="WaitHandle"/> that
    /// can be used to monitor when the pending task finish.
    /// </summary>
    /// <remarks>
    /// If there is no task running <seealso cref="WaitHandle.WaitOne()"/>
    /// overloads returns imediatelly.
    /// </remarks>
    public WaitHandle Stop() {
      signaler_.Set();
      return signaler_;
    }
  }
}
