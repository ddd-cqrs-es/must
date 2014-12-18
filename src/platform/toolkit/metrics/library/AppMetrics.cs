using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using Nohros.Extensions;
using Nohros.Metrics.Annotations;

namespace Nohros.Metrics
{
  /// <summary>
  /// A default registry that delegates all actions to a class specified by
  /// the key "DefaultMetricsRegistry" in the
  /// <see cref="ConfigurationManager.AppSettings"/> configuration section. The
  /// specified registry class must have a constructor with no arguments. If
  /// the property is not specified or the class cannot be loaded an instance
  /// of the <see cref="MetricsRegistry"/> will be used.
  /// </summary>
  public class AppMetrics
  {
    const string kDefaultMetricsRegistryKey = "DefaultMetricsRegistry";

    static AppMetrics() {
      string default_metrics_registry_class =
        ConfigurationManager.AppSettings[kDefaultMetricsRegistryKey];

      if (default_metrics_registry_class != null) {
        var runtime_type = new RuntimeType(default_metrics_registry_class);

        Type type = RuntimeType.GetSystemType(runtime_type);

        if (type != null) {
          ForCurrentProcess =
            RuntimeTypeFactory<IMetricsRegistry>
              .CreateInstanceFallback(runtime_type);
        }
      }

      if (ForCurrentProcess == null) {
        ForCurrentProcess = new MetricsRegistry();
      }
    }

    /// <summary>
    /// Register the given metric in the default registry.
    /// </summary>
    public static void Register(IMetric metric) {
      ForCurrentProcess.Register(metric);
    }

    /// <summary>
    /// Register the given metrics in the default registry.
    /// </summary>
    public static void Register(IEnumerable<IMetric> metrics) {
      foreach (var metric in metrics) {
        Register(metric);
      }
    }

    /// <summary>
    /// Unregister the given metrics from the default registry.
    /// </summary>
    public static void Unregister(IMetric metric) {
      ForCurrentProcess.Unregister(metric);
    }

    /// <summary>
    /// Register a <see cref="ICompositeMetric"/> that is a composite for all
    /// metric fields and annotated attributes of a given object.
    /// </summary>
    /// <remarks>
    /// Object to search for metrics on. All fields of type
    /// <see cref="IMetric"/> and fields/methods with a
    /// <see cref="MetricAttribute"/> attribute will be extracted and
    /// returned using <see cref="ICompositeMetric.Metrics"/>
    /// <para>
    /// Note that the <see cref="RegisterObject(object)"/>  will use
    /// reflection to add all instances of <see cref="IMetric"/> that have
    /// been declared, and also add a tag with the value set to class simple
    /// name (<see cref="Type.Name"/>).
    /// </para>
    /// </remarks>
    /// <returns>
    /// A <see cref="ICompositeMetric"/> based on the fields of the class of
    /// <paramref name="obj"/>.
    /// </returns>
    public static ICompositeMetric RegisterObject(object obj) {
      // The tags defined at the class level should be merged with the tags
      // defined for the fields and methods of the class.
      Type klass = obj.GetType();
      Tags tags =
        new Tags.Builder()
          .WithTags(GetTags(klass, true)) // static class tags
          .WithTags(GetTagsList(obj)) // dynamic class tags
          .WithTag("class", klass.Name)
          .Build();

      var metrics = new List<IMetric>();
      for (Type type = klass; type != null; type = type.BaseType) {
        AddMetrics(metrics, tags, obj, type);
      }

      var config = new MetricConfig("annotated", tags);
      var composite = new BasicCompositeMetric(config, metrics);
      Register((IMetric) composite);

      return composite;
    }

    /// <summary>
    /// Extract all fields of <paramref name="obj"/> that are of type
    /// <see cref="IMetric"/> and add them to <paramref name="metrics"/>.
    /// </summary>
    /// <param name="metrics">
    /// A <see cref="List{T}"/> object to add the extracted metrics.
    /// </param>
    /// <param name="tags">
    /// A <see cref="Tags"/> object contained a list of tags that should be
    /// added to the extracted metrics.
    /// </param>
    /// <param name="obj">
    /// The object to extract the monitor fields.
    /// </param>
    /// <param name="type">
    /// The type to extract the fields. This type is one of the types of
    /// the <paramref name="obj"/> hierarchy.
    /// </param>
    static void AddMetrics(List<IMetric> metrics, Tags tags, object obj,
      Type type) {
      IEnumerable<FieldInfo> metric_fields =
        GetFields(type)
          .Where(IsMetricType);

      foreach (FieldInfo field in metric_fields) {
        var metric = field.GetValue(obj) as IMetric;
        if (metric == null) {
          throw new ArgumentNullException(Resources.NullAnnotatedField.Fmt());
        }
        metrics.Add(Wrap(metric, tags, field));
      }
    }

    static IMetric Wrap(IMetric metric, Tags tags, FieldInfo field) {
      var field_tags =
        new Tags.Builder(tags)
          .WithTags(tags)
          .WithTags(metric.Config.Tags)
          .WithTags(GetTags(field))
          .Build();

      var config = new MetricConfig(metric.Config.Name, field_tags);
      return new MetricWrapper(config, metric);
    }

    static IEnumerable<Tag> GetTagsList(object obj) {
      FieldInfo field =
        GetFields(obj.GetType())
          .Where(IsTagList)
          .FirstOrDefault();

      if (field != null) {
        return (IEnumerable<Tag>) field.GetValue(obj);
      }

      MethodInfo method =
        GetMethods(obj.GetType())
          .Where(IsTagList)
          .FirstOrDefault();

      if (method != null) {
        return (IEnumerable<Tag>) method.Invoke(obj, new object[0]);
      }
      return Enumerable.Empty<Tag>();
    }

    static bool IsMetricType(FieldInfo field) {
      return typeof (IMetric).IsAssignableFrom(field.FieldType);
    }

    static bool IsTagList(FieldInfo field) {
      return IsTagList(field.FieldType);
    }

    static bool IsTagList(MethodInfo method) {
      return IsTagList(method.ReturnType);
    }

    static bool IsTagList(Type type) {
      return typeof (IEnumerable<Tag>).IsAssignableFrom(type);
    }

    static IEnumerable<FieldInfo> GetFields(Type type) {
      const BindingFlags kFlags =
        BindingFlags.Instance |
          BindingFlags.Static |
          BindingFlags.Public |
          BindingFlags.NonPublic |
          BindingFlags.DeclaredOnly;

      return type.GetFields(kFlags);
    }

    static IEnumerable<MethodInfo> GetMethods(Type type) {
      const BindingFlags kFlags =
        BindingFlags.Instance |
          BindingFlags.Static |
          BindingFlags.Public |
          BindingFlags.NonPublic |
          BindingFlags.DeclaredOnly;

      return type.GetMethods(kFlags);
    }

    /// <summary>
    /// Gets the tags declared for the given member.
    /// </summary>
    /// <param name="member">
    /// The type to get the tags.
    /// </param>
    /// <param name="inherit">
    /// <c>true</c> to search this member's inheritance chain to find the
    /// tags; otherwise, <c>false</c>. This parameter is ignored for
    /// fields.
    /// </param>
    /// <returns>
    /// A <see cref="Tags"/> object containing the tags declared for the
    /// given member.
    /// </returns>
    static IEnumerable<Tag> GetTags(MemberInfo member, bool inherit = false) {
      return
        GetAttributes<TagAttribute>(member, inherit)
          .Select(t => t.Tag);
    }

    static IEnumerable<T> GetAttributes<T>(MemberInfo member,
      bool inherit = false) {
      return
        member
          .GetCustomAttributes(typeof (T), inherit)
          .Cast<T>();
    }

    /// <summary>
    /// Gets the default configured <see cref="IMetricsRegistry"/>.
    /// </summary>
    public static IMetricsRegistry ForCurrentProcess { get; private set; }
  }
}
