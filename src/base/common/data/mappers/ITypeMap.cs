using System;
using System.Linq.Expressions;
using Nohros.Data;

namespace Nohros.Data
{
  public interface ITypeMap
  {
    /// <summary>
    /// Gets the value that is used on the map operation.
    /// </summary>
    object Value { get; }

    TypeMapType MapType { get; }
  }
}
