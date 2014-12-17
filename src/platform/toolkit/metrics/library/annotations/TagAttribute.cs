﻿using System;

namespace Nohros.Metrics.Annotations
{
  /// <summary>
  /// Represents a tags that is associated with a field of a class.
  /// </summary>
  /// <remarks>
  /// The tags will be queried when the instance is registered with the
  /// <see cref="AppMetrics.Register(object)"/>
  /// </remarks>
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field,
    AllowMultiple = true)]
  public class TagAttribute : Attribute
  {
    readonly Tag tag_;

    /// <summary>
    /// Initializes a new instance of the <see cref="TagAttribute"/> by
    /// using the given tag's name and value.
    /// </summary>
    /// <param name="name">
    /// The tag's name. The name of the tag should be unique for a given
    /// metric.
    /// </param>
    /// <param name="value">
    /// The tag's value.
    /// </param>
    public TagAttribute(string name, string value) {
      tag_ = new Tag(name, value);
    }

    /// <summary>
    /// The <see cref="Tag"/> object that was created by using the speficied
    /// tag's name and value.
    /// </summary>
    public Tag Tag {
      get { return tag_; }
    }
  }
}
