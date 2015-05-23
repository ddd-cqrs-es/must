using System;

namespace Nohros.Data
{
  /// <summary>
  /// Ignores the property, and throws a <see cref="NotImplementedException"/>
  /// when an attempt to get the value of the property is made.
  /// </summary>s
  public class IgnoreMapType : TypeMap
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="DoubleMapType"/> class
    /// using the specified column name.
    /// </summary>
    public IgnoreMapType()
      : base(null, TypeMapType.Ignore) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DoubleMapType"/> class
    /// using the specified column name.
    /// </summary>
    internal IgnoreMapType(object value) : base(value, TypeMapType.Ignore) {
    }
  }
}
