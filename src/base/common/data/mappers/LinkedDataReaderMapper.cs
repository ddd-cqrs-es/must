using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Nohros.Data
{
  /// <summary>
  /// A <see cref="IDataReaderMapper{T}"/> that can be used to map a single
  /// row to multiple objects through linking.
  /// </summary>
  internal class LinkedDataReaderMapper<T> : IDataReaderMapper<T>
  {
    readonly IDataReaderMapper<T> mapper_;
    readonly List<Action<IDataReader, T>> nodes_;

    /// <summary>
    /// Initializes a new instance of the <see cref="LinkedDataReaderMapper{T}"/>
    /// class by using the given <paramref name="mapper"/>.
    /// </summary>
    /// <param name="mapper">
    /// The first <see cref="IDataReaderMapper{T}"/> of the chain.
    /// </param>
    /// <param name="nodes"></param>
    public LinkedDataReaderMapper(IDataReaderMapper<T> mapper,
      IEnumerable<Action<IDataReader, T>> nodes) {
      mapper_ = mapper;
      nodes_ = nodes.ToList();
    }

    /// <inheritdoc/>
    public T Map(IDataReader reader) {
      return Link(reader, mapper_.Map(reader));
    }

    /// <inheritdoc/>
    public bool Map(IDataReader reader, out T t) {
      bool result = mapper_.Map(reader, out t);
      t = Link(reader, t);
      return result;
    }

    /// <inheritdoc/>
    public T Map(IDataReader reader, Action<T> post_map) {
      return Link(reader, mapper_.Map(reader, post_map));
    }

    /// <inheritdoc/>
    public T MapCurrent(IDataReader reader) {
      return Link(reader, mapper_.MapCurrent(reader));
    }

    /// <inheritdoc/>
    public IEnumerable<T> Map(IDataReader reader, bool defer) {
      return mapper_.Map(reader, defer).Select(t => Link(reader, t));
    }

    /// <inheritdoc/>
    public IEnumerable<T> Map(IDataReader reader, bool defer, Action<T> post_map) {
      return mapper_.Map(reader, defer, post_map).Select(t => Link(reader, t));
    }

    T Link(IDataReader reader, T t) {
      foreach (var action in nodes_) {
        action(reader, t);
      }
      return t;
    }
  }
}
