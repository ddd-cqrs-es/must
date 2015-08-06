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
  public interface IDataReaderMapper<T>
  {
    /// <summary>
    /// Maps a element of a query result to a object of type
    /// <typeparamref name="T"/>.
    /// </summary>
    /// <returns>
    /// A object of type <typeparamref name="T"/> containing the data readed
    /// from the current row of the given <paramref name="reader"/>.
    /// </returns>
    T Map(IDataReader reader);

    /// <summary>
    /// maps a element of a query result to a object of type
    /// <typeparamref name="T"/>.
    /// </summary>
    /// <param name="t">
    /// When this method returns contains a object of type
    /// <typeparamref name="T"/> associated the data readed from the current
    /// row of the given <paramref name="reader"/>.
    /// </param>
    /// <returns>
    /// <c>true</c> when the map operation the data returned from the database
    /// was successfully mapped to a object of type <typeparamref name="T"/>;
    /// otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// When <paramref name="reader"/> does not contain any data, <c>false</c>
    /// will be returned to indicate that the expected result does not exists.
    /// </remarks>
    bool Map(IDataReader reader, out T t);

    /// <summary>
    /// Maps a element of a query result to a object of type
    /// <typeparamref name="T"/>.
    /// </summary>
    /// <returns>
    /// A object of type <typeparamref name="T"/> containing the data readed
    /// from the next row of the given <paramref name="reader"/>.
    /// </returns>
    /// <remarks>
    /// The <see cref="IDataReader.Read"/> method will be called to fetch the
    /// next record.
    /// </remarks>
    T Map(IDataReader reader, Action<T> post_map);

    /// <summary>
    /// Maps a element of a query result to a object of type using the current
    /// record of the <see cref="IDataReader"/>.
    /// <typeparamref name="T"/>.
    /// </summary>
    /// <returns>
    /// A object of type <typeparamref name="T"/> containing the data readed
    /// from the current row of the given <paramref name="reader"/>.
    /// </returns>
    /// <remarks>
    /// The given <paramref name="reader"/> should be positioned at a valid
    /// record.
    /// </remarks>
    T MapCurrent(IDataReader reader);

    /// <summary>
    /// Maps a element of a query result to a object of type using the current
    /// record of the <see cref="IDataReader"/>.
    /// <typeparamref name="T"/>.
    /// </summary>
    /// <remarks>
    /// The given <paramref name="reader"/> should be positioned at a valid
    /// record.
    /// </remarks>
    void MapCurrent(IDataReader reader, T t);

    /// <summary>
    /// Maps a element of a query result to a collection of object of type
    /// <typeparamref name="T"/>.
    /// </summary>
    /// <returns>
    /// A collection of objects of type <typeparamref name="T"/> containing
    /// the data readed from the given <paramref name="reader"/>.
    /// </returns>
    IEnumerable<T> Map(IDataReader reader, bool defer);

    /// <summary>
    /// Maps a element of a query result to a collection of object of type
    /// <typeparamref name="T"/>.
    /// </summary>
    /// <returns>
    /// A collection of objects of type <typeparamref name="T"/> containing
    /// the data readed from the given <paramref name="reader"/>.
    /// </returns>
    IEnumerable<T> Map(IDataReader reader, bool defer, Action<T> post_map);
  }
}
