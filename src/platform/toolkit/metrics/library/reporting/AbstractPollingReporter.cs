using System;
using System.Threading;

namespace Nohros.Metrics.Reporting
{
  /// <summary>
  /// Provides a base class for all <see cref="IMeasureObserver"/>
  /// implementations which periodically poll registered metrics (e.g., to
  /// send data to another service).
  /// </summary>
  public abstract class AbstractPollingReporter : IPollingMeasureObserver
  {
    readonly MetricsRegistry registry_;
    System.Threading.Timer timer_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="AbstractPollingReporter"/>
    /// class by using the specified <see cref="MetricsRegistry"/> object.
    /// </summary>
    /// <param name="registry">
    /// A <see cref="MetricsRegistry"/> that can be used to collect the
    /// metrics to be reported.
    /// </param>
    protected AbstractPollingReporter(MetricsRegistry registry) {
      registry_ = registry;
    }
    #endregion

    /// <inheritdoc/>
    public virtual void Shutdown() {
      var waiter = new ManualResetEvent(false);
      Shutdown(waiter);
      waiter.WaitOne();
    }

    /// <inheritdoc/>
    public virtual void Shutdown(WaitHandle waiter) {
      timer_.Change(Timeout.Infinite, Timeout.Infinite);
      timer_.Dispose(waiter);
    }

    /// <summary>
    /// Starts the reporter polling at the given period.
    /// </summary>
    /// <param name="period">
    /// The amount of time between polls.
    /// </param>
    /// <param name="unit">
    /// The unit for <paramref name="period"/>
    /// </param>
    public virtual void Start(long period, TimeUnit unit) {
      timer_ = new System.Threading.Timer(obj => Run(), null, 0,
        TimeUnitHelper.ToMilliseconds(period, unit));
    }

    /// <summary>
    /// Starts the reporter polling at the given period.
    /// </summary>
    /// <param name="period">
    /// The amount of time between polls.
    /// </param>
    /// <param name="unit">
    /// The unit for <paramref name="period"/>
    /// </param>
    /// <param name="predicate">
    /// A <see cref="MetricPredicate"/> delegate that defines the conditions of
    /// the metrics to report.
    /// </param>
    public virtual void Start(long period, TimeUnit unit,
      MetricPredicate predicate) {
      timer_ = new System.Threading.Timer(obj => Run((MetricPredicate) obj),
        predicate, 0, TimeUnitHelper.ToMilliseconds(period, unit));
    }

    /// <summary>
    /// The method that is called when a poll is scheduled to occur.
    /// </summary>
    public abstract void Run();

    /// <summary>
    /// The method that is called when a poll is scheduled to occur.
    /// </summary>
    public abstract void Run(MetricPredicate predicate);

    /// <summary>
    /// Gets the associated metrics registry.
    /// </summary>
    protected internal MetricsRegistry MetricsRegistry {
      get { return registry_; }
    }
  }
}
