using System;

namespace Nohros.Data
{
  public interface ITypeMap
  {
    /// <summary>
    /// Gets the value that is used on the map operation.
    /// </summary>
    object Value { get; }

    /// <summary>
    /// Gets a <see cref="TypeMapType"/> that defines how the
    /// <see cref="Value"/> should be interpreted.
    /// </summary>
    TypeMapType MapType { get; }

    /// <summary>
    /// Gets a valus that indicates if the presence of the column associated
    /// with the type is optional.
    /// </summary>
    bool Optional { get; }
  }
}
