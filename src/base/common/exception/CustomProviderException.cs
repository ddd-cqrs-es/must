using System;
using System.Runtime.Serialization;

namespace Nohros
{
  /// <summary>
  /// Exception thrown when an custom error(an user defined exceotion) is
  /// raised. For example, when an attempt is made to persistently add an
  /// item to a repository that would violate the logic of a table trigger.
  /// </summary>
  [Serializable]
  public class CustomProviderException : RepositoryException
  {
    #region .ctor
    /// <summary>
    /// Creates a new instance of the
    /// <see cref="CustomProviderException"/> class.
    /// </summary>
    public CustomProviderException() {
    }

    /// <summary>
    /// Creates a new instance of the
    /// <see cref="CustomProviderException"/> class.
    /// </summary>
    /// <param name="message">
    /// A message describing why this
    /// <see cref="CustomProviderException"/>was throw.
    /// </param>
    public CustomProviderException(string message)
      : base(message) {
    }

    /// <summary>
    /// Creates a new instance of the
    /// <see cref="CustomProviderException"/> class.
    /// </summary>
    /// <param name="inner_exception">
    /// The exception that caused this
    /// <see cref="CustomProviderException"/> to be throw.
    /// </param>
    public CustomProviderException(Exception inner_exception)
      : base(inner_exception) {
    }

    /// <summary>
    /// Creates a new instance if the
    /// <see cref="CustomProviderException"/> class.
    /// </summary>
    /// <param name="info">
    /// The object that holds the information to deserialize.
    /// </param>
    /// <param name="context">
    /// Contextual information about the source or destination.
    /// </param>
    protected CustomProviderException(SerializationInfo info,
      StreamingContext context) : base(info, context) {
    }

    /// <summary>
    /// Creates a new instance of the
    /// <see cref="CustomProviderException"/> class.
    /// </summary>
    /// <param name="message">
    /// A message describing why this
    /// <see cref="CustomProviderException"/> was throw.
    /// </param>
    /// <param name="inner_exception">
    /// The exception that caused CustomProviderException
    /// <see cref="CustomProviderException"/>to be throw.
    /// </param>
    public CustomProviderException(string message,
      Exception inner_exception) : base(message, inner_exception) {
    }
    #endregion
  }
}
