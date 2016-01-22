using System;

namespace Nohros.Resources
{
  /// <summary>
  /// A coolection of common string resources.
  /// </summary>
  public sealed class StringResources
  {
    /// <summary>
    /// Looks up a localized string similar to [The {0} argument could not be
    /// an empty string].
    /// </summary>
    public static string Argument_EmptyString {
      get { return Resources.Argument_EmptyString; }
    }

    /// <summary>
    /// Looks up a localized string similar to [Missing required switch {0}].
    /// </summary>
    public static string Switches_MissingSwitch {
      get { return Resources.Switches_MissingSwitch; }
    }
    /// <summary>
    /// Looks up a localized string similar to [{0} cannot contain null
    /// elements.].
    /// </summary>
    public static string Argument_CollectionNoNulls {
      get { return Resources.Argument_CollectionNoNulls; }
    }

    /// <summary>
    /// Looks up a localized string similar to [Non-negative number required.].
    /// </summary>
    public static string ArgumentOutOfRange_NeedNonNegNum {
      get { return Resources.ArgumentOutOfRange_NeedNonNegNum; }
    }

    /// <summary>
    /// Looks up a localized string similar to [An entry with the same key
    /// already exists].
    /// </summary>
    public static string Argument_AddingDuplicate {
      get { return Resources.Argument_AddingDuplicate; }
    }

    /// <summary>
    /// Looks up a localized string similar to [An entry with the same key
    /// already exists. Key:{0}.].
    /// </summary>
    public static string Argument_AddingDuplicateKey {
      get { return Resources.Argument_AddingDuplicateKey; }
    }

    /// <summary>
    /// Looks up a localized string similar to [The {0} argument could not be
    /// an empty string or a sequence of spaces].
    /// </summary>
    public static string Argument_EmptyStringOrSpaceSequence {
      get { return Resources.Argument_EmptyStringOrSpaceSequence; }
    }

    /// <summary>
    /// Looks up a localized string similar to [An IDbConnection instance
    /// could not be created. Verify if your connection string and data source
    /// type were correctly specified.]
    /// </summary>
    public static string DataProvider_Connection {
      get { return Resources.DataProvider_Connection; }
    }

    public static string Argument_ArrayLengthMismatch {
      get { return Resources.Argument_ArrayLengthMismatch; }
    }

    /// <summary>
    /// Looks up a localized string similar to [Index was out of range. Must
    /// be non-negative and less than the size of the collection].
    /// </summary>
    public static string ArgumentOutOfRange_Index {
      get { return Resources.ArgumentOutOfRange_Index; }
    }

    /// <summary>
    /// Looks up a localized string similar to [Destination array is not long
    /// enough to copy all the items in the collection. Check array index and
    /// length].
    /// </summary>
    public static string Arg_ArrayPlusOffTooSmall {
      get { return Resources.Arg_ArrayPlusOffTooSmall; }
    }

    /// <summary>
    /// Looks up a localized string similar to [{0} is empty].
    /// </summary>
    public static string Argument_Empty {
      get { return Resources.Argument_Empty; }
    }

    /// <summary>
    /// Looks up a localized string similar to [The key {0} was not found].
    /// </summary>
    public static string Arg_KeyNotFound {
      get { return Resources.Arg_KeyNotFound; }
    }

    /// <summary>
    /// Looks up a localized string similar to [One of the specified arguments
    /// is a null reference].
    /// </summary>
    public static string ArgumentNull_Any {
      get { return Resources.ArgumentNull_Any; }
    }

    /// <summary>
    /// Looks up a localized string similar to [Offset and length were out of
    /// bounds for the array or count is greater than the number of elements
    /// from index to the end of the source collection].
    /// </summary>
    public static string Argument_InvalidOfLen {
      get { return Resources.Argument_InvalidOfLen; }
    }

    /// <summary>
    /// Looks up a localized string similar to [Index is outside the range of
    /// valid indexes for array].
    /// </summary>
    public static string Arg_IndexOutOfRange {
      get { return Resources.Arg_IndexOutOfRange; }
    }

    /// <summary>
    /// Looks up a localized string similar to [Not a valid value].
    /// </summary>
    public static string Arg_OutOfRange {
      get { return Resources.Arg_OutOfRange; }
    }

