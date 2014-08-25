using System;

namespace Nohros.Metrics.Reporting
{
  /// <summary>
  /// A reporter that receives updates about measures.
  /// </summary>
  public interface IMeasureObserver
  {
    /// <summary>
    /// Stops the reporter and deallocates any associated resources.
    /// </summary>
    void Shutdown();
  }
}
