using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;

namespace Nohros.Data
{
  public class FieldNameLookup
  {
    readonly string[] fields_;
    readonly CompareInfo compare_info_;
    readonly Dictionary<string, int> lookup_;

    public FieldNameLookup(IDataReader reader) {
      int field_count = reader.FieldCount;
      fields_ = new string[field_count];
      lookup_ = new Dictionary<string, int>();

      for (int i = 0; i < field_count; i++) {
        string name = reader.GetName(i);
        fields_[i] = name;
        lookup_[name] = i;
      }

      compare_info_ = CultureInfo.InvariantCulture.CompareInfo;
    }

    public int GetOrdinal(string name) {
      if (name == null) {
        throw new ArgumentNullException("name");
      }
      int index = IndexOf(name);
      if (index == -1) {
        throw new IndexOutOfRangeException(name);
      }
      return index;
    }

    public int IndexOf(string name) {
      int index;
      if (lookup_.TryGetValue(name, out index)) {
        return index;
      }
      index = LinearIndexOf(name, CompareOptions.IgnoreCase);
      if (index == -1) {
        return LinearIndexOf(name,
          CompareOptions.IgnoreWidth | CompareOptions.IgnoreKanaType |
            CompareOptions.IgnoreCase);
      }
      return index;
    }

    int LinearIndexOf(string name, CompareOptions compare_options) {
      int length = fields_.Length;
      for (int i = 0; i < length; i++) {
        if (compare_info_.Compare(name, fields_[i], compare_options) == 0) {
          lookup_[name] = i;
          return i;
        }
      }
      return -1;
    }
  }
}
