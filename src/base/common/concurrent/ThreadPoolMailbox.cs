using System;
using System.Threading;

namespace Nohros.Concurrent
{
  /// <summary>
  /// A implementation of the <see cref="Mailbox{T}"/> that uses a thread from
  /// the <see cref="ThreadPool"/> to receive messages.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class ThreadPoolMailbox<T> : Mailbox<T>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="Mailbox{T}"/> class by
    /// using the specified receive callback and initial capacity.
    /// </summary>
    /// <param name="callback">
    /// A <see cref="MailboxReceiveCallback{T}"/> delegate that is called to
    /// process each message sent to the <see cref="Mailbox{T}"/>.
    /// </param>
    /// <remarks>
    /// A <see cref="Thread"/> from the <see cref="ThreadPool"/> is used to
    /// execute the callback.
    /// </remarks>
    public ThreadPoolMailbox(MailboxReceiveCallback<T> callback)
      : this(callback, kDefaultCapacity) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Mailbox{T}"/> class by
    /// using the specified receive callback and initial capacity.
    /// </summary>
    /// <param name="callback">
    /// A <see cref="MailboxReceiveCallback{T}"/> delegate that is called to
    /// process each message sent to the <see cref="Mailbox{T}"/>.
    /// </param>
    /// <param name="capacity">
    /// The number of messages that the <see cref="Mailbox{T}"/> can initially
    /// store
    /// </param>
    /// <remarks>
    /// A <see cref="Thread"/> from the <see cref="ThreadPool"/> is used to
    /// execute the callback.
    /// </remarks>
    public ThreadPoolMailbox(MailboxReceiveCallback<T> callback, int capacity)
      : base(callback, capacity) {
      var wait_callback =
        (synchronization_context_ == null)
          ? (WaitCallback) (state => Receive())
          : state => Receive(state as SynchronizationContext);
      Receiver = () => ThreadPool.QueueUserWorkItem(wait_callback);
    }
  }
}