    /// <summary>
    /// Looks up a localized string similar to [The range of valid values are
    /// {0} to {1}].
    /// </summary>
    public static string Arg_RangeNotBetween {
      get { return Resources.Arg_RangeNotBetween; }
    }

    /// <summary>
    /// Looks up a localized string similar to [The "{0}" path must be
    /// relative to the application base directory.]
    /// </summary>
    public static string Arg_PathRooted {
      get { return Resources.Arg_PathRooted; }
    }

    /// <summary>
    /// Looks up a localized string similar to [The "{0}" path is not rooted.]
    /// </summary>
    public static string Arg_PathNotRooted {
      get { return Resources.Arg_PathNotRooted; }
    }

    /// <summary>
    /// Looks up a localized string similar to [The file {0} does not represent
    /// a valid configuration file.].
    /// </summary>
    public static string Configuration_InvalidFormat {
      get { return Resources.Configuration_InvalidFormat; }
    }

    /// <summary>
    /// Looks up a localized string similar to [Invalid cast from {0} to {1}.].
    /// </summary>
    public static string InvalidCast_FromTo {
      get { return Resources.InvalidCast_FromTo; }
    }

    /// <summary>
    /// Looks up a localized string similar to [Missing configuration
    /// information for {0} at {1}.].
    /// </summary>
    public static string Configuration_Missing {
      get { return Resources.Configuration_Missing; }
    }

    /// <summary>
    /// Looks up a localized string similar to [{0} is not a valid value
    /// for {1}].
    /// </summary>
    public static string Configuration_ArgOutOfRange {
      get { return Resources.Configuration_ArgOutOfRange; }
    }

    /// <summary>
    /// Looks up a localized string similar to [Queue empty].
    /// </summary>
    public static string InvalidOperation_EmptyQueue {
      get { return Resources.InvalidOperation_EmptyQueue; }
    }

    /// <summary>
    /// Looks up a localized string similar to [Queue full].
    /// </summary>
    public static string InvalidOperation_FullQueue {
      get { return Resources.InvalidOperation_FullQueue; }
    }

    /// <summary>
    /// Looks up a localized string similar to [The element {0} is already
    /// in collection].
    /// </summary>
    public static string Collection_AddingDuplicate {
      get { return Resources.Collection_AddingDuplicate; }
    }

    /// <summary>
    /// Looks up a localized string similar to [Cannot create an instance of
    /// the type {0}].
    /// </summary>
    public static string TypeLoad_CreateInstance {
      get { return Resources.TypeLoad_CreateInstance; }
    }

    /// <summary>
    /// Looks up a localized string similar to [{0} throws an exception.].
    /// </summary>
    public static string Log_ThrowsException {
      get { return Resources.Log_ThrowsException; }
    }

    /// <summary>
    /// Looks up a localized string similar to [The {0} method of {0} throws
    /// an exception.].
    /// </summary>
    public static string Log_MethodThrowsException {
      get { return Resources.Log_MethodThrowsException; }
    }

    /// <summary>
    /// Looks up a localized string similar to [The configuration node {0} is
    /// not defined.].
    /// </summary>
    public static string Configuration_MissingNode {
      get { return Resources.Configuration_MissingNode; }
    }

    /// <summary>
    /// Looks up a localized string similar to [Argument should be of type {0}.].
    /// </summary>
    public static string Arg_ArgumentWrongType {
      get { return string.Format(Arg_WrongType, "Argument"); }
    }

    /// <summary>
    /// Looks up a localized string similar to [{0} should be of type {1}.].
    /// </summary>
    public static string Arg_WrongType {
      get { return Resources.Arg_WrongType; }
    }

    /// <summary>
    /// Looks up a localized string similar to [The length of the array {0}
    /// should be equals to the length of {1}.].
    /// </summary>
    public static string Arg_ArrayLengthDifferFrom {
      get { return Resources.Arg_ArrayLengthDifferFrom; }
    }

    /// <summary>
    /// Looks up a localized string similar to [Array lengths must be the same.].
    /// </summary>
    public static string Arg_ArrayLengthsDiffer {
      get { return Resources.Arg_ArrayLengthsDiffer; }
    }

    /// <summary>
    /// Looks up a localized string similar to [This class cannot be used to
    /// create types related to {0}.].
    /// </summary>
    public static string NotSupported_CannotCreateType {
      get { return Resources.NotSupported_CannotCreateType; }
    }
  }
}
