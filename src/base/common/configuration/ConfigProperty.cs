using System;

namespace Nohros.Configuration
{
  /// <summary>
  /// An extension class for parsing configuration values to .NET types.
  /// </summary>
  public sealed class ConfigProperty
  {
    /// <summary>
    /// Cast the <paramref name="value"/> to the type <typeparamref name="T"/>
    /// without throwing exceptions.
    /// </summary>
    /// <typeparam name="T">
    /// The type to cast to.
    /// </typeparam>
    /// <param name="value">
    /// The value to cast.
    /// </param>
    /// <returns>
    /// The <paramref name="value"/> casted to the type <typeparamref name="T"/>
    /// or the default value for <typeparamref name="T"/> if the cast fails.
    /// </returns>
    public static T CastTo<T>(object value) {
      try {
        return (T) value;
      } catch (Exception) {
        return default(T);
      }
    }
  }
}
