using System.Configuration;
using Nohros.Extensions;

namespace Nohros.Configuration
{
  /// <summary>
  /// Represents a collection of <see cref="DataServerConfig"/> elements.
  /// </summary>
  public class DataServerConfigCollection : ConfigurationElementCollection
  {
    protected override ConfigurationElement CreateNewElement() {
      return new DataServerConfig();
    }

    protected override object GetElementKey(ConfigurationElement element) {
      var config = (DataServerConfig) element;
      return "{0}::{1}".Fmt(config.ServerId, config.ConnectionStringName);
    }
  }
}
