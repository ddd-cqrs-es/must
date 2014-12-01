using System;

namespace Nohros.Metrics.Annotations
{
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
  public class TagAttribute : Attribute
  {
    readonly Tag tag_;

    public TagAttribute(string name, string value) {
      tag_ = new Tag(name, value);
    }

    public Tag Tag {
      get { return tag_; }
    }
  }
}
