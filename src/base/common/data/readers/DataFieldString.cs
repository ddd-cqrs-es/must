﻿using System;
using System.Data;

namespace Nohros.Data
{
  /// <summary>
  /// An implementation of the <see cref="IDataField{T}"/> that maps a
  /// <see cref="String"/> data type to a concrete <see cref="IDataField{T}"/>
  /// object.
  /// </summary>
  public class DataFieldString: DataField<string>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="DataFieldString"/> class by
    /// using the specified field name and position.
    /// </summary>
    /// <param name="name">
    /// THe name of the field.
    /// </param>
    /// <param name="position">
    /// The zero based ordinal position of the field within an
    /// <see cref="IDataReader"/>.
    /// </param>
    public DataFieldString(string name, int position)
      : base(name, position) {
    }

    /// <summary>
    /// Gets the value of the field as a string
    /// </summary>
    /// <param name="reader">
    /// A <see cref="IDataReader"/> that can be used to extract a string
    /// at the field position.
    /// </param>
    public override string GetValue(IDataReader reader) {
      return reader.GetString(position);
    }
  }
}
