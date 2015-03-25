using System;
using System.Collections.Generic;
using System.Linq;

namespace Nohros.Extensions
{
  /// <summary>
  /// Extension methods used to create instances of a <see cref="Type"/>
  /// at runtime.
  /// </summary>
  public static class RuntimeTypeFactoryExtensions
  {
    /// <summary>
    /// Applies the given action to all the types that can be assigned
    /// to <typeparamref name="T" />.
    /// </summary>
    public static void ForAllTypes<T>(this IEnumerable<Type> types,
      Action<Type> action) where T : class {
      IEnumerable<Type> filtered_types =
        types
          .Where(
            t =>
              typeof (T).IsAssignableFrom(t) && !(t.IsAbstract || t.IsInterface));
      foreach (var type in filtered_types) {
        action(type);
      }
    }

    /// <summary>
    /// Creates new instances to all types that can be assigned
    /// to <typeparamref name="T" />.
    /// the types.
    /// </summary>
    /// <param name="types">
    /// A list of <see cref="Type"/> objects containing the type to be
    /// filtered and instantiated.
    /// </param>
    /// <param name="args">
    /// An array of arguments that match the parameters of the constructor to
    /// invoke. If args is an empty array or null, or if a matching constructor
    /// is not found the constructor that takes no parameters(the default
    /// constructor) is invoked.
    /// </param>
    /// <returns>
    /// An collection of <typeparamref name="T"/> containing an instance for
    /// each type of the <paramref name="types"/> list that can be assigned to
    /// <typeparamref name="T" />.
    /// </returns>
    public static IEnumerable<T> CreateInstances<T>(
      this IEnumerable<Type> types,
      params object[] args) where T : class {
      return CreateInstances<T>(types, true, true, args);
    }

    /// <summary>
    /// Creates new instances to all types that can be assigned
    /// to <typeparamref name="T" />.
    /// the types.
    /// </summary>
    /// <param name="types">
    /// A list of <see cref="Type"/> objects containing the type to be
    /// filtered and instantiated.
    /// </param>
    /// <param name="fallback">
    /// A value that indicates if we should fallback to the default constructor
    /// when a constructor that accepts the given <paramref name="args"/> as
    /// parameters is not found. Default to <c>true</c>.
    /// </param>
    /// <param name="throw">
    /// A value that indicates if exceptions should be propagated to the
    /// caller or silently ignored. Default to <c>true</c>.
    /// </param>
    /// <param name="args">
    /// An array of arguments that match the parameters of the constructor to
    /// invoke. If args is an empty array or null, or if a matching constructor
    /// is not found the constructor that takes no parameters(the default
    /// constructor) is invoked.
    /// </param>
    /// <returns>
    /// An collection of <typeparamref name="T"/> containing an instance for
    /// each type of the <paramref name="types"/> list that can be assigned to
    /// <typeparamref name="T" />.
    /// </returns>
    public static IEnumerable<T> CreateInstances<T>(
      this IEnumerable<Type> types, bool fallback, bool @throw,
      params object[] args) where T : class {
      return
        types
          .Where(
            t =>
              typeof (T).IsAssignableFrom(t) && !(t.IsAbstract || t.IsInterface))
          .Select(t =>
            RuntimeTypeFactory<T>
              .CreateInstance(t, fallback, @throw, args));
    }
  }
}
