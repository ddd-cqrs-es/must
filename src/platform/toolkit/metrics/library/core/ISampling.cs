using System;

namespace Nohros.Metrics
{
  /// <summary>
  /// An object which samples values.
  /// </summary>
  public interface ISampling
  {
    /// <summary>
    /// Gets a snapshot of the values.
    /// </summary>
    /// <value>A snapshot of the values.</value>
    void GetSnapshot(SnapshotCallback callback);
  }
}
