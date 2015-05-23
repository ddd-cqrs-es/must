using System;

namespace Nohros.Data
{
  /// <summary>
  /// Provides a base implementation for the interface <see cref="ITypeMap"/>
  /// to reduce the effort required to implemente that interface.
  /// </summary>
  public class TypeMap : ITypeMap
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="TypeMap"/> class by
    /// using the given <paramref name="value"/> and <paramref name="map_type"/>
    /// </summary>
    /// <param name="value">
    /// The value associated with the type.
    /// </param>
    /// <param name="map_type">
    /// A <see cref="TypeMapType"/> object that describes how
    /// <paramref name="value"/> should be treated.
    /// </param>
    protected internal TypeMap(object value, TypeMapType map_type)
      : this(value, map_type, false) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TypeMap"/> class by
    /// using the given <paramref name="value"/>,
    /// <paramref name="map_type"/> and <paramref name="optional"/> flag.
    /// </summary>
    /// <param name="value">
    /// The value associated with the type.
    /// </param>
    /// <param name="map_type">
    /// A <see cref="TypeMapType"/> object that describes how
    /// <paramref name="value"/> should be treated.
    /// </param>
    /// <param name="optional">
    /// A flag that indicates if the presence of the column associated
    /// with the type is optional.
    /// </param>
    protected internal TypeMap(object value, TypeMapType map_type, bool optional) {
      Value = value;
      MapType = map_type;
      Optional = optional;
    }

    /// <inheritdoc/>
    public object Value { get; private set; }

    /// <inheritdoc/>
    public TypeMapType MapType { get; private set; }

    /// <inheritdoc/>
    public bool Optional { get; private set; }
  }
}
