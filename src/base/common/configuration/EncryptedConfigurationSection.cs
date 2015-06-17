using System;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace Nohros.Configuration
{
  /// <summary>
  /// Defines a <see cref="ConfigurationElement"/> that contains data encrypted
  /// with DPAPI for <see cref="DataProtectionScope.LocalMachine"/> scope.
  /// </summary>
  public class EncryptedConfigurationSection : ConfigurationSection
  {
    /// <summary>
    /// Gets a value indicating if the data is encrypted.
    /// </summary>
    /// <value>
    /// <c>true</c> if the element contains encrypted data; otherwise,
    /// <c>false</c>.
    /// </value>
    [ConfigurationProperty("Encrypted", IsRequired = false,
      DefaultValue = false)]
    public bool Encrypted {
      get { return (bool) this["Encrypted"]; }
      set { this["Encrypted"] = value; }
    }

    /// <summary>
    /// Decrypt the encrypted data.
    /// </summary>
    /// <param name="encrypted">
    /// The data to be decrypted.
    /// </param>
    /// <returns>
    /// The <paramref name="encrypted"/> data decrypted.
    /// </returns>
    /// <remarks>
    /// The data should be encrypted using the DPAPI engine for the
    /// <see cref="DataProtectionScope.LocalMachine"/> scope.
    /// </remarks>
    protected string Decrypt(string encrypted) {
      byte[] data = Convert.FromBase64String(encrypted);
      byte[] decrypted =
        ProtectedData
          .Unprotect(data, new byte[0], DataProtectionScope.LocalMachine);
      return Encoding.Unicode.GetString(decrypted);
    }
  }
}
