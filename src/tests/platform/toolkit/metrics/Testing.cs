using System;
using System.Threading;

namespace Nohros.Metrics.Tests
{
  public static class Testing
  {
    public static T Sync<T>(IMetric metric, Action<Action<T>> async) {
#if DEBUG
      var signaler = new ManualResetEventSlim(false);
      T result = default(T);
      async(arg1 => result = arg1);
      metric.Run(signaler.Set);
      signaler.Wait();
      return result;
#else
      throw new NotImplementedException();
#endif
    }

    public static double Sync(IMetric metric, Action<Action<Measure>> async) {
      return Sync<Measure>(metric, async).Value;
    }
  }
}
