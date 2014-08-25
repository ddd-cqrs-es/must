using System;
using System.Threading;

namespace Nohros.Metrics.Reporting
{
  public interface IPollingMeasureObserver : IMeasureObserver
  {
    /// <summary>
    /// Starts the reporter polling at the given period.
    /// </summary>
    /// <param name="period">
    /// The amount of time between polls.
    /// </param>
    /// <param name="unit">
    /// The unit for <paramref name="period"/>
    /// </param>
    void Start(long period, TimeUnit unit);

    /// <summary>
    /// Starts the reporter polling at the given period.
    /// </summary>
    /// <param name="period">
    /// The amount of time between polls.
    /// </param>
    /// <param name="unit">
    /// The unit for <paramref name="period"/>.
    /// </param>
    /// <param name="predicate">
    /// A <see cref="MetricPredicate"/> delegate that defines the conditions of
    /// the metrics to report.
    /// </param>
    void Start(long period, TimeUnit unit, MetricPredicate predicate);

    /// <summary>
    /// Stops the reporter and deallocates any associated resources.
    /// </summary>
    void Shutdown(WaitHandle waiter);
  }
}
