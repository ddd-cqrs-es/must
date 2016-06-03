using System;
using System.Collections.Concurrent;
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
    readonly ConcurrentQueue<T> message_queue_;

    // There is only one thread receiving from the mailbox, but there is
    // arbitrary number of threads sending. Given that |message_queue_| requires
    // synchronized access on both of its endpoints, we have to synchronize
    // the sending side.
    readonly object mutex_;

    // True if the underlying queue is active, ie. when we are allowed to
    // read command from it.
    volatile bool active_;

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
    public Mailbox(MailboxReceiveCallback<T> callback) {
      mutex_ = new object();
      message_queue_ = new ConcurrentQueue<T>();
      callback_ = callback;

      // Get the pipe into passive state. That way, if the user starts by
      // polling on the associated queue, it will be woken up when
      // new message is posted.
      active_ = false;
    }

    /// <summary>
    /// Sends a command to the mailbox.
    /// </summary>
    /// <param name="message">
    /// The message to be sent.
    /// </param>
    public void Send(T message) {
      message_queue_.Enqueue(message);

      // Woken up the consumer thread if it is sleeping.
      if (!active_) {
        // Synchronize the mode exchange with the consumer to ensure that only
        // one thread is consuming at a given point in time.
        lock (mutex_) {
          if (!active_) {
            active_ = true;

            // The mailbox is dormat, we need to get a thread from thread pool
            // to process our pending requests.
            ThreadPool.QueueUserWorkItem(ThreadMain);
          }
        }
      }
    }

    /// <summary>
    /// Receives messages from the mailbox and executes the receiver callback.
    /// </summary>
    /// <remarks>
    /// This method runs into a single dedicated thread.
    /// </remarks>
    void Receive() {
      T message;
      while (GetMessage(out message)) {
        callback_(message);
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
    bool GetMessage(out T message) {
      // try to get message straight away.
      if (!message_queue_.TryDequeue(out message)) {
        // If there are no more messages available, switch into passive mode.
        // We need to synchronize the state change with the sender, because
        // it uses it to ensure that no more that one thread runs the receive
        // method.
        lock (mutex_) {
          // recheck the queue for emptiness, now we are inside the lock. We
          // need to recheck to make sure that no messages are sent to the
          // queue between the first check and the lock operation.
          if (!message_queue_.TryDequeue(out message)) {
            active_ = false;
          }
        }

        // If the queue is empty we need to raise MaiboxEmpty event. The
        // emptiness is detected inside a lock block that is in sync with the
        // sender side. If we raise the event inside that lock block the
        // sender side will be blocked until the event is processed and the
        // event processing is not in the control of this class.
        if (!active_) {
          OnEmpty();
          return false;
        }
      }
      return true;
    }

    void OnEmpty() {
      if (Empty != null) {
        Empty(this);
      }
    }

    void ThreadMain(object obj) {
      Receive();
    }

    /// <summary>
    /// Event that is raised when the queue becomes empty.
    /// </summary>
    /// <remarks>
    /// This event is raised only after the first item was added and consumed
    /// from the mailbox.
    /// <para>
    /// Note that this event is raised at the exactly moment when the
    /// <see cref="Mailbox{T}"/> becomes empty. It may be the case that a
    /// sender modify the <see cref="Mailbox{T}"/> after the event is raised.
    /// The purpose of this event is just to notify that the
    /// <see cref="Mailbox{T}"/> becomes empty but no conclusion should be
    /// assumed about the current state of the <see cref="Mailbox{T}"/>.
    /// </para>
    /// </remarks>
    public event Action<Mailbox<T>> Empty;

    /// <summary>
    /// Get the number of elements contained in the
    /// <see cref="Mailbox{T}"/>.
    /// </summary>
    /// <remarks>
    /// For determining whether the collection contains any items, use of the
    /// <see cref="IsEmpty"/> property is recommended rather than retrieving
    /// the number of items from the <see cref="Count"/> property and
    /// comparing it to 0.
    /// </remarks>
    public int Count {
      get { return message_queue_.Count; }
    }

    /// <summary>
    /// Gets a value that indicates whether the <see cref="Mailbox{T}"/> is
    /// empty.
    /// </summary>
    /// <value>
    /// <c>true</c> if the <see cref="Mailbox{T}"/> is empty; otherwise,
    /// <c>false</c>.
    /// </value>
    /// <remarks>
    /// For determining whether the collection contains any items, use of this
    /// property is recommended rather than retrieving the number of items
    /// from the <see cref="Count"/> property and comparing it to 0. However,
    /// as this collection is intended to be accessed concurrently, it may
    /// be the case that another thread will modify the collection after
    /// <see cref="IsEmpty"/> returns, thus invalidating the result.
    /// </remarks>
    public bool IsEmpty {
      get { return message_queue_.IsEmpty; }
    }
  }
}
