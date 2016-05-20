#if NET45
  // .NET 4.5 already has a class named IReadOnlyList<T> that has the
  // same signature.
#else
using System.Collections.Generic;

namespace Nohros.Collections
{
  /// <summary>
  /// Represents a stringly-typed, read-only collection of elements that can be
  /// accessed by index.
  /// </summary>
  /// <typeparam name="T">
  /// The type of the elements in the read-only list.
  /// </typeparam>
  public interface IReadOnlyList<out T> : IReadOnlyCollection<T>
  {
    /// <summary>
    /// Gets the element at the specified index in the read-only list.
    /// </summary>
    /// <param name="i">
    /// The zero-based index of the element to get.
    /// </param>
    /// <value>
    /// The element at the specified index in the read-only list.
    /// </value>
    T this[int i] { get; }
  }
}

#endif
