using System;
using System.Threading;
using Nohros.Resources;

namespace Nohros.Concurrent
{
  /// <summary>
  /// <see cref="YQueue{T}"/> is an efficient queue implementation ported from
  /// the zeromq library. <see cref="YQueue{T}"/> allows one thread to use the
  /// <see cref="Enqueue"/> function while another one use the
  /// <see cref="Dequeue(out T)"/> function without locking.
  /// <typeparam name="T">
  /// The type of the objects in the queue.
  /// </typeparam>
  /// </summary>
  internal class YQueue<T>
  {
    class Entry
    {
      public readonly T value;
      public readonly bool sentinel;

      public Entry(T value, bool sentinel = false) {
        this.value = value;
        this.sentinel = sentinel;
      }
    }

    class Chunk
    {
      public Chunk next;
      public readonly Entry[] values;

      #region .ctor
      /// <summary>
      /// Initializes a new instance of the <see cref="Chunk"/> class by using
      /// the specified granularity.
      /// </summary>
      /// <param name="granularity">
      /// The number of elements that the chunk can hold.
      /// </param>
      public Chunk(int granularity) {
        values = new Entry[granularity];
        next = null;
      }
      #endregion
    }

    const int kDefaultCapacity = 32;
    readonly int granularity_;

    // Head position should accessed exclusively by queue reader, while
    // back and end positions should be acessed exclusively by queue writer.
    Chunk head_chunk_, tail_chunk_, back_chunk_, spare_chunk_;
    int head_pos_, tail_pos_, back_pos_;

    // Points to the last enqueued item. This variable is used exclusively
    // by the writer thread.
    Entry writer_;

    // Point to the next item to be dequeued. This varibale is used
    // exclusively by the reader thread.
    Entry reader_;

    // The single point of contention between writer and reader threads.
    // Points past the last enqueued item. It if is NULL, reader is asleep.
    // This variable should always be acessed using atomic operations.
    Entry divider_;

    static readonly Entry sentinel_ = new Entry(default(T));

    /// <summary>
    /// Initializes a new instance of the <see cref="YQueue{T}"/> class
    /// that has the default granularity.
    /// </summary>
    public YQueue() : this(kDefaultCapacity) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="YQueue{T}"/> class
    /// by using the specified granularity.
    /// </summary>
    /// <param name="granularity">
    /// A number that defines the granularity of the queue(how many pushes
    /// have to be done till actual memory allocation is required).
    /// </param>
    public YQueue(int granularity) {
      granularity_ = granularity;

      head_chunk_ = new Chunk(granularity);
      head_pos_ = 0;
      back_chunk_ = tail_chunk_;
      back_pos_ = 0;
      tail_chunk_ = head_chunk_;
      tail_pos_ = 0;

      // Insert terminator element into the queue.
      Produce();

      // Lets point everything to the terminator.
      divider_ = reader_ = writer_ = back_chunk_.values[back_pos_];
    }

    /// <summary>
    /// Adds an element to the back end of the queue.
    /// </summary>
    /// <param name="element">
    /// The element to be added to the back end of the queue.
    /// </param>
    /// <returns>
    /// <c>true</c> if the reader thread is alive; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// If the reader thread is sleeping this method returns <c>false</c> and
    /// the caller should wake up the reader to restart the message processing.
    /// </remarks>
    public bool Enqueue(T element) {
      // Place the value to the queue, add new terminator element.
      Entry entry = new Entry(element);
      back_chunk_.values[back_pos_] = entry;
      Produce();

      // Lets make the produced item visible to the consumer thread, by
      // pointing the divider to the produced slot in an atomic fashion.
      if (Interlocked.Exchange(ref divider_, entry) != writer_) {
        // Exchange was unseccessul because |divider_| was  modified by
        // the reader thread. This means that the reader thread is asleep.
        writer_ = entry;
        return false;
      }

      // Reader is alive. Nothing special to do now. Just point the writter
      // to the wrote slot.
      writer_ = entry;
      return true;
    }

