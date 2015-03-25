using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Nohros.Configuration
{
  public static class AssemblyScannerResultsExtensions
  {
    /// <summary>
    /// Applies the given action to all the scanned types that can be assigned
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
    /// Applies the given action to all the scanned types that can be assigned
    /// to <typeparamref name="T" />.
    /// </summary>
    public static void ForAllTypes<T>(this AssemblyScannerResults results,
      Action<Type> action) where T : class {
      results.Types.ForAllTypes<T>(action);
    }
  }

  /// <summary>
  /// Holds <see cref="AssemblyScanner.GetScannableAssemblies"/> results.
  /// Contains list of errors and list of scannable assemblies.
  /// </summary>
  public class AssemblyScannerResults
  {
    /// <summary>
    /// Constructor to initialize AssemblyScannerResults
    /// </summary>
    public AssemblyScannerResults() {
      Assemblies = new List<Assembly>();
      Types = new List<Type>();
      SkippedFiles = new List<SkippedFile>();
    }

    internal void RemoveDuplicates() {
      Assemblies = Assemblies.Distinct().ToList();
      Types = Types.Distinct().ToList();
    }

    /// <summary>
    /// List of successfully found and loaded assemblies
    /// </summary>
    public List<Assembly> Assemblies { get; private set; }

    /// <summary>
    /// List of files that were skipped while scanning because they were:
    /// <para>
    /// a) explicitly excluded by the user
    /// </para>
    /// <para>
    /// b) not a .NET DLL
    /// </para>
    /// </summary>
    public List<SkippedFile> SkippedFiles { get; private set; }

    /// <summary>
    /// True if errors where encountered during assembly scanning
    /// </summary>
    public bool ErrorsThrownDuringScanning { get; internal set; }

    /// <summary>
    /// List of types.
    /// </summary>
    public List<Type> Types { get; private set; }
  }
}
