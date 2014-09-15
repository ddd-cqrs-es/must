using System;

namespace Nohros
{
  /// <summary>
  /// Extension method for <see cref="ComparisonOperator"/> enum.
  /// </summary>
  public static class ComparisonOperatorExtensions
  {
    /// <summary>
    /// Gets the symbol of a <see cref="ComparisonOperator"/>.
    /// </summary>
    /// <param name="op">
    /// The <see cref="ComparisonOperator"/> to get the symbol.
    /// </param>
    /// <returns>
    /// The symbol of a <see cref="ComparisonOperator"/>.
    /// </returns>
    public static string Symbol(this ComparisonOperator op) {
      switch (op) {
        case ComparisonOperator.Equals:
          return "=";
        case ComparisonOperator.GreaterThan:
          return ">";
        case ComparisonOperator.LessThan:
          return "<";
        default:
          throw new ArgumentOutOfRangeException("op");
      }
    }
  }

  /// <summary>
  /// Describes the operation that should be performed.
  /// </summary>
  public enum ComparisonOperator
  {
    /// <summary>
    /// A equals comparison, such as (a == b)
    /// </summary>
    Equals = 0,

    /// <summary>
    /// A less than comparison, such as (a < b)
    /// </summary>
    LessThan = 1,

    /// <summary>
    /// A greater than comparison, such as (a > b)
    /// </summary>
    GreaterThan = 2,
  }
}
