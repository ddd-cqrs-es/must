using System;
using System.Collections.Generic;
using System.Text;

namespace $rootnamespace$
{
  /// <summary>
  /// A simple logging interface abstracting logging APIs.
  /// </summary>
  /// <remarks>
  /// <para>
  /// A logger is tipically a class that implements the singleton pattern and
  /// is usually used like this:
  /// <code>
  ///   Logger.Instance.Error("message")
  /// </code>
  /// </para>
  /// <para>
  /// Calling the logger as the code above creates a dependency relationship
  /// between the caller and the external logger class. To minmize the number
  /// of points where a dependency is created this logging interface was
  /// created. External dependencies is resumed to an single application
  /// class that implements this interface. To log something the application
  /// can now use an internal application class.
  /// <code>
  ///   ILogger logger =
  ///     LoggerThatRouteCallsToExternalLogger.Instance.Log("message")
  /// </code>
  /// </para>
  /// <para>
  /// If something change on the logger or if the client does not want to use
  /// the logger anymore, it could simple implements this interface without
  /// break the code or have to rewrite all the logger calls.
  /// </para>
  /// <para>
  /// This interface is very simple and does not expose some advanced features
  /// that some logging libraries provides. For example, log4net provides
  /// methods that format the logging message based on some arguments, this
  /// interface does not declare such methods, because this could be done by
  /// the client code and could conflict with libraries that do not implements
  /// this type of functionality.
  /// </para>
  /// <para>
  /// The internal logging library that was used by the this project could
  /// change and projects that depends on this library will stop working.
  /// This interface provides an abstraction around the logging API and
  /// garantee that changes on the underlying logging library does not affect
  /// the application.
  /// </para>
  /// </remarks>
  interface $safeitemname$
  {
    #region IsEnabled
    /// <summary>
    /// Checks if this logger is enabled for the
    /// <see cref="Nohros.Logging.LogLevel.Debug"/>
    /// level.
    /// </summary>
    /// <remarks>
    /// This function is intended to lessen the computational cost of disabled
    /// log debug statements.
    /// For some ILog interface log, when you write:
    /// <code>
    ///   log.Debug("This is entry number: " + i );
    /// </code>
    /// You incur the cost constructing the message, string construction and
    /// concatenation in this case, regardless of whether the message is logged
    /// or not. If you are worried about speed (who isn't), then you should
    /// write:
    /// <code>
    /// if (log.IsDebugEnabled) {
    ///     log.Debug("This is entry number: " + i );
    /// }
    /// </code>
    /// This way you will not incur the cost of parameter construction if
    /// debugging is disabled for log. On the other hand, if the log is debug
    /// enabled, you will incur the cost of evaluating whether the logger is
    /// debug enabled twice. Once in <see cref="IsDebugEnabled"/> and once in
    /// the <see cref="Debug(string)"/>. This is an insignificant overhead
    /// since evaluating a logger takes about 1% of the time it takes to
    /// actually log. This is the preferred style of logging. Alternatively if
    /// your logger is available statically then the is debug enabled state can
    /// be stored in a static variable like this:
    /// <code>
    /// private static readonly bool isDebugEnabled = log.IsDebugEnabled;
    /// </code>
    /// Then when you come to log you can write:
    /// <code>
    ///     if (isDebugEnabled) {
    ///         log.Debug("This is entry number: " + i );
    ///     }
    /// </code>
    /// This way the debug enabled state is only queried once when the class is
    /// loaded. Using a private static readonly variable is the most efficient
    /// because it is a run time constant and can be heavily optimized by the
    /// JIT compiler. Of course if you use a static readonly variable to hold
    /// the enabled state of the logger then you cannot change the enabled
    /// state at runtime to vary the logging that is produced. You have to
    /// decide if you need absolute speed or runtime flexibility.
    /// </remarks>
    bool IsDebugEnabled { get; }

    /// <summary>
    /// Checks if this logger is enabled for the
    /// <see cref="Nohros.Logging.LogLevel.Error"/>.
    /// </summary>
    /// <seealso cref="IsDebugEnabled" />
    bool IsErrorEnabled { get; }

    /// <summary>
    /// Checks if this logger is enabled for the
    /// <see cref="Nohros.Logging.LogLevel.Fatal"/> level.
    /// </summary>
    /// <seealso cref="IsDebugEnabled" />
    bool IsFatalEnabled { get; }

    /// <summary>
    /// Checks if this logger is enabled for the
    /// <see cref="Nohros.Logging.LogLevel.Info"/> level.
    /// </summary>
    /// <seealso cref="IsDebugEnabled" />
    bool IsInfoEnabled { get; }

    /// <summary>
    /// Checks if this logger is enabled for the
    /// <see cref="Nohros.Logging.LogLevel.Warn"/> level.
    /// </summary>
    /// <seealso cref="IsDebugEnabled" />
    bool IsWarnEnabled { get; }

    /// <summary>
    /// Checks if this logger is enabled for the
    /// <see cref="Nohros.Logging.LogLevel.Trace"/> level.
    /// </summary>
    /// <seealso cref="IsDebugEnabled" />
    bool IsTraceEnabled { get; }

    #endregion

    /// <summary>
    /// Log a message object with the <see cref="LogLevel.Debug"/> log level.
    /// </summary>
    /// <param name="message">The message object to log.</param>
    /// <remarks>
    /// This method first checks if this logger is DEBUG enabled by comparing
    /// the level of the logger with the <see cref="LogLevel.Debug"/> level
    /// and if it is enabled the message is logged.
    /// </remarks>
    void Debug(string message);

