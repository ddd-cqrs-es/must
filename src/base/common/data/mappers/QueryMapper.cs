using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Nohros.Data
{
  /// <summary>
  /// The default implementation of the <see cref="IQueryMapper{T}"/> interface.
  /// </summary>
  /// <typeparam name="T">
  /// The type of the class to be mapped.
  /// </typeparam>
  /// <remarks>
  /// The <see cref="QueryMapper{T}"/> maps the result of a
  /// <see cref="IDataReader"/> using a supplied
  /// <see cref="IDataReaderMapper{T}"/> and dispose the objects associated
  /// with the <see cref="IDataReader"/> when the <see cref="QueryMapper{T}"/>
  /// is disposed.
  /// </remarks>
  public class QueryMapper<T> : IQueryMapper<T>
  {
    readonly IDataReaderMapper<T> mapper_;
    readonly IDataReader reader_;
    readonly IDisposable[] disposables_;

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryMapper{T}"/>
    /// </summary>
    /// <param name="mapper">
    /// A <see cref="IDataReaderMapper{T}"/> that should be used to map
    /// the data containined into the <paramref name="reader"/> to a
    /// object or a collection of objects of the type <typeparamref name="T"/>.
    /// </param>
    /// <param name="reader">
    /// A <see cref="IDataReader"/> to fetch the data.
    /// </param>
    public QueryMapper(IDataReaderMapper<T> mapper, IDataReader reader)
      : this(mapper, reader, Enumerable.Empty<IDisposable>()) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryMapper{T}"/>
    /// </summary>
    /// <param name="mapper">
    /// A <see cref="IDataReaderMapper{T}"/> that should be used to map
    /// the data containined into the <paramref name="reader"/> to a
    /// object or a collection of objects of the type <typeparamref name="T"/>.
    /// </param>
    /// <param name="reader">
    /// A <see cref="IDataReader"/> to fetch the data.
    /// </param>
    /// <param name="disposables">
    /// A collection of <see cref="IDisposable"/> objects related with the
    /// given <paramref name="reader"/> that needs to be disposed as the same
    /// time as the <see cref="QueryMapper{T}"/> object.
    /// </param>
    public QueryMapper(IDataReaderMapper<T> mapper,
      IDataReader reader,
      IEnumerable<IDisposable> disposables) {
      mapper_ = mapper;
      reader_ = reader;
      disposables_ = disposables.ToArray();
    }

    /// <summary>
    /// Maps a element of a query result to a object of type
    /// <typeparamref name="T"/>.
    /// </summary>
    /// <returns>
    /// A object of type <typeparamref name="T"/> containing the data readed
    /// from the current row of the associated <see cref="IDataReader"/>.
    /// </returns>
    public T Map() {
      return mapper_.Map(reader_);
    }

    /// <summary>
    /// maps a element of a query result to a object of type
    /// <typeparamref name="T"/>.
    /// </summary>
    /// <param name="t">
    /// When this method returns contains a object of type
    /// <typeparamref name="T"/> associated the data readed from the current
    /// row of the associated <see cref="IDataReader"/>.
    /// </param>
    /// <returns>
    /// <c>true</c> when the map operation the data returned from the database
    /// was successfully mapped to a object of type <typeparamref name="T"/>;
    /// otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// When the associated <see cref="IDataReader"/> does not contain any
    /// data, <c>false</c> will be returned to indicate that the expected
    /// result does not exists.
    /// </remarks>
    public bool Map(out T t) {
      return mapper_.Map(reader_, out t);
    }

    /// <summary>
    /// Maps a element of a query result to a object of type
    /// <typeparamref name="T"/>.
    /// </summary>
    /// <returns>
    /// A object of type <typeparamref name="T"/> containing the data readed
    /// from the next row of the associated <see cref="IDataReader"/>.
    /// </returns>
    /// <remarks>
    /// The <see cref="IDataReader.Read"/> method will be called to fetch the
    /// next record.
    /// </remarks>
    public T Map(Action<T> post_map) {
      return mapper_.Map(reader_, post_map);
    }

    /// <summary>
    /// Maps a element of a query result to a object of type using the current
    /// record of the <see cref="IDataReader"/>.
    /// <typeparamref name="T"/>.
    /// </summary>
    /// <returns>
    /// A object of type <typeparamref name="T"/> containing the data readed
    /// from the current row of the associated <see cref="IDataReader"/>.
    /// </returns>
    /// <remarks>
    /// The associated <see cref="IDataReader"/> should be positioned at a
    /// valid record.
    /// </remarks>
    public T MapCurrent() {
      return mapper_.MapCurrent(reader_);
    }

    /// <summary>
    /// Maps a element of a query result to a object of type using the current
    /// record of the <see cref="IDataReader"/>.
    /// <typeparamref name="T"/>.
    /// </summary>
    /// <remarks>
    /// The associated <see cref="IDataReader"/> should be positioned at a
    /// valid record.
    /// </remarks>
    public void MapCurrent(T t) {
      mapper_.MapCurrent(reader_, t);
    }

    /// <summary>
    /// Maps a element of a query result to a collection of object of type
    /// <typeparamref name="T"/>.
    /// </summary>
    /// <returns>
    /// A collection of objects of type <typeparamref name="T"/> containing
    /// the data readed from the associated <see cref="IDataReader"/>.
    /// </returns>
    public IEnumerable<T> Map(bool defer) {
      return mapper_.Map(reader_, defer);
    }

    /// <summary>
    /// Maps a element of a query result to a collection of object of type
    /// <typeparamref name="T"/>.
    /// </summary>
    /// <returns>
    /// A collection of objects of type <typeparamref name="T"/> containing
    /// the data readed from the associated <see cref="IDataReader"/>.
    /// </returns>
    public IEnumerable<T> Map(bool defer, Action<T> post_map) {
      return mapper_.Map(reader_, defer, post_map);
    }

    /// <inheritdoc/>
    public IReadOnlyCollection<T> MapReadOnly() {
      return mapper_.MapReadOnly(reader_);
    }

    /// <inheritdoc/>
    public IReadOnlyCollection<T> MapReadOnly(Action<T> post_map) {
      return mapper_.MapReadOnly(reader_, post_map);
    }

    /// <summary>
    /// Gets the underlying <see cref="IDataReader"/>.
    /// </summary>
    [Obsolete("This method is obsolte. Use the BaseDataReader instead", false)]
    public IDataReader Reader {
      get { return BaseDataReader; }
    }

    /// <summary>
    /// Gets the underlying <see cref="IDataReader"/>.
    /// </summary>
    public IDataReader BaseDataReader {
      get { return reader_; }
    }

    /// <inheritdoc/>
    public void Dispose() {
      reader_.Dispose();
      foreach (var disposable in disposables_) {
        if (disposable != null) {
          disposable.Dispose();
        }
      }
    }
  }
}
