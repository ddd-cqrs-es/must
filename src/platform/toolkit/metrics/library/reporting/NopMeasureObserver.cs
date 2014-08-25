using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Nohros.Metrics.Reporting
{
  /// <summary>
  /// A <see cref="IPollingMeasureObserver"/> that does nothing.
  /// </summary>
  public class NopMeasureObserver : IPollingMeasureObserver
  {
    public void Shutdown() {
    }

    public void Start(long period, TimeUnit unit) {
    }

    public void Start(long period, TimeUnit unit, MetricPredicate predicate) {
    }

    public void Shutdown(WaitHandle waiter) {
    }
  }
}
