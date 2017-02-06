using System;
using System.Collections.Generic;

namespace Nohros.Data
{
  public partial class DataReaderMapperBuilder<T>
  {
    /// <summary>
    /// Builds a dynamic <see cref="DataReaderMapper{T}"/> for the type
    /// <typeparamref source="T"/> using the specified
    /// properties.
    /// </summary>
    /// <param name="mapping">
    /// An <see cref="KeyValuePair{TKey,TValue}"/> containing the mapping
    /// between source columns and destination properties.
    /// </param>
    /// <remarks>
    /// The <see cref="KeyValuePair{TKey,TValue}.Value"/> is used as the source
    /// column name and the <see cref="KeyValuePair{TKey,TValue}.Key"/> is
    /// used as the destination property name.
    /// </remarks>
    [Obsolete(
      "This method is obsolete. Use the Map method that receives a LINQ expression."
      , true)]
    public DataReaderMapperBuilder<T> Map(
      IEnumerable<KeyValuePair<string, string>> mapping) {
      foreach (KeyValuePair<string, string> map in mapping) {
        Map(map.Key, map.Value);
      }
      return this;
    }

    /// <summary>
    /// Builds a dynamic <see cref="DataReaderMapper{T}"/> for the type
    /// <typeparamref source="T"/> using the specified
    /// properties.
    /// </summary>
    /// <param name="mapping">
    /// An <see cref="KeyValuePair{TKey,TValue}"/> containing the mapping
    /// between source columns and destination properties.
    /// </param>
    /// <remarks>
    /// The <see cref="KeyValuePair{TKey,TValue}.Value"/> is used as the source
    /// column name and the <see cref="KeyValuePair{TKey,TValue}.Key"/> is
    /// used as the destination property name.
    /// </remarks>
    [Obsolete(
      "This method is obsolete. Use the Map method that receives a LINQ expression."
      , true)]
    public DataReaderMapperBuilder<T> Map(
      IEnumerable<KeyValuePair<string, ITypeMap>> mapping) {
      foreach (KeyValuePair<string, ITypeMap> map in mapping) {
        Map(map.Key, map.Value);
      }
      return this;
    }

    /// <summary>
    /// Maps the source column <paramref source="source"/> to the interface
    /// property <paramref source="destination"/>.
    /// </summary>
    /// <param name="source">
    /// The source of the source column.
    /// </param>
    /// <param name="destination">
    /// The source of the property that will be mapped to the column
    /// <paramref name="source"/>.
    /// </param>
    /// <returns>
    /// A <see cref="DataReaderMapperBuilder{T}"/> that builds an object of type
    /// <typeparamref source="T"/> and mapping the column <paramref source="source"/>
    /// to the property named <paramref source="destination"/>.
    /// </returns>
    [Obsolete(
      "This method is obsolete. Use the Map method that receives a LINQ expression."
      , true)]
    public DataReaderMapperBuilder<T> Map(string destination, string source) {
      return Map(destination, source, null);
    }

    /// <summary>
    /// Maps the source column <paramref source="source"/> to the interface
    /// property <paramref source="destination"/>.
    /// </summary>
    /// <param name="source">
    /// The source of the source column.
    /// </param>
    /// <param name="destination">
    /// The source of the property that will be mapped to the column
    /// <paramref name="source"/>.
    /// </param>
    /// <returns>
    /// A <see cref="DataReaderMapperBuilder{T}"/> that builds an object of type
    /// <typeparamref source="T"/> and mapping the column <paramref source="source"/>
    /// to the property named <paramref source="destination"/>.
    /// </returns>
    [Obsolete(
      "This method is obsolete. Use the Map method that receives a LINQ expression."
      , true)]
    public DataReaderMapperBuilder<T> Map<TMap>(string destination,
      string source) {
      return Map(destination, source, typeof (TMap));
    }

    /// <summary>
    /// Maps the source column <paramref source="source"/> to the interface
    /// property <paramref source="destination"/>.
    /// </summary>
    /// <param name="source">
    /// The source of the source column.
    /// </param>
    /// <param name="destination">
    /// The source of the property that will be mapped to the column
    /// <paramref name="source"/>.
    /// </param>
    /// <param name="type">
    /// The type of source column.
    /// </param>
    /// <returns>
    /// A <see cref="DataReaderMapperBuilder{T}"/> that builds an object of type
    /// <typeparamref source="T"/> and mapping the column <paramref source="source"/>
    /// to the property named <paramref source="destination"/>.
    /// </returns>
    [Obsolete(
      "This method is obsolete. Use the Map method that receives a LINQ expression."
      , true)]
    public DataReaderMapperBuilder<T> Map(string destination,
      string source,
      Type type) {
      return Map(destination, GetTypeMap(source, type));
    }

