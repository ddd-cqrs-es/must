using System.Collections;
using System.Collections.Generic;

namespace Nohros.Collections
{
  /// <summary>
  /// provides a way to convert a <see cref="IEnumerable{T}"/> object into a
  /// <see cref="IReadOnlyCollection{T}"/> object.
  /// </summary>
  public static class ReadOnlyCollectionExtensions
  {
    /// <summary>
    /// Returns a read-only wrapper for the given <paramref name="enumerable"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The type of elements contained in the <paramref name="enumerable"/>.
    /// </typeparam>
    /// <param name="enumerable">
    /// The <see cref="IEnumerable{T}"/> to wrap.
    /// </param>
    /// <remarks>
    /// If the <paramref name="enumerable"/> is the result of a lazy-loaded
    /// operation it will be resolved as soon as the this class is constructed.
    /// <para>
    /// If the <paramref name="enumerable"/> represents an class that
    /// implements or derives from an <see cref="IList{T}"/> the list will be
    /// used directly and no elements copy will be performed, otherwise, the
    /// elements of the <see cref="enumerable"/> will be copied to a new
    /// collection.
    /// </para>
    /// <para>
    /// To prevent modifications to <paramref name="enumerable"/> objects that
    /// inherits or derives from <see cref="IList{T}"/>, expose the
    /// <paramref name="enumerable"/> only through this wrapper. The returned
    /// <see cref="IReadOnlyCollection{T}"/> does not expose methods that modify
    /// the collection. However, if changes are made to the underlying
    /// <paramref name="enumerable"/>, the read-only collection reflects those
    /// changes.
    /// </para>
    /// <para>
    /// This constructor is a O(1) operation for <paramref name="enumerable"/>
    /// that inherits or derive from <see cref="IList{T}"/> and a O(n)
    /// operation for other types.
    /// </para>
    /// </remarks>
    /// <returns></returns>
    public static IReadOnlyCollection<T> AsReadOnly<T>(
      this IEnumerable<T> enumerable) {
      return new ReadOnlyCollection<T>(enumerable);
    }
  }

  /// <summary>
  /// Provides the base class for generic read-only collection.
  /// </summary>
  /// <typeparam name="T">
  /// The type of elements in the collection.
  /// </typeparam>
  internal class ReadOnlyCollection<T> : IReadOnlyCollection<T>
  {
    readonly System.Collections.ObjectModel.ReadOnlyCollection<T> collection_;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlyCollection{T}"/>
    /// class that is a read-only wrapper around the specified
    /// <paramref name="enumerable"/>.
    /// </summary>
    /// <param name="enumerable">
    /// The <see cref="IEnumerable{T}"/> to wrap.
    /// </param>
    /// <remarks>
    /// If the <paramref name="enumerable"/> is the result of a lazy-loaded
    /// operation it will be resolved as soon as the this class is constructed.
    /// <para>
    /// If the <paramref name="enumerable"/> represents an class that
    /// implements or derives from an <see cref="IList{T}"/> the list will be
    /// used directly and no elements copy will be performed, otherwise, the
    /// elements of the <see cref="enumerable"/> will be copied to a new
    /// collection.
    /// </para>
    /// <para>
    /// To prevent modifications to <paramref name="enumerable"/> objects that
    /// inherits or derives from <see cref="IList{T}"/>, expose the
    /// <paramref name="enumerable"/> only through this wrapper. A
    /// <see cref="ReadOnlyCollection{T}"/> does not expose methods that modify
    /// the collection. However, if changes are made to the underlying
    /// <paramref name="enumerable"/>, the read-only collection reflects those
    /// changes.
    /// </para>
    /// <para>
    /// This constructor is a O(1) operation for <paramref name="enumerable"/>
    /// that inherits or derive from <see cref="IList{T}"/> and a O(n)
    /// operation for other types.
    /// </para>
    /// </remarks>
    internal ReadOnlyCollection(IEnumerable<T> enumerable) {
      var list = enumerable is IList<T>
        ? (IList<T>) enumerable
        : new List<T>(enumerable);
      collection_ =
        new System.Collections.ObjectModel.ReadOnlyCollection<T>(list);
    }

    IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }

    public IEnumerator<T> GetEnumerator() {
      return collection_.GetEnumerator();
    }

    /// <summary>
    /// Get the number of elements contained in the
    /// <see cref="ReadOnlyCollection{T}"/>.
    /// </summary>
    public int Count {
      get { return collection_.Count; }
    }
  }
}
