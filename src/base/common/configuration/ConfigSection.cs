using System;
using System.Configuration;

namespace Nohros.Configuration
{
  /// <summary>
  /// A helper class for <see cref="ConfigurationSection"/>.
  /// </summary>
  public sealed class ConfigSection
  {
    /// <summary>
    /// Gets a <see cref="ConfigurationSection"/> based on the given type.
    /// </summary>
    public static T GetConfiguration<T>() where T : class, new() {
      if (!typeof (ConfigurationSection).IsAssignableFrom(typeof (T))) {
        throw new ArgumentException(
          "Only supports .NET ConfigurationSections is supported");
      }
      return ConfigurationManager.GetSection(typeof(T).Name) as T;
    }
  }
}