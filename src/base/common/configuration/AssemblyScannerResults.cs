using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Nohros.Configuration
{
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