    /// <summary>
    /// Prepare the queue to enqueue a item by adding a slot to its back
    /// end. The added slot will be referenced by the |back_chunk_| at the
    /// position |back_pos_|.
    /// </summary>
    void Produce() {
      back_chunk_ = tail_chunk_;
      back_pos_ = tail_pos_;

      // Ensure that the back slot points to something.
      back_chunk_.values[back_pos_] = new Entry(default(T), true);

      if (++tail_pos_ != granularity_) {
        return;
      }

      // reuse the spare chunk as the new tail if it is set.
      Chunk previous_spare_chunk = Interlocked.Exchange(ref spare_chunk_, null);
      tail_chunk_.next = previous_spare_chunk ?? new Chunk(granularity_);

      tail_chunk_ = tail_chunk_.next;
      tail_pos_ = 0;
    }

    /// <summary>
    /// Removes all elements from the <see cref="YQueue{T}"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="Clear"/> removes the elements that are not currently
    /// present in the queue. Elements added to the queue after
    /// <see cref="Clear"/> is called and while <see cref="Clear"/> is running,
    /// will not be cleared.
    /// </para>
    /// This operation should be sychronized with the <see cref="Dequeue()"/>
    /// and <see cref="Dequeue(out T)"/> operations. The clear method should
    /// be called only by the producer thread.
    /// </remarks>
    public void Clear() {
    }

    /// <summary>
    /// Removes and returns the object at the beginning of the
    /// <see cref="YQueue{T}"/>.
    /// </summary>
    /// <returns><typeparamref name="T"/> The object that is removed from the
    /// <see cref="YQueue{T}"/></returns>
    /// <exception cref="InvalidOperationException">The
    /// <see cref="YQueue{T}"/> is empty.</exception>
    public T Dequeue() {
      T t;
      bool ok = Dequeue(out t);
      if (!ok) {
        throw new InvalidOperationException(
          StringResources.InvalidOperation_EmptyQueue);
      }
      return t;
    }

    /// <summary>
    /// Removes and returns the object at the beginning of the
    /// <see cref="YQueue&lt;T&gt;"/>.
    /// </summary>
    /// <param name="t">When this method returns, contains the object that was
    /// removed from the beginning of the <see cref="YQueue&lt;T&gt;"/>, if
    /// the object was successfully removed; otherwise the default value
    /// of the type <typeparamref name="T"/>.</param>
    /// <returns><c>true</c> if the queue is not empty and the object at the
    /// beginning of it was successfully removed; otherwise, false.
    /// </returns>
    public bool Dequeue(out T t) {
      // Ensure that the queue is not empty.
      Entry entry = head_chunk_.values[head_pos_];
      if (entry == reader_) {
        // The queue is probably empty, so lets check it by comparing the
        // value of the |divider_| with the value of the next entry to be
        // dequeued in an atomic fashion. If the queue is really empty, which
        // is true if the |divider_| is pointing to the next entry to be
        // dequeued, set the divider to |sentinel_|.
        reader_ = Interlocked.CompareExchange(ref divider_, sentinel_, entry);

        // If the consumer thread has been changed the value of divider right
        // before or after the CAS operation, the entry will be different than
        // the reader or the entry will be different than the value stored
        // on the entry's slot. We should re-read the entry value.
        entry = head_chunk_.values[head_pos_];
        if (entry == reader_) {
          t = default(T);
          return false;
        }

        // The value of entry could be the value fetched from the processor
        // cache (in which it you be equals to the first readed entry) or a
        // JIT/CPU optmization should prevent us to see the most
        // up to date value of entry. Lets ensure that we are not returning
        // the value of a sentinel to the caller.
        //
        // TODO (neylor.silva) Check if we an remove this when compiling
        // for processors with strong memory models.
        if (entry.sentinel) {
          t = default(T);
          return false;
        }
      }

      t = entry.value;
      Consume();
      return true;
    }

    /// <summary>
    /// Mark an element as consumed by advancing the head pointer of the
    /// head chunk by one position.
    /// </summary>
    void Consume() {
      if (++head_pos_ == granularity_) {
        Chunk previous_head_chunk = head_chunk_;
        head_chunk_ = head_chunk_.next;
        head_pos_ = 0;

        // |previous_head_chunk| has been more recently used than
        // |spare_chunk|, so for cache reasons we'll get rid of the spare and
        // use |previous_head_chunk| as the spare.This reduces the chances
        // of the spare object to reach the gen1 or gen2 GC level, minimizing
        // the work that needs to be done by the garbage collector.
        Interlocked.Exchange(ref spare_chunk_, previous_head_chunk);
      }
    }
  }
}
