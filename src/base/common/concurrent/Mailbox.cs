using System;
using System.Threading;

namespace Nohros.Concurrent
{
  /// <summary>
  /// Mailbox is basically a queue to store messages sent to a object that
  /// lives in a particular thread.
  /// </summary>
  /// <remarks>
  /// Mailbox is intended to be used on concurrent enviroments that use message
  /// passing to achieve concurrency and internl scalability.
  /// <para>
  /// This mode of concurrency allows multithreaded applications to work
  /// without using mutexes, condition variables or semaphores to orchestrate
  /// the parallel processing. Instead, each object can live in its own thread
  /// and no other thread should ever touch it(that's why mutexes are not
  /// needed). Other threads can comunnicate with the object by sendind it
  /// messages(T). Same way the object can speak to other objects - potentially
  /// running in different threads - by sending them messages.
  /// </para>
  /// <para>
  /// The mailbox message processing is single threaded and no more than one
  /// task will be active at any given time. When a message is
  /// send to it, it is queued to be processed by the executor. The
  /// executor runs until the message queue is empty. So, if you are using
  /// a <see cref="SameThreadExecutor"/> that is no guarantee that a
  /// message callback is executed by the thread that send the message, if
  /// there is an thread already executing a callback when a message is sent,
  /// that thread will be used to process the following messages until the
  /// message queue is empty.
  /// </para>
  /// </remarks>
  /// <typeparam name="T">
  /// The type of the messages that the mailbox can receive.
  /// </typeparam>
  public class Mailbox<T>
  {
    protected const int kDefaultCapacity = 32;

    readonly MailboxReceiveCallback<T> callback_;

    // The pipe to store actual messages
    readonly YQueue<T> message_queue_;

    // There is only one thread receiving from the mailbox, but there is
    // arbitrary number of threads sending. Given that |message_queue_| requires
    // synchronized access on both of its endpoints, we have to synchronize
    // the sending side.
    readonly object mutex_;

    // True if the underlying queue is active, ie. when we are allowed to
    // read command from it.

    // The synchronization context of the thread that creates the mailbox.
    protected readonly SynchronizationContext synchronization_context_;

    Action receive_;

    volatile bool active_;

    /// <summary>
    /// Initializes a new instance of the <see cref="Mailbox{T}"/> class by
    /// using the specified receive callback.
    /// </summary>
    /// <param name="callback">
    /// A <see cref="MailboxReceiveCallback{T}"/> delegate that is called to
    /// process each message sent to the <see cref="Mailbox{T}"/>.
    /// </param>
    /// <remarks>
    /// A <see cref="Thread"/> from the <see cref="ThreadPool"/> is used to
    /// execute the callback.
    /// </remarks>
    public Mailbox(MailboxReceiveCallback<T> callback)
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
    public Mailbox(MailboxReceiveCallback<T> callback, int capacity) {
      mutex_ = new object();
      message_queue_ = new YQueue<T>(capacity);
      callback_ = callback;
      synchronization_context_ = SynchronizationContext.Current;

      receive_ = DefaultReceiver;

      // Get the pipe into passive state. That way, if the user starts by
      // polling on the associated queue, it will be woken up when
      // new message is posted.
      active_ = false;
    }

    internal Action Receiver {
      get { return receive_; }
      set { receive_ = value; }
    }

    Action DefaultReceiver {
      get {
        return synchronization_context_ == null
          ? (Action) (Receive)
          : () => Receive(synchronization_context_);
      }
    }

    /// <summary>
    /// Sends a command to the mailbox.
    /// </summary>
    /// <param name="message">
    /// The message to be sent.
    /// </param>
    public void Send(T message) {
      bool active;
      lock (mutex_) {
        message_queue_.Enqueue(message);

        // Cache the current value of the active flag locally, so we can
        // exit the lock block as soon as possible and perform remaining
        // tasks outside of it.
        active = active_;

        if (!active_) {
          active_ = true;
        }
      }

      // If we are not already processing messages, request a thread to do it.
      if (!active) {
        receive_();
      }
    }

    /// <summary>
    /// Receives messages from the mailbox and executes the receiver callback.
    /// </summary>
    /// <remarks>
    /// This method runs into a single dedicated thread.
    /// </remarks>
    protected void Receive() {
      T message;
      while (GetMessage(out message)) {
        callback_(message);
      }
    }

    /// <summary>
    /// Receives messages from the mailbox and execute the receiver callback
    /// using the context of the thread that creates the
    /// <see cref="Mailbox{T}"/> object.
    /// </summary>
    protected void Receive(SynchronizationContext context) {
      T message;
      while (GetMessage(out message)) {
        context.Send(state => callback_((T) state), message);
      }
    }

    /// <summary>
    /// Gets a message from the mailbox.
    /// </summary>
    /// <returns><c>true</c> when a message is successfully retrieved from
    /// the mailbox; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// If there are no incoming messages available at the mailbox, this
    /// method switch to passive state and returns <c>false</c>.
    /// </remarks>
    protected bool GetMessage(out T message) {
      // try to get message straight away.
      if (!message_queue_.Dequeue(out message)) {
        // If there are no more messages available, switch into passive mode.
        // We need to synchronize the state change with the sender, because
        // it uses it to ensure that no more that one thread runs the receive
        // method.
        lock (mutex_) {
          // recheck the queue for emptiness, now we are inside the lock. We
          // need to recheck to make sure that no messages are sent to the
          // queue between the first check and the lock operation.
          if (!message_queue_.Dequeue(out message)) {
            active_ = false;
            return false;
          }
        }
      }
      return true;
    }
  }
}
