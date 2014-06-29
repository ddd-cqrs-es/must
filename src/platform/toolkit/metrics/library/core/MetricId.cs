using System;
using System.Collections.Generic;

namespace Nohros.Metrics
{
  /// <summary>
  /// A metric id with the ability to include semantic tags.
  /// </summary>
  /// <remarks>
  /// Tags allow the natural grouping of similar metrics, wich give users the
  /// ability to ealisy correlate freely between the metrics that incorporate
  /// them.
  /// 
  /// For example, take the following two metrics
  /// 
  ///  * nohros.com.interface.traffic.eth0.in
  ///  * nohros.com.interface.traffic.eth0.out
  /// 
  /// Tags simplifies that representation, these would be represented as:
  /// 
  ///  * interface.traffic {host=nohros.com, interface=eth0, direction=in}
  ///  * interface.traffic {host=nohros.com, interface=eth0, direction=out}
  /// </remarks>
  public class MetricId
  {
    readonly IDictionary<string, string> tags_;
    readonly int hashcode_;

    /// <summary>
    /// Initializes a new instance of the <see cref="MetricId"/> class by
    /// using the given metric's id.
    /// </summary>
    /// <param name="name">
    /// A string that could be used to identify the metric.
    /// </param>
    public MetricId(string name) : this(name, new Dictionary<string, string>()) {
    }

    /// <summary>
    /// Initializes a new instance o the <see cref="MetricId"/> class by
    /// using the given metric id and associated tags.
    /// </summary>
    /// <param name="name">
    /// A string that can be used to identify a metric.
    /// </param>
    /// <param name="tags">
    /// A collection of key value pairs that can be used to distinguish two
    /// metrics witht the same id.
    /// </param>
    public MetricId(string name, IEnumerable<KeyValuePair<string, string>> tags) {
      Name = name;

      tags_ = new Dictionary<string, string>();
      foreach (var tag in tags) {
        tags_.Add(tag);
      }

      hashcode_ = ComputeHashCode();
    }

    /// <summary>
    /// A string that, associated with <see cref="Tags"/>, uniquely identifies
    /// a metric.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// A collection of key/value pairs that semantically identifies a
    /// metric.
    /// </summary>
    public IEnumerable<KeyValuePair<string, string>> Tags {
      get { return tags_; }
    }

    public override bool Equals(object obj) {
      if (obj == null) {
        return false;
      }

      return Equals(obj as MetricId);
    }

    public bool Equals(MetricId obj) {
      if ((object) obj == null) {
        return false;
      }

      if (obj.Name == Name) {
        foreach (var tag in tags_) {
          string obj_tag;
          if (!(obj.tags_.TryGetValue(tag.Key, out obj_tag)
            && tag.Value == obj_tag)) {
            return false;
          }
        }
        return true;
      }
      return false;
    }

    int ComputeHashCode() {
      unchecked {
        int hash = 17;
        hash = hash*23 + tags_.GetHashCode();
        hash = hash*23 + Name.GetHashCode();
        return hash;
      }
    }

    public override int GetHashCode() {
      return hashcode_;
    }
  }
}
