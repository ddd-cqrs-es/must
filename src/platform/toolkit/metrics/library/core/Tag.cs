using System.Collections.Generic;

namespace Nohros.Metrics
{
  public struct Tag
  {
    internal Tag(KeyValuePair<string, string> tag) : this() {
      Name = tag.Key;
      Value = tag.Value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Tag"/> class by using the
    /// given tag name and value.
    /// </summary>
    /// <param name="name">
    /// The tag's name.
    /// </param>
    /// <param name="value">
    /// The tag's value.
    /// </param>
    public Tag(string name, string value) : this() {
      Name = name;
      Value = value;
    }

    /// <summary>
    /// Gets or sets the name of the tag.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the value of the tag.
    /// </summary>
    public string Value { get; set; }
  }
}
