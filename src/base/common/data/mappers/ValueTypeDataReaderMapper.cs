using System;
using System.Collections.Generic;
using System.Data;

namespace Nohros.Data
{
  /// <summary>
  /// A implementation of the <see cref="IDataReaderMapper{T}"/> for
  /// <see cref="System.Data.ValueType"/>
  /// </summary>
  /// <typeparam name="T">
  /// The type to be mapped.
  /// </typeparam>
  /// <remarks>
  /// <typeparamref name="T"/> should be a type that derives from
  /// <seealso cref="System.Data.ValueType"/>
  /// </remarks>
  internal abstract class ValueTypeDataReaderMapper<T> : DataReaderMapper<T>
  {
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="ValueTypeDataReaderMapper{T}"/> class.
    /// </summary>
    public ValueTypeDataReaderMapper() {
      if (!typeof (T).IsValueType) {
        throw new ArgumentException(Resources.Resources.Arg_ShouldBeValueType);
      }
    }

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
