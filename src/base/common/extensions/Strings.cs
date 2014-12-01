using System;
using System.Globalization;
using System.Text;

namespace Nohros.Extensions
{
  /// <summary>
  /// A collection of useful extensions for the <see cref="string"/> class.
  /// </summary>
  public static class StringExtensions
  {
    /// <summary>
    /// Compares two string objects by evaluating the numeric values of the
    /// corresponding <see cref="Char"/> objects in each string.
    /// </summary>
    /// <param name="str">
    /// The first string.
    /// </param>
    /// <param name="comparand">
    /// The string to compare with the first.
    /// </param>
    /// <returns>
    /// <c>true</c> if <paramref name="str"/> is equals to
    /// <paramref name="comparand"/>; otherwise, <c>false</c>.
    /// </returns>
    public static bool CompareOrdinal(this string str, string comparand) {
      return string.Compare(str, comparand, StringComparison.Ordinal) == 0;
    }

    /// <summary>
    /// Compares two string objects by evaluating the numeric values of the
    /// corresponding <see cref="Char"/> objects in each string ignoring
    /// their case.
    /// </summary>
    /// <param name="str">
    /// The first string.
    /// </param>
    /// <param name="comparand">
    /// The string to compare with the first.
    /// </param>
    /// <returns>
    /// <c>true</c> if <paramref name="str"/> is equals to
    /// <paramref name="comparand"/>; otherwise, <c>false</c>.
    /// </returns>
    public static bool CompareOrdinalIgnoreCase(this String str,
      string comparand) {
      return
        string.Compare(str, comparand, StringComparison.OrdinalIgnoreCase) == 0;
    }

    /// <summary>
    /// Compares two string objects by evaluating the numeric values of the
    /// corresponding <see cref="Char"/> objects in each string ignoring
    /// their case and using the current culture.
    /// </summary>
    /// <param name="str">
    /// The first string.
    /// </param>
    /// <param name="comparand">
    /// The string to compare with the first.
    /// </param>
    /// <returns>
    /// <c>true</c> if <paramref name="str"/> is equals to
    /// <paramref name="comparand"/>; otherwise, <c>false</c>.
    /// </returns>
    public static bool CompareCurrentCultureIgnoreCase(this string str,
      string comparand) {
      return string.
        Compare(str, comparand, StringComparison.CurrentCultureIgnoreCase) == 0;
    }

    /// <summary>
    /// Compare two strings, ignoring or honoring their case.
    /// </summary>
    /// <param name="str">
    /// The first string.
    /// </param>
    /// <param name="comparand">
    /// The string to compare with the first.
    /// </param>
    /// <param name="ignore_case">
    /// A boolean value indicating a case-sensitive or insensitive comparison(
    /// <c>true</c> indicates a case-insensitive comparison.)
    /// </param>
    /// <returns>
    /// <c>true</c> if <paramref name="str"/> is equals to
    /// <paramref name="comparand"/>; otherwise, <c>false</c>.
    /// </returns>
    public static bool Compare(this string str, string comparand,
      bool ignore_case) {
      return string.Compare(str, comparand, ignore_case) == 0;
    }

    /// <summary>
    /// Compares two string objects by evaluating the numeric values of the
    /// corresponding <see cref="Char"/> objects in each string, ignoring or
    /// honoring their case.
    /// </summary>
    /// <param name="str">
    /// The first string.
    /// </param>
    /// <param name="comparand">
    /// The string to compare with the first.
    /// </param>
    /// <param name="ignore_case">
    /// A boolean value indicating a case-sensitive or insensitive comparison(
    /// <c>true</c> indicates a case-insensitive comparison.)
    /// </param>
    /// <returns>
    /// <c>true</c> if <paramref name="str"/> is equals to
    /// <paramref name="comparand"/>; otherwise, <c>false</c>.
    /// </returns>
    public static bool CompareOrdinal(this string str, string comparand,
      bool ignore_case) {
      return string.Compare(str, comparand, ignore_case) == 0;
    }

    /// <summary>
    /// Compares two strings using a parameter that specifies whether the
    /// comparison uses the current or invariant culture, honors or ignores
    /// case, and uses word or ordina sort rules.
    /// </summary>
    /// <param name="str">
    /// The first string.
    /// </param>
    /// <param name="comparand">
    /// The string to compare with the first.
    /// </param>
    /// <param name="comparison">
    /// One of the <see cref="StringComparison"/> values that indicates the
    /// type of comparison to perform.
    /// </param>
    /// <returns>
    /// <c>true</c> if <paramref name="str"/> is equals to
    /// <paramref name="comparand"/>; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// The <paramref name="comparison"/> parameter indicates whether the
    /// comparison should use the current or invariant culture, honor or
    /// ignore the case of the comparands, or use work(culture-sensitive) or
    /// ordinal (culture-insensitive) sort rules.
    /// <para>
    /// This method performs the same operation as the
    /// <see cref="string.Compare(string,string)"/>.
    /// </para>
    /// </remarks>
    /// <seealso cref="string.Compare(string,string)"/>
    public static bool Compare(this string str, string comparand,
      StringComparison comparison) {
      return string.Compare(str, comparand, comparison) == 0;
    }

    public static string Fmt(this string str, object arg0) {
      return string.Format(str, arg0);
    }

    public static string Fmt(this string str, object arg0, object arg1) {
      return string.Format(str, arg0, arg1);
    }

    public static string Fmt(this string str, object arg0, object arg1,
      object arg2) {
      return string.Format(str, arg0, arg1, arg2);
    }

    public static string Fmt(this string str, params object[] args) {
      return string.Format(str, args);
    }

    public static bool IsNullOrEmpty(this string str) {
      return string.IsNullOrEmpty(str);
    }

    public static string RemoveDiacritics(this string str) {
      string normalized_string = str.Normalize(NormalizationForm.FormD);
      var builder = new StringBuilder(str.Length);
      for (int i = 0, j = normalized_string.Length; i < j; i++) {
        UnicodeCategory category =
          CharUnicodeInfo.GetUnicodeCategory(normalized_string[i]);
        if (category != UnicodeCategory.NonSpacingMark) {
          builder.Append(normalized_string[i]);
        }
      }
      return builder.ToString().Normalize(NormalizationForm.FormC);
    }

    /// <summary>
    /// Encodes the given string using the base64 digits.
    /// </summary>
    /// <param name="str">
    /// The string to be encoded.
    /// </param>
    /// <param name="encoding">
    /// The character encoding of string <paramref name="str"/>.
    /// </param>
    /// <returns>
    /// The <paramref name="str"/> string encoded using the base64 digits
    /// </returns>
    public static string AsBase64(this string str, Encoding encoding) {
      return Convert.ToBase64String(encoding.GetBytes(str));
    }

    /// <summary>
    /// Gets the original version of a string that was encoded using the
    /// base64 digits.
    /// </summary>
    /// <param name="str">
    /// The base64 encoded version of the string to get the original version.
    /// </param>
    /// <param name="encoding">
    /// The character encoding of the original string.
    /// </param>
    /// <returns></returns>
    public static string FromBase64String(this string str, Encoding encoding) {
      return encoding.GetString(Convert.FromBase64String(str));
    }
  }
}
