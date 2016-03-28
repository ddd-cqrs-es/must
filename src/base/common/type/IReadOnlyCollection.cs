#if NET45
// .NET 4.5 already has a class named IReadOnlyCollection<T> that has the
// same signature.
#else
using System.Collections.Generic;

namespace Nohros
{
  /// <summary>
  /// Represents a stringly-typed, read-only collection of elements.
  /// </summary>
  /// <typeparam name="T">
  /// The type of the elements
  /// </typeparam>
  public interface IReadOnlyCollection<out T> : IEnumerable<T>
  {
    /// <summary>
    /// Gets the number of elements in the collection.
    /// </summary>
    int Count { get; }
  }
}
#endif
