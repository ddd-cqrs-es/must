using System;
using System.Runtime.Serialization;

namespace Nohros
{
  /// <summary>
  /// Exception thrown when an action would violate a NOT NULL constraint on
  /// repostiory. For example, when an attempt is made to persistently add an
  /// item to a repository that would violate that repository's NOT NULL
  /// constraint.
  /// </summary>
  [Serializable]
  public class NotNullConstraintViolationException : RepositoryException
  {
    /// <summary>
    /// Creates a new instance of the
    /// <see cref="NotNullConstraintViolationException"/> class.
    /// </summary>
    public NotNullConstraintViolationException() {
    }

    /// <summary>
    /// Creates a new instance of the
    /// <see cref="NotNullConstraintViolationException"/> class.
    /// </summary>
    /// <param name="message">
    /// A message describing why this
    /// <see cref="NotNullConstraintViolationException"/>was throw.
    /// </param>
    public NotNullConstraintViolationException(string message) : base(message) {
    }

    /// <summary>
    /// Creates a new instance of the
    /// <see cref="NotNullConstraintViolationException"/> class.
    /// </summary>
    /// <param name="inner_exception">
    /// The exception that caused this
    /// <see cref="NotNullConstraintViolationException"/> to be throw.
    /// </param>
    public NotNullConstraintViolationException(Exception inner_exception)
      : base(inner_exception) {
    }

    /// <summary>
    /// Creates a new instance if the
    /// <see cref="NotNullConstraintViolationException"/> class.
    /// </summary>
    /// <param name="info">
    /// The object that holds the information to deserialize.
    /// </param>
    /// <param name="context">
    /// Contextual information about the source or destination.
    /// </param>
    protected NotNullConstraintViolationException(SerializationInfo info,
      StreamingContext context) : base(info, context) {
    }

    /// <summary>
    /// Creates a new instance of the
    /// <see cref="NotNullConstraintViolationException"/> class.
    /// </summary>
    /// <param name="message">
    /// A message describing why this
    /// <see cref="NotNullConstraintViolationException"/> was throw.
    /// </param>
    /// <param name="inner_exception">
    /// The exception that caused this
    /// <see cref="NotNullConstraintViolationException"/>to be throw.
    /// </param>
    public NotNullConstraintViolationException(string message,
      Exception inner_exception) : base(message, inner_exception) {
    }
  }
}
