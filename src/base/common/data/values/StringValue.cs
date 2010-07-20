using System;
using System.Collections.Generic;
using System.Text;

namespace Nohros.Data
{
    /// <summary>
    /// Represents a string value
    /// </summary>
    public class StringValue : Value
    {
        string value_;

        /// <summary>
        /// Initializes a new instance of the StringValue class.
        /// </summary>
        public StringValue(string in_value)
            : base(ValueType.TYPE_DICTIONARY)
        {
            value_ = in_value;
        }

        public override bool GetAsString(out string out_value)
        {
            out_value = value_;
            return (out_value != null);
        }

        public Value DeepCopy()
        {
            return CreateStringValue(value_);
        }

        public override bool Equals(Value other)
        {
            string lhs, rhs;

            if (other.Type != Type)
                return false;

            return GetAsString(out lhs) && other.GetAsString(out rhs) && lhs == rhs;
        }
    }
}