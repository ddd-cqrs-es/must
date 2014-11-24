using System;
using Nohros.Extensions;

namespace Nohros.Metrics
{
  /// <summary>
  /// A <see cref="Tag"/> that contain informations about a
  /// <see cref="MetricType"/>.
  /// </summary>
  public static class MetricTypeTag
  {
    const string kDefaultName = "type";

    /// <summary>
    /// Creates a <see cref="Tag"/> by using  the string "nohros.metrics.type"
    /// as the tag name and the given <paramref name="type"/> as the tag value.
    /// </summary>
    /// <param name="type">
    /// A <see cref="MetricType"/> that defines the value of the tag.
    /// </param>
    public static Tag AsTag(this MetricType type) {
      return AsTag(type, kDefaultName);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MetricTypeTag"/> by using 
    /// the given <paramref name="name"/> as the tag name and the
    /// <paramref name="type"/> as the tag value.
    /// </summary>
    /// <param name="type">
    /// A <see cref="MetricType"/> that defines the value of the tag.
    /// </param>
    /// <param name="name">
    /// The name of the tag.
    /// </param>
    public static Tag AsTag(this MetricType type, string name) {
      switch (type) {
        case MetricType.Counter:
          return new Tag(name, "counter");
        case MetricType.Gauge:
          return new Tag(name, "gauge");
        case MetricType.Rate:
          return new Tag(name, "rate");
          case MetricType.EWMA:
          return new Tag(name, "ewma");
        default:
          throw new ArgumentOutOfRangeException(
            Resources.ArgIsInvalid.Fmt((int) type, typeof (MetricType).Name));
      }
    }
  }
}
