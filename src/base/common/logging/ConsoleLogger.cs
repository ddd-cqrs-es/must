using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Nohros.Logging
{
  /// <summary>
  /// A generic logger that logs messages to the console.
  /// </summary>
  /// <remarks>
  /// This is a generic logger that loads automatically and configures
  /// itself through the code. The messages are logged to the console window.
  /// <para>
  /// The pattern used to log message are:
  /// . "[%date %-5level/%thread] %message%newline %exception" for the
  /// non-debug messages.
  /// </para>
  /// <para>
  /// The default threshold level is INFO and could be overloaded on the
  /// nohros configuration file.
  /// </para>
  /// </remarks>
  public sealed class ConsoleLogger
  {
    static ILogger current_process_logger_;

    #region .ctor
    /// <summary>
    /// Initializes the singleton process's logger instance.
    /// </summary>
    static ConsoleLogger() {
      Log4NetConsoleLogger logger = new Log4NetConsoleLogger();
      logger.Configure();

      current_process_logger_ = logger as ILogger;
    }
    #endregion

    /// <summary>
    /// Gets the current process logger.
    /// </summary>
    public static ILogger ForCurrentProcess {
      get { return current_process_logger_; }
    }
  }
}
