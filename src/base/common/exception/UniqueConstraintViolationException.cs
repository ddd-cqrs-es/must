using System;
using System.Runtime.Serialization;

namespace Nohros
{
  /// <summary>
  /// Exception thrown when an action would violate a constraint on repostiory
  /// structure. For example, when an attempt is made to persistently add an
  /// item to a repository that would violate that repository's constraint.
  /// </summary>
  [Serializable]
  public class UniqueConstraintViolationException : RepositoryException
  {
    /// <summary>
    /// Creates a new instance of the
    /// <see cref="UniqueConstraintViolationException"/> class.
    /// </summary>
    public UniqueConstraintViolationException() {
    }

    /// <summary>
    /// Creates a new instance of the
    /// <see cref="UniqueConstraintViolationException"/> class.
    /// </summary>
    /// <param name="message">
    /// A message describing why this
    /// <see cref="UniqueConstraintViolationException"/>was throw.
    /// </param>
    public UniqueConstraintViolationException(string message)
      : base(message) {
    }

    /// <summary>
    /// Creates a new instance of the
    /// <see cref="UniqueConstraintViolationException"/> class.
    /// </summary>
    /// <param name="inner_exception">
    /// The exception that caused this
    /// <see cref="UniqueConstraintViolationException"/> to be throw.
    /// </param>
    public UniqueConstraintViolationException(Exception inner_exception)
      : base(inner_exception) {
    }

    /// <summary>
    /// Creates a new instance if the
    /// <see cref="UniqueConstraintViolationException"/> class.
    /// </summary>
    /// <param name="info">
    /// The object that holds the information to deserialize.
    /// </param>
    /// <param name="context">
    /// Contextual information about the source or destination.
    /// </param>
    protected UniqueConstraintViolationException(SerializationInfo info,
      StreamingContext context)
      : base(info, context) {
    }

    /// <summary>
    /// Creates a new instance of the
    /// <see cref="ConstraintViolationException"/> class.
    /// </summary>
    /// <param name="message">
    /// A message describing why this
    /// <see cref="UniqueConstraintViolationException"/> was throw.
    /// </param>
    /// <param name="inner_exception">
    /// The exception that caused this
    /// <see cref="UniqueConstraintViolationException"/>to be throw.
    /// </param>
    public UniqueConstraintViolationException(string message,
      Exception inner_exception)
      : base(message, inner_exception) {
    }
  }
}
