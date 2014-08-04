using System;

namespace Nohros.Metrics
{
  public interface ICounter : ICounted
  {
    /// <summary>
    /// Increments the counter by one.
    /// </summary>
    /// <param name="callback">
    /// A <see cref="CountedCallback"/> method that will be executed before the
    /// increment operation completes. The callback is executed in sync with the
    /// increment operation.
    /// </param>
    void Increment(CountedCallback callback);

    /// <summary>
    /// Increments the counter by <paramref name="n"/>.
    /// </summary>
    /// <param name="n">
    /// The amount by which the counter will be increased.
    /// </param>
    /// <param name="callback">
    /// A <see cref="CountedCallback"/> method that will be executed before the
    /// increment operation completes. The callback is executed in sync with the
    /// increment operation.
    /// </param>
    void Increment(long n, CountedCallback callback);

    /// <summary>
    /// Decrements the counter by one.
    /// </summary>
    /// <param name="callback">
    /// A <see cref="CountedCallback"/> method that will be executed before the
    /// decrement operation completes. The callback is executed in sync with the
    /// decrement operation.
    /// </param>
    void Decrement(CountedCallback callback);

    /// <summary>
    /// Decrements the counter by <paramref name="n"/>
    /// </summary>
    /// <param name="n">
    /// The amount by which the counter will be increased.
    /// </param>
    /// <param name="callback">
    /// A <see cref="CountedCallback"/> method that will be executed before the
    /// decrement operation completes. The callback is executed in sync with the
    /// decrement operation.
    /// </param>
    void Decrement(long n, CountedCallback callback);
  }
}
