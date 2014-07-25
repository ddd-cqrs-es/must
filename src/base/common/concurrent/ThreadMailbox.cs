using System;
using System.Threading;

namespace Nohros.Concurrent
{
  /// <summary>
  /// A implementation of the <see cref="Mailbox{T}"/> that uses a dedicated
  /// thread to receive messages.
  /// </summary>
  public class ThreadMailbox<T> : Mailbox<T>
  {
    readonly Thread thread_;
    readonly AutoResetEvent sync_;
    volatile bool running_;

    /// <summary>
    /// Initializes a new instance of the <see cref="Mailbox{T}"/> class by
    /// using the specified receive callback and initial capacity.
    /// </summary>
    /// <param name="callback">
    /// A <see cref="MailboxReceiveCallback{T}"/> delegate that is called to
    /// process each message sent to the <see cref="Mailbox{T}"/>.
    /// </param>
    /// <param name="factory">
    /// A <see cref="IThreadFactory"/> that can be used to create the thread
    /// that will be used to receive mailbox messages.
    /// </param>
    /// <remarks>
    /// The <see cref="Thread"/> created by the <paramref name="factory"/> is
    /// used to execute the callback.
    /// </remarks>
    public ThreadMailbox(MailboxReceiveCallback<T> callback,
      IThreadFactory factory) : this(callback, factory, kDefaultCapacity) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Mailbox{T}"/> class by
    /// using the specified receive callback and initial capacity.
    /// </summary>
    /// <param name="callback">
    /// A <see cref="MailboxReceiveCallback{T}"/> delegate that is called to
    /// process each message sent to the <see cref="Mailbox{T}"/>.
    /// </param>
    /// <param name="factory">
    /// A <see cref="IThreadFactory"/> that can be used to create the thread
    /// that will be used to receive mailbox messages.
    /// </param>
    /// <param name="capacity">
    /// The number of messages that the <see cref="Mailbox{T}"/> can initially
    /// store
    /// </param>
    /// <remarks>
    /// The <see cref="Thread"/> created by the <paramref name="factory"/> is
    /// used to execute the callback.
    /// </remarks>
    public ThreadMailbox(MailboxReceiveCallback<T> callback,
      IThreadFactory factory, int capacity) : base(callback, capacity) {
      running_ = true;
      sync_ = new AutoResetEvent(false);

      thread_ = factory.CreateThread(ThreadMain);
      thread_.Start(synchronization_context_);

      Receiver = () => sync_.Set();
    }

    void ThreadMain(object obj) {
      var context = obj as SynchronizationContext;
      if (context == null) {
        while (running_) {
          sync_.WaitOne();
          Receive();
        }
      } else {
        while (running_) {
          sync_.WaitOne();
          Receive(context);
        }
      }
    }

    /// <summary>
    /// Stops the mailbox thread.
    /// </summary>
    public void Shutdown() {
      running_ = false;
      sync_.Set();
      thread_.Join();
    }
  }
}
