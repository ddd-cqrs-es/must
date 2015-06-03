using System;
using System.Linq;
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
      Type type_of_t = typeof (T);
      if (!typeof (ConfigurationSection).IsAssignableFrom(type_of_t)) {
        throw new ArgumentException(
          "Only supports .NET ConfigurationSections is supported");
      }
      var t = ConfigurationManager.GetSection(type_of_t.Name) as T;

      // If the section could not be found by name try to find it by type.
      if (t == (default(T))) {
        return ConfigurationManager
          .OpenExeConfiguration(ConfigurationUserLevel.None)
          .Sections
          .Cast<ConfigurationSection>()
          .FirstOrDefault(cfg => cfg is T) as T;
      }
      return t;
    }
  }
}
