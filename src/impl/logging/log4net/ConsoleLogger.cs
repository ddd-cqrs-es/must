using System;
using System.Reflection;
using log4net;
using log4net.Core;
using log4net.Appender;
using log4net.Layout;
using log4net.Repository;
using log4net.Repository.Hierarchy;

namespace Nohros.Logging.log4net
{
  /// <summary>
  /// A generic logger that uses the third party log4net logging library.
  /// </summary>
  /// <remarks>
  /// This is a generic logger that loads automatically and configures itself
  /// through the code. The messages are logged to the console window.
  /// <para>
  /// The pattern used to log message are:
  ///   . "[%date %-5level/%thread] %message%newline %exception" for the
  ///   non-debug messages.
  /// </para>
  /// <para>
  /// The default threshold level is INFO and could be overloaded on the
  /// nohros configuration file.
  /// </para>
  /// </remarks>
  public class ConsoleLogger : AbstractLogger
  {
    readonly string layout_pattern_;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsoleLogger"/> by
    /// using the default layout pattern.
    /// </summary>
    /// <see cref="AbstractLogger.kDefaultLogMessagePattern"/>
    public ConsoleLogger() : this(kDefaultLogMessagePattern) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsoleLogger"/> class by
    /// using the specified layout pattern.
    /// </summary>
    /// <param name="layout_pattern">
    /// A string that identifies the pattern to be used to format the output
    /// log message.
    /// </param>
    public ConsoleLogger(string layout_pattern) {
      layout_pattern_ = layout_pattern;
    }

    /// <summary>
    /// Configures the <see cref="FileLogger"/> logger adding the appenders
    /// to the root repository.
    /// </summary>
    public void Configure() {
      // create a new logger into the repository of the current assembly.
      ILoggerRepository root_repository =
        LogManager.GetRepository(Assembly.GetExecutingAssembly());

      Logger nohros_console_logger =
        root_repository
          .GetLogger("NohrosConsoleLogger") as Logger;

      // create the layout and appender for on error messages.
      var layout = new PatternLayout {ConversionPattern = layout_pattern_};
      layout.ActivateOptions();

      // create the appender
      var appender = new ConsoleAppender {
        Name = "NohrosCommonConsoleAppender",
        Layout = layout,
        Target = "Console.Out",
        Threshold = Level.All
      };
      appender.ActivateOptions();

      nohros_console_logger.Parent.AddAppender(appender);

      root_repository.Configured = true;

      logger = LogManager.GetLogger("NohrosConsoleLogger");
    }
  }
}
