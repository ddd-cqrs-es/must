using System;

namespace Nohros.Data
{
  /// <summary>
  /// Factory class for implementations of the
  /// <see cref="IDataReaderMapper{T}"/> interface that maps to value types.
  /// </summary>
  public static class Mappers
  {
    /// <summary>
    /// Creates an instance of a class that implements the
    /// <see cref="IDataReaderMapper{T}"/> where T is <see cref="bool"/>.
    /// </summary>
    public static IDataReaderMapper<bool> Boolean() {
      return new BooleanDataReaderMapper();
    }

    /// <summary>
    /// Creates an instance of a class that implements the
    /// <see cref="IDataReaderMapper{T}"/> where T is <see cref="byte"/>.
    /// </summary>
    public static IDataReaderMapper<byte> Byte() {
      return new ByteDataReaderMapper();
    }

    /// <summary>
    /// Creates an instance of a class that implements the
    /// <see cref="IDataReaderMapper{T}"/> where T is <see cref="char"/>.
    /// </summary>
    public static IDataReaderMapper<char> Char() {
      return new CharDataReaderMapper();
    }

    /// <summary>
    /// Creates an instance of a class that implements the
    /// <see cref="IDataReaderMapper{T}"/> where T is <see cref="DateTime"/>.
    /// </summary>
    public static IDataReaderMapper<DateTime> DateTime() {
      return new DateTimeDataReaderMapper();
    }

    /// <summary>
    /// Creates an instance of a class that implements the
    /// <see cref="IDataReaderMapper{T}"/> where T is <see cref="decimal"/>.
    /// </summary>
    public static IDataReaderMapper<decimal> Decimal() {
      return new DecimalDataReaderMapper();
    }

    /// <summary>
    /// Creates an instance of a class that implements the
    /// <see cref="IDataReaderMapper{T}"/> where T is <see cref="double"/>.
    /// </summary>
    public static IDataReaderMapper<double> Double() {
      return new DoubleDataReaderMapper();
    }

    /// <summary>
    /// Creates an instance of a class that implements the
    /// <see cref="IDataReaderMapper{T}"/> where T is <see cref="float"/>.
    /// </summary>
    public static IDataReaderMapper<float> Float() {
      return new FloatDataReaderMapper();
    }

    /// <summary>
    /// Creates an instance of a class that implements the
    /// <see cref="IDataReaderMapper{T}"/> where T is <see cref="Guid"/>.
    /// </summary>
    public static IDataReaderMapper<Guid> Guid() {
      return new GuidDataReaderMapper();
    }

    /// <summary>
    /// Creates an instance of a class that implements the
    /// <see cref="IDataReaderMapper{T}"/> where T is <see cref="short"/>.
    /// </summary>
    public static IDataReaderMapper<short> Int16() {
      return new Int16DataReaderMapper();
    }

    /// <summary>
    /// Creates an instance of a class that implements the
    /// <see cref="IDataReaderMapper{T}"/> where T is <see cref="int"/>.
    /// </summary>
    public static IDataReaderMapper<int> Int32() {
      return new Int32DataReaderMapper();
    }

    /// <summary>
    /// Creates an instance of a class that implements the
    /// <see cref="IDataReaderMapper{T}"/> where T is <see cref="long"/>.
    /// </summary>
    public static IDataReaderMapper<long> Long() {
      return new LongDataReaderMapper();
    }
  }
}
