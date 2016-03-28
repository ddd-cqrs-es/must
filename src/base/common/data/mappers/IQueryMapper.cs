using System;
using System.Collections.Generic;
using System.Data;

namespace Nohros.Data
{
  /// <summary>
  /// A class that maps the fields of a <see cref="IDataReader"/> to the
  /// properties of an instance of the type <typeparamref name="T"/>.
  /// </summary>
  /// <typeparam name="T">
  /// The type that should be mapped.
  /// </typeparam>
  /// <remarks>
  /// The <see cref="IQueryMapper{T}"/> class is similar to the
  /// <see cref="IDataReaderMapper{T}"/> class by is intended
  /// to be used to map the result of a query once, while
  /// <see cref="IDataReaderMapper{T}"/> is intended to map the result of a
  /// query multiple times.
  /// <para>
  /// The Map method overloads of the <see cref="IQueryMapper{T}"/> class
  /// does not accept a <see cref="IDataReader"/>, so it must be set in the
  /// object constructor. This prevents the <see cref="IDataReader"/> to be
  /// disposed by the method that creates it. For that reason the
  /// <see cref="IQueryMapper{T}"/> should implements the
  /// <see cref="IDisposable"/> interface and should dispose the associated
  /// <see cref="IDataReader"/> when its <see cref="IDisposable.Dispose"/>
  /// method is called.
  /// </para>
  /// </remarks>
  public interface IQueryMapper<T> : IDisposable
  {
    /// <summary>
    /// Maps a element of a query result to a object of type
    /// <typeparamref name="T"/>.
    /// </summary>
    /// <returns>
    /// A object of type <typeparamref name="T"/> containing the data readed
    /// from the current row of the associated <see cref="IDataReader"/>.
    /// </returns>
    T Map();

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
    bool Map(out T t);

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
    T Map(Action<T> post_map);

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
    T MapCurrent();

    /// <summary>
    /// Maps a element of a query result to a collection of object of type
    /// <typeparamref name="T"/>.
    /// </summary>
    /// <returns>
    /// A collection of objects of type <typeparamref name="T"/> containing
    /// the data readed from the associated <see cref="IDataReader"/>.
    /// </returns>
    IEnumerable<T> Map(bool defer);

    /// <summary>
    /// Maps a element of a query result to a collection of object of type
    /// <typeparamref name="T"/>.
    /// </summary>
    /// <returns>
    /// A collection of objects of type <typeparamref name="T"/> containing
    /// the data readed from the associated <see cref="IDataReader"/>.
    /// </returns>
    IEnumerable<T> Map(bool defer, Action<T> post_map);

    /// <summary>
    /// Maps a element of a query result to a collection of object of type
    /// <typeparamref name="T"/>.
    /// </summary>
    /// <returns>
    /// A collection of objects of type <typeparamref name="T"/> containing
    /// the data readed from the associated <see cref="IDataReader"/>.
    /// </returns>
    IReadOnlyCollection<T> MapReadOnly();

    /// <summary>
    /// Maps a element of a query result to a collection of object of type
    /// <typeparamref name="T"/>.
    /// </summary>
    /// <returns>
    /// A collection of objects of type <typeparamref name="T"/> containing
    /// the data readed from the associated <see cref="IDataReader"/>.
    /// </returns>
    IReadOnlyCollection<T> MapReadOnly(Action<T> post_map);
  }
}
