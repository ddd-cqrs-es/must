using System;

namespace Nohros.Metrics.Annotations
{
  /// <summary>
  /// An attribute for marking a method as timed
  /// </summary>
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
  public class TimedAttribute : Attribute
  {
    public TimedAttribute(string name) {
      Name = name;
    }

    public string Name { get; private set; }
  }
}
