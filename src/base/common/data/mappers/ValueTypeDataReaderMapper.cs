using System;
using System.Collections.Generic;
using System.Data;

namespace Nohros.Data
{
  internal abstract class ValueTypeDataReaderMapper<T> : DataReaderMapper<T>
  {
    /// <inheritdoc/>
    public override T Map(IDataReader reader) {
      T t;
      if (!Map(reader, out t)) {
        throw new NoResultException();
      }
      return t;
    }

    /// <inheritdoc/>
    public override bool Map(IDataReader reader, out T t) {
      return Map(reader, true, out t);
    }

    /// <inheritdoc/>
    public override T MapCurrent(IDataReader reader) {
      throw new NotSupportedException(
        "This class should be used only to map result sets with a single column.");
    }

    /// <inheritdoc/>
    public override IEnumerable<T> Map(IDataReader reader, bool defer,
      Action<T> post_map) {
      var enumerable = new Enumerator(reader, this, post_map);
      if (defer) {
        return enumerable;
      }
      return new List<T>(enumerable);
    }

    /// <inheritdoc/>
    internal override void GetOrdinals(IDataReader reader) {
      throw new NotSupportedException(
        "This class should be used only to map result sets with a single column. The ordinal of a single column result set will be always zero.");
    }

    /// <inheritdoc/>
    internal abstract override T MapInternal(IDataReader reader);
  }
}
