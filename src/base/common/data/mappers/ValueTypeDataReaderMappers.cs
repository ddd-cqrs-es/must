using System;
using System.Data;

namespace Nohros.Data
{
  internal class BooleanDataReaderMapper : ValueTypeDataReaderMapper<bool>
  {
    internal override bool MapInternal(IDataReader reader) {
      return reader.GetBoolean(0);
    }
  }

  internal class ByteDataReaderMapper : ValueTypeDataReaderMapper<byte>
  {
    internal override byte MapInternal(IDataReader reader) {
      return reader.GetByte(0);
    }
  }

  internal class CharDataReaderMapper : ValueTypeDataReaderMapper<char>
  {
    internal override char MapInternal(IDataReader reader) {
      return reader.GetChar(0);
    }
  }

  internal class DateTimeDataReaderMapper : ValueTypeDataReaderMapper<DateTime>
  {
    internal override DateTime MapInternal(IDataReader reader) {
      return reader.GetDateTime(0);
    }
  }

  internal class DecimalDataReaderMapper : ValueTypeDataReaderMapper<decimal>
  {
    internal override decimal MapInternal(IDataReader reader) {
      return reader.GetDecimal(0);
    }
  }

  internal class DoubleDataReaderMapper : ValueTypeDataReaderMapper<double>
  {
    internal override double MapInternal(IDataReader reader) {
      return reader.GetDouble(0);
    }
  }

  internal class FloatDataReaderMapper : ValueTypeDataReaderMapper<float>
  {
    internal override float MapInternal(IDataReader reader) {
      return reader.GetFloat(0);
    }
  }

  internal class GuidDataReaderMapper : ValueTypeDataReaderMapper<Guid>
  {
    internal override Guid MapInternal(IDataReader reader) {
      return reader.GetGuid(0);
    }
  }

  internal class Int16DataReaderMapper : ValueTypeDataReaderMapper<short>
  {
    internal override short MapInternal(IDataReader reader) {
      return reader.GetInt16(0);
    }
  }

  internal class Int32DataReaderMapper : ValueTypeDataReaderMapper<int>
  {
    internal override int MapInternal(IDataReader reader) {
      return reader.GetInt32(0);
    }
  }

  internal class LongDataReaderMapper : ValueTypeDataReaderMapper<long>
  {
    internal override long MapInternal(IDataReader reader) {
      return reader.GetInt64(0);
    }
  }

  internal class StringDataReaderMapper : ValueTypeDataReaderMapper<string>
  {
    internal override string MapInternal(IDataReader reader) {
      return reader.GetString(0);
    }
  }
}
