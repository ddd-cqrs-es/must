using System;
using System.Data;
using System.Linq.Expressions;

namespace Nohros.Data
{
  /// <summary>
  /// Maps a class property to a value of a column from a
  /// <see cref="IDataReader"/>.
  /// </summary>
  internal class StringTypeMap : TypeMap
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="StringTypeMap"/> class
    /// using the specified column name.
    /// </summary>
    /// <param name="value">
    /// The name of the column to use as data source for the map operation.
    /// </param>
    public StringTypeMap(string value) : this(value, null, false) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StringTypeMap"/> class
    /// using the specified column name.
    /// </summary>
    /// <param name="value">
    /// The name of the column to use as data source for the map operation.
    /// </param>
    /// <param name="optional">
    /// A flag that indicates if the presence of the column associated
    /// with the type is optional.
    /// </param>
    public StringTypeMap(string value, bool optional)
      : this(value, null, optional) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StringTypeMap"/> class
    /// using the specified column name.
    /// </summary>
    /// <param name="value">
    /// The name of the column to use as data source for the map operation.
    /// </param>
    /// <param name="raw_type">
    /// The type of the column to be mapped.
    /// </param>
    /// <remarks>
    /// If the type of the column to be mapped is not equals to the type of
    /// the mapped field you should set the <paramref name="raw_type"/> to
    /// the type of the columun. This way the mapper can perform the type
    /// conversion and use the right Get method of the <see cref="IDataReader"/>.
    /// </remarks>
    public StringTypeMap(string value, Type raw_type)
      : this(value, raw_type, false) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StringTypeMap"/> class
    /// using the specified column name.
    /// </summary>
    /// <param name="value">
    /// The name of the column to use as data source for the map operation.
    /// </param>
    /// <param name="raw_type">
    /// The type of the column to be mapped.
    /// </param>
    /// <param name="optional">
    /// A flag that indicates if the presence of the column associated
    /// with the type is optional.
    /// </param>
    /// <remarks>
    /// If the type of the column to be mapped is not equals to the type of
    /// the mapped field you should set the <paramref name="raw_type"/> to
    /// the type of the columun. This way the mapper can perform the type
    /// conversion and use the right Get method of the <see cref="IDataReader"/>.
    /// </remarks>
    public StringTypeMap(string value, Type raw_type, bool optional)
      : base(value, TypeMapType.String, optional) {
      RawType = raw_type;
    }

    /// <summary>
    /// Implicit converts a string to a <see cref="StringTypeMap"/>.
    /// </summary>
    /// <param name="str">
    /// The string to be converted.
    /// </param>
    /// <returns>
    /// A <see cref="StringTypeMap"/> object representing the string
    /// <paramref name="str"/>.
    /// </returns>
    public static implicit operator StringTypeMap(string str) {
      return new StringTypeMap(str);
    }

    /// <summary>
    /// Gets or sets the type of the underlying column data.
    /// </summary>
    public Type RawType { get; set; }

    /// <summary>
    /// Gets or sets a expression that should be used to convert the value
    /// returned from the database to another type.
    /// </summary>
    public LambdaExpression Conversor { get; set; }
  }
}