    /// <summary>
    /// Maps the constant value <see cref="value"/> to the interface
    /// property <paramref source="destination"/>.
    /// </summary>
    /// <param name="value">
    /// The value that should be returned when by the interface property
    /// <paramref name="destination"/>.
    /// </param>
    /// <param name="destination">
    /// The source of the property that will be mapped to the value
    /// <paramref name="value"/>.
    /// </param>
    /// <returns>
    /// A <see cref="DataReaderMapperBuilder{T}"/> that builds an object of type
    /// <typeparamref source="T"/> and mapping the constant value
    /// <paramref source="value"/> to the property named
    /// <paramref source="destination"/>.
    /// </returns>
    [Obsolete(
      "This method is obsolete. Use the Map method that receives a LINQ expression."
      , true)]
    public DataReaderMapperBuilder<T> Map(string destination, int value) {
      return Map(destination, TypeMaps.Integer(value));
    }

    /// <summary>
    /// Maps the constant value <see cref="value"/> to the interface
    /// property <paramref source="destination"/>.
    /// </summary>
    /// <param name="value">
    /// The value that should be returned when by the interface property
    /// <paramref name="destination"/>.
    /// </param>
    /// <param name="destination">
    /// The source of the property that will be mapped to the value
    /// <paramref name="value"/>.
    /// </param>
    /// <returns>
    /// A <see cref="DataReaderMapperBuilder{T}"/> that builds an object of type
    /// <typeparamref source="T"/> and mapping the constant value
    /// <paramref source="value"/> to the property named
    /// <paramref source="destination"/>.
    /// </returns>
    [Obsolete(
      "This method is obsolete. Use the Map method that receives a LINQ expression."
      , true)]
    public DataReaderMapperBuilder<T> Map(string destination, short value) {
      return Map(destination, TypeMaps.Short(value));
    }

    /// <summary>
    /// Maps the constant value <see cref="value"/> to the interface
    /// property <paramref source="destination"/>.
    /// </summary>
    /// <param name="value">
    /// The value that should be returned when by the interface property
    /// <paramref name="destination"/>.
    /// </param>
    /// <param name="destination">
    /// The source of the property that will be mapped to the value
    /// <paramref name="value"/>.
    /// </param>
    /// <returns>
    /// A <see cref="DataReaderMapperBuilder{T}"/> that builds an object of type
    /// <typeparamref source="T"/> and mapping the constant value
    /// <paramref source="value"/> to the property named
    /// <paramref source="destination"/>.
    /// </returns>
    [Obsolete(
      "This method is obsolete. Use the Map method that receives a LINQ expression."
      , true)]
    public DataReaderMapperBuilder<T> Map(string destination, long value) {
      return Map(destination, TypeMaps.Long(value));
    }

    /// <summary>
    /// Maps the constant value <see cref="value"/> to the interface
    /// property <paramref source="destination"/>.
    /// </summary>
    /// <param name="value">
    /// The value that should be returned when by the interface property
    /// <paramref name="destination"/>.
    /// </param>
    /// <param name="destination">
    /// The source of the property that will be mapped to the value
    /// <paramref name="value"/>.
    /// </param>
    /// <returns>
    /// A <see cref="DataReaderMapperBuilder{T}"/> that builds an object of type
    /// <typeparamref source="T"/> and mapping the constant value
    /// <paramref source="value"/> to the property named
    /// <paramref source="destination"/>.
    /// </returns>
    [Obsolete(
      "This method is obsolete. Use the Map method that receives a LINQ expression."
      , true)]
    public DataReaderMapperBuilder<T> Map(string destination, float value) {
      return Map(destination, TypeMaps.Float(value));
    }

    /// <summary>
    /// Maps the constant value <see cref="value"/> to the interface
    /// property <paramref source="destination"/>.
    /// </summary>
    /// <param name="value">
    /// The value that should be returned when by the interface property
    /// <paramref name="destination"/>.
    /// </param>
    /// <param name="destination">
    /// The source of the property that will be mapped to the value
    /// <paramref name="value"/>.
    /// </param>
    /// <returns>
    /// A <see cref="DataReaderMapperBuilder{T}"/> that builds an object of type
    /// <typeparamref source="T"/> and mapping the constant value
    /// <paramref source="value"/> to the property named
    /// <paramref source="destination"/>.
    /// </returns>
    [Obsolete(
      "This method is obsolete. Use the Map method that receives a LINQ expression."
      , true)]
    public DataReaderMapperBuilder<T> Map(string destination, decimal value) {
      return Map(destination, TypeMaps.Decimal(value));
    }
  }
}
