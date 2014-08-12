using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Nohros.Metrics
{
  /// <summary>
  /// Represents a collection of <see cref="Tag"/>.
  /// </summary>
  /// <seealso cref="Tag"/>
  public class Tags : IEnumerable<Tag>
  {
    public class Builder
    {
      internal readonly List<Tag> tags_;

      /// <summary>
      /// Initializes a new instance of the <see cref="Builder"/> class whose
      /// tags and name values are copied from the given
      /// <paramref name="config"/> object.
      /// </summary>
      /// <param name="config">
      /// A <see cref="MetricConfig"/> whose tags and values are copied to the
      /// <see cref="Builder"/>.
      /// </param>
      public Builder(MetricConfig config) {
        tags_ = new List<Tag>(config.Tags);
      }

      /// <summary>
      /// Adds a tag to the config.
      /// </summary>
      /// <param name="name">
      /// The tag's name
      /// </param>
      /// <param name="value">
      /// The tag's value.
      /// </param>
      public Builder WithTag(string name, string value) {
        tags_.Add(new Tag(name, value));
        return this;
      }

      /// <summary>
      /// Adds a tag to the config.
      /// </summary>
      /// <param name="tag">
      /// The tag to be added.
      /// </param>
      public Builder WithTag(Tag tag) {
        tags_.Add(tag);
        return this;
      }

      /// <summary>
      /// Adds a tag to the config.
      /// </summary>
      /// <param name="tags">
      /// The tag to be added.
      /// </param>
      public Builder WithTags(IEnumerable<Tag> tags) {
        if (tags != null) {
          tags_.AddRange(tags);
        }
        return this;
      }

      public Tags Build() {
        return new Tags(tags_);
      }
    }

    readonly HashSet<Tag> tags_;

    /// <summary>
    /// Initializes a new instance of the <see cref="Tags"/> that contains no
    /// tags.
    /// </summary>
    public Tags() {
      tags_ = new HashSet<Tag>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Tags"/> class that
    /// contians elements copied from the specified collection of tags.
    /// </summary>
    /// <param name="tag">
    /// The single and unique tag of the collection.
    /// </param>
    public Tags(Tag tag) : this() {
      tags_.Add(tag);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Tags"/> class that
    /// contians elements copied from the specified collection of tags.
    /// </summary>
    /// <param name="tags">
    /// The collection whose elements are copied to the <see cref="Tags"/>
    /// collection.
    /// </param>
    public Tags(IEnumerable<Tag> tags) : this() {
      foreach (var tag in tags) {
        tags_.Add(tag);
      }
    }

    /// <summary>
    /// Determines whether a <see cref="Tags"/> object and the specified
    /// collection contain the same elements.
    /// </summary>
    /// <param name="other">
    /// The collection to compare with the current <see cref="Tags"/> object.
    /// </param>
    /// <returns>
    /// <c>true</c> if the specified collection contain the same elements as
    /// the current <see cref="Tags"/> object.
    /// </returns>
    public bool EqualsTo(IEnumerable<Tag> other) {
      return tags_.SetEquals(other);
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }

    /// <inheritdoc/>
    public IEnumerator<Tag> GetEnumerator() {
      return tags_.GetEnumerator();
    }

    /// <summary>
    /// Gets a <see cref="Tags"/> object that contains no tags.
    /// </summary>
    public static Tags Empty {
      get { return new Tags(Enumerable.Empty<Tag>()); }
    }
  }
}
