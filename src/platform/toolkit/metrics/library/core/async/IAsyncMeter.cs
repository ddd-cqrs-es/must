﻿using System;

namespace Nohros.Metrics
{
  public interface IAsyncMeter : IAsyncMetered
  {
    /// <summary>
    /// Mark the occurrence of an event.
    /// </summary>
    void Mark();

    /// <summary>
    /// Mark the occurrence of a given number of events.
    /// </summary>
    /// <param name="n">
    /// The number of events.
    /// </param>
    void Mark(long n);
  }
}
