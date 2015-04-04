using System;
using System.Collections.Generic;
using System.Linq;

namespace Nohros.Logging
{
  /// <summary>
  /// A implementation of the <see cref="ChainedLogger"/> that uses the
  /// nohros must framework.
  /// </summary>
  /// <remarks>
  /// This class uses the nohros must framework and is the only point where
  /// this dependency exists. Clients should call the
  /// <see cref="ForCurrentProcess"/> method to obtain an instance of the
  /// <see cref="ChainedLogger"/> class, and uses it to log messages.
  /// <para>
  /// By default the <see cref="NOPLogger"/> is returned by the
  /// <see cref="ForCurrentProcess"/> method. The application must configure
  /// the correct logger on the app initialization.
  /// </para>
  /// </remarks>
  public class ChainedLogger : ILogger
  {
    readonly List<ILogger> loggers_;
    readonly ILogger main_logger_;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChainedLogger"/> class
    /// by using the specified logger.
    /// </summary>
    /// <param name="loggers">
    /// A list of <see cref="ILogger"/> to be chainable.
    /// </param>
    /// <remarks>
    /// The first logger of the chain will dictate the behavior of the methods
    /// used to check if a particular level is enabled or not. Note that the
    /// first logger dictates onle the level of the <see cref="ChainedLogger"/>
    /// class, each logger uses it own level to perform logging
    /// operations.
    /// </remarks>
    public ChainedLogger(IEnumerable<ILogger> loggers) {
      if (loggers == null) {
        throw new ArgumentException("loggers");
      }
      loggers_ = new List<ILogger>(loggers);
      main_logger_ = loggers_.First();
    }

    /// <inherit />
    public bool IsDebugEnabled {
      get { return main_logger_.IsDebugEnabled; }
    }

    /// <inherit />
    public bool IsErrorEnabled {
      get { return main_logger_.IsErrorEnabled; }
    }

    /// <inherit />
    public bool IsFatalEnabled {
      get { return main_logger_.IsFatalEnabled; }
    }

    /// <inherit />
    public bool IsInfoEnabled {
      get { return main_logger_.IsInfoEnabled; }
    }

    /// <inherit />
    public bool IsWarnEnabled {
      get { return main_logger_.IsWarnEnabled; }
    }

    /// <inherit />
    public bool IsTraceEnabled {
      get { return main_logger_.IsTraceEnabled; }
    }

    /// <inherit />
    public void Debug(string message) {
      Log(message, (msg, logger) => logger.Debug(msg));
    }

    /// <inherit />
    public void Debug(string message, Exception exception) {
      LogWithException(message, exception,
        (msg, logger, e) => logger.Debug(msg, e));
    }

    /// <inherit />
    public void Error(string message) {
      Log(message, (msg, logger) => logger.Error(msg));
    }

    /// <inherit />
    public void Error(string message, Exception exception) {
      LogWithException(message, exception,
        (msg, logger, e) => logger.Error(msg, e));
    }

    /// <inherit />
    public void Fatal(string message) {
      Log(message, (msg, logger) => logger.Fatal(msg));
    }

    /// <inherit />
    public void Fatal(string message, Exception exception) {
      LogWithException(message, exception,
        (msg, logger, e) => logger.Fatal(msg, e));
    }

    /// <inherit />
    public void Info(string message) {
      Log(message,
        (msg, logger) => logger.Info(msg));
    }

    /// <inherit />
    public void Info(string message, Exception exception) {
      LogWithException(message, exception,
        (msg, logger, e) => logger.Info(msg, e));
    }

    /// <inherit />
    public void Warn(string message) {
      Log(message, (msg, logger) => logger.Warn(msg));
    }

    /// <inherit />
    public void Warn(string message, Exception exception) {
      LogWithException(message, exception,
        (msg, logger, e) => logger.Warn(msg, e));
    }

    void Log(string message, Action<string, ILogger> action) {
      foreach (ILogger logger in loggers_) {
        var l = logger;
        action(message, l);
      }
    }

    void LogWithException(string message, Exception exception,
      Action<string, ILogger, Exception> action) {
      foreach (ILogger logger in loggers_) {
        var l = logger;
        action(message, l, exception);
      }
    }
  }
}
