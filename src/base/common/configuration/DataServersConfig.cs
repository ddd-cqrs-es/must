using System.Configuration;

namespace Nohros.Configuration
{
  /// <summary>
  /// Represents the main section of the <see cref="DataServerConfig"/>
  /// configurations.
  /// </summary>
  public class DataServersConfig : ConfigurationSection
  {
    /// <summary>
    /// Gets or sets the collection of configured <see cref="DataServersConfig"/>
    /// </summary>
    [ConfigurationProperty("", IsRequired = false, IsKey = false,
      IsDefaultCollection = true)]
    public virtual DataServerConfigCollection Servers {
      get { return this[""] as DataServerConfigCollection; }
      set { this[""] = value; }
    }
  }
}
