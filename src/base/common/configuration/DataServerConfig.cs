using System;
using System.Configuration;

namespace Nohros.Configuration
{
  /// <summary>
  /// Defines the configuration for a data server.
  /// </summary>
  public class DataServerConfig : ConfigurationElement
  {
    /// <summary>
    /// Gets or set the name of the data sever.
    /// </summary>
    [ConfigurationProperty("Name", IsRequired = true)]
    public string Name {
      get { return this["Name"] as string; }
      set { this["Name"] = value; }
    }

    /// <summary>
    /// Gets or set the name of the connection string that is associated with
    /// the data server.
    /// </summary>
    [ConfigurationProperty("ConnectionStringName", IsRequired = true)]
    public string ConnectionStringName {
      get { return this["ConnectionStringName"] as string; }
      set { this["ConnectionStringName"] = value; }
    }

    /// <summary>
    /// Gets or set a <see cref="Guid"/> that uniquely identifies the server.
    /// </summary>
    [ConfigurationProperty("ServerId", IsRequired = true)]
    public Guid ServerId {
      get { return (Guid) this["ServerId"]; }
      set { this["ServerId"] = value; }
    }

    /// <summary>
    /// Gets or sets a value that indicates if the connections to the data
    /// server should supress the ambient transaction context.
    /// </summary>
    [ConfigurationProperty("SupressDTC", IsRequired = false,
      DefaultValue = false)]
    public bool SupressDTC {
      get { return ConfigProperty.CastTo<bool>(this["SupressDTC"]); }
      set { this["SupressDTC"] = value; }
    }

    /// <summary>
    /// Gets a value indicating if the connection string is encrypted.
    /// </summary>
    /// <value>
    /// <c>true</c> if the associated connection string is encrypted.
    /// </value>
    [ConfigurationProperty("Encrypted", IsRequired = false,
      DefaultValue = false)]
    public bool Encrypted {
      get { return (bool)this["Encrypted"]; }
      set { this["Encrypted"] = value; }
    }
  }
}
