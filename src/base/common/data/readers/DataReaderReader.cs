using System;
using System.Data;
using Nohros.Resources;

namespace Nohros.Data
{
  /// <summary>
  /// Represents a reader that can read data from a <see cref="IDataReader"/>.
  /// </summary>
  [Obsolete("This class is obsolete. Use the FieldNameLookup class.")]
  public class DataReaderReader
  {
    readonly IDataReader reader_;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataReaderReader"/> class
    /// using the specified <see cref="IDataReader"/>.
    /// </summary>
    /// <param name="reader">
    /// The <see cref="IDataReader"/> to be readed.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="reader"/> is <c>null</c>.
    /// </exception>
    public DataReaderReader(IDataReader reader) {
      if (reader == null) {
        throw new ArgumentNullException("reader");
      }
      reader_ = reader;
    }

    /// <summary>
    /// Advances the reader to the next record.
    /// </summary>
    /// <returns>
    /// <c>true</c> if there ae more rows; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// The default position of the reader is prior the first record. Therefore
    /// you must call <see cref="Read"/> to begin accessing any data.
    /// </remarks>
    public bool Read() {
      return reader_.Read();
    }

    /// <summary>
    /// Advances the reader to the next result, when reading the results of
    /// a <see cref="IDataReader"/> associated with a batch of statements.
    /// </summary>
    /// <returns>
    /// <c>true</c> if there are more rows; otherwise, <c>false</c>.
    /// </returns>
    public bool NextResult() {
      return reader_.NextResult();
    }

    /// <summary>
    /// Gets the columns indexes(ordinals), given the name of the columns.
    /// </summary>
    /// <param name="columns">
    /// An array containing the column names to get the ordinals.
    /// </param>
    /// <returns>
    /// An array containing the indexes of the specified column
    /// names within a <see cref="IDataReader"/>. The indexes is
    /// returned in the same order they appear on column names array.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// The number of fields of the associated <see cref="IDataReader"/> is
    /// less than the number of columns in <paramref name="columns"/>.
    /// </exception>
    /// <remarks>
    /// Ordinal-based lookups are more efficient than name lookups, it
    /// is inefficient to call <see cref="IDataReader.GetOrdinal"/> within a
    /// loop. This method provides a convenient way to call the
    /// <see cref="IDataReader.GetOrdinal"/> method for a set of columns
    /// defined within a <see cref="IDataReader"/> and reduce the
    /// code len used for that purpose.
    /// </remarks>
    public int[] GetOrdinals(params string[] columns) {
      var ordinals = new int[columns.Length];
      int j = columns.Length;
      if (reader_.FieldCount < j) {
        throw new ArgumentException(
          StringResources.Argument_ArrayLengthMismatch);
      }
      for (int i = 0; i < j; i++) {
        ordinals[i] = reader_.GetOrdinal(columns[i]);
      }
      return ordinals;
    }
  }
}
