using System;

namespace Nohros.Logging
{
  /// <summary>
  /// A implementation of the <see cref="ILogger"/> used for internal library
  /// logging.
  /// </summary>
  /// <remarks>
  /// This class uses the nohros must framework and is the only point where
  /// this dependency exists. Clients should call the
  /// <see cref="ForCurrentProcess"/> method to obtain an instance of the
  /// <see cref="ILogger"/> class, and uses it to log messages.
  /// <para>
  /// By default the <see cref="NOPLogger"/> is returned by the
  /// <see cref="ForCurrentProcess"/> method. The client should configure
  /// the correct logger on the app initialization.
  /// </para>
  /// </remarks>
  public class MustLogger : ForwardingLogger
  {
    static MustLogger() {
      ForCurrentProcess = new MustLogger(new NOPLogger());
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MustLogger"/>
    /// class by using the specified <see cref="ILogger"/> object.
    /// </summary>
    public MustLogger(ILogger logger) : base(logger) {
    }

    /// <summary>
    /// Gets the current configured application logger.
    /// </summary>
    public static MustLogger ForCurrentProcess { get; set; }
  }
}
