using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Nohros.Concurrent
{
  /// <summary>
  /// Factory methods for <see cref="IExecutor"/>.
  /// </summary>
  public sealed class Executors
  {
    /// <summary>
    /// Creates an executor that runs each task in the thread that invokes
    /// <see cref="IExecutor.Execute"/>.
    /// </summary>
    /// <returns>
    /// An executor that runs each task in the thread that invokes
    /// <see cref="IExecutor.Execute"/>.
    /// </returns>
    /// <remarks>
    /// Tasks are immediately executed in the thread that submitted the task.
    /// </remarks>
    public static IExecutor SameThreadExecutor() {
      return new SameThreadExecutor();
    }

    /// <summary>
    /// Creates an <see cref="IExecutor"/> that executes each submitted task
    /// using a thread from <see cref="ThreadPool"/>.
    /// </summary>
    /// <returns>
    /// An <see cref="IExecutor"/> that executes each submitted task using a
    /// thread from <see cref="ThreadPool"/>.
    /// </returns>
    public static IExecutor ThreadPoolExecutor() {
      return new ThreadPoolExecutor();
    }
  }
}
