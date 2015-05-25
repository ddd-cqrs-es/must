using System;
using Nohros.Collections;

namespace Nohros
{
  /// <summary>
  /// The real type of the object that derivates from the <see cref="Value"/>
  /// class.
  /// </summary>
  public enum ValueType
  {
    /// <summary>
    /// A null reference.
    /// </summary>
    NullValue = 0,

    /// <summary>
    /// Types used to store boolean values, true and false.
    /// </summary>
    Boolean = 1,

    /// <summary>
    /// Types used to store integral types.
    /// </summary>
    Integer = 2,

    /// <summary>
    /// Types used to store doubles types.
    /// </summary>
    Real = 3,

    /// <summary>
    /// Types used to store strings.
    /// </summary>
    String = 4,

    /// <summary>
    /// A <see cref="DictionaryValue"/> type.
    /// </summary>
    Dictionary = 5,

    /// <summary>
    /// A <see cref="ListValue"/> type.
    /// </summary>
    List = 6,

    /// <summary>
    /// A <see cref="GenericValue{T}"/> type.
    /// </summary>
    Generic = 7,

    /// <summary>
    /// A custom type that implements the <see cref="IValue"/> interface.
    /// </summary>
    Class = 8,

    /// <summary>
    /// A <see cref="DictionaryValue"/> type.
    /// </summary>
    GenericDictionary = 9
  }
}
