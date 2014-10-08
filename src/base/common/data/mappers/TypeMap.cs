using System;
using System.Linq.Expressions;

namespace Nohros.Data
{
  /// <summary>
  /// Provides a base implementation for the interface <see cref="ITypeMap"/>
  /// to reduce the effort required to implemente that interface.
  /// </summary>
  public abstract class TypeMap : ITypeMap
  {
    readonly TypeMapType map_type_;
    readonly object value_;

    #region .ctor
    protected TypeMap(object value, TypeMapType map_type) {
      value_ = value;
      map_type_ = map_type;
    }
    #endregion

    public object Value {
      get { return value_; }
    }

    public TypeMapType MapType {
      get { return map_type_; }
    }
  }
}
