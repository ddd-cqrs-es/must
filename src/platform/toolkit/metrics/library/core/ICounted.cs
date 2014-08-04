using System;

namespace Nohros.Metrics
{
  public interface ICounted
  {
    /// <summary>
    /// Get the number of values recorded.
    /// </summary>
    /// <returns>The number of values recorded.</returns>
    void GetCount(LongMetricCallback callback);
  }
}