    /// <summary>
    /// Log a message object with the
    /// <see cref="Nohros.Logging.LogLevel.Debug"/> level including the stack
    /// trace of the <see cref="Exception "/> passed as a parameter.
    /// </summary>
    /// <param name="message">The message object to log.</param>
    /// <param name="exception">The exception to log, including its stack
    /// trace.</param>
    /// <seealso cref="Debug(string)"/>
    /// <remarks>
    /// This method first checks if this logger is DEBUG enabled by comparing
    /// the level of the logger with the
    /// <see cref="Nohros.Logging.LogLevel.Debug"/> level and if it is enabled
    /// the message is logged.
    /// </remarks>
    void Debug(string message, Exception exception);

    /// <summary>
    /// Logs a message object with the
    /// <seealso cref="Nohros.Logging.LogLevel.Error"/> level.
    /// </summary>
    /// <param name="message">The message object to log.</param>
    /// <remarks>
    /// This method first checks if this logger is ERROR enabled by comparing
    /// the level of the logger with the
    /// <see cref="Nohros.Logging.LogLevel.Error"/> level and if it is enabled
    /// the message is logged.
    /// </remarks>
    void Error(string message);

    /// <summary>
    /// Log a message object with the
    /// <see cref="Nohros.Logging.LogLevel.Error"/> level including the stack
    /// trace of the System.Exception passed as a parameter.
    /// </summary>
    /// <param name="message">The message object to log.</param>
    /// <param name="exception">The exception to log, including its stack
    /// trace.</param>
    /// <seealso cref="Error(string)"/>
    /// <remarks>
    /// This method first checks if this logger is ERROR enabled by comparing
    /// the level of the logger with the
    /// <see cref="Nohros.Logging.LogLevel.Error"/> level and if it is enabled
    /// the message is logged.
    /// </remarks>
    void Error(string message, Exception exception);

    /// <summary>
    /// Log a message object with the
    /// <see cref="Nohros.Logging.LogLevel.Fatal"/> level.
    /// </summary>
    /// <param name="message">The message object to log.</param>
    /// <remarks>
    /// This method first checks if this logger is FATAL enabled by comparing
    /// the level of the logger with the
    /// <see cref="Nohros.Logging.LogLevel.Fatal"/> level and if it is enabled
    /// the message is logged.
    /// </remarks>
    void Fatal(string message);

    /// <summary>
    /// Log a message object with the
    /// <see cref="Nohros.Logging.LogLevel.Fatal"/> level including the stack
    /// trace of the <see cref="Exception"/> passed as a parameter.
    /// </summary>
    /// <param name="message">The message object to log.</param>
    /// <param name="exception">The exception to log, including its stack
    /// trace.</param>
    /// <remarks>
    /// This method first checks if this logger is ERROR enabled by comparing
    /// the level of the logger with the
    /// <see cref="Nohros.Logging.LogLevel.Fatal"/> level and if it is enabled
    /// the message is logged.
    /// </remarks>
    /// <seealso cref="Fatal(string)"/>
    void Fatal(string message, Exception exception);

    /// <summary>
    /// Logs a message object with the
    /// <see cref="Nohros.Logging.LogLevel.Info"/> level.
    /// </summary>
    /// <param name="message">The message object to log.</param>
    /// <remarks>
    /// This method first checks if this logger is INFO enabled by comparing
    /// the level of the logger with the
    /// <see cref="Nohros.Logging.LogLevel.Info"/> level and if it is enabled
    /// the message is logged.
    /// </remarks>
    void Info(string message);

    /// <summary>
    /// Logs a message object with the
    /// <see cref="Nohros.Logging.LogLevel.Info"/> level including the stack
    /// trace of the <see cref="Exception"/> passed as a parameter.
    /// </summary>
    /// <param name="message">The message object to log.</param>
    /// <param name="exception">The exception to log, including its stack
    /// trace.</param>
    /// <seealso cref="Info(string)"/>
    /// <remarks>
    /// This method first checks if this logger is INFO enabled by comparing
    /// the level of the logger with the
    /// <see cref="Nohros.Logging.LogLevel.Info"/> level and if it is enabled
    /// the message is logged.
    /// </remarks>
    void Info(string message, Exception exception);

    /// <summary>
    /// Log a message object with the<see cref="Nohros.Logging.LogLevel.Warn"/>
    /// level.
    /// </summary>
    /// <param name="message">The message object to log.</param>
    /// <remarks>
    /// This method first checks if this logger is WARN enabled by comparing
    /// the level of the logger with the
    /// <see cref="Nohros.Logging.LogLevel.Warn"/> level and if it is enabled
    /// the message is logged.
    /// </remarks>
    void Warn(string message);

    /// <summary>
    /// Log a message object with the
    /// <see cref="Nohros.Logging.LogLevel.Warn"/> level including the stack
    /// trace of the <see cref="Exception"/> passed as a parameter.
    /// </summary>
    /// <param name="message">The message object to log.</param>
    /// <param name="exception">The exception to log, including its stack
    /// trace.</param>
    /// <seealso cref="Warn(string)"/>
    /// <remarks>
    /// This method first checks if this logger is WARN enabled by comparing
    /// the level of the logger with the
    /// <see cref="Nohros.Logging.LogLevel.Warn"/> level and if it is enabled
    /// the message is logged.
    /// </remarks>
    void Warn(string message, Exception exception);
  }
}