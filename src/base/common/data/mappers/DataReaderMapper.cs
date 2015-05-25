using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Nohros.Data
{
  /// <summary>
  /// An implementation of the <see cref="IDataReaderMapper{T}"/> interface.
  /// </summary>
#if DEBUG
  public abstract partial class DataReaderMapper<T> : IDataReaderMapper<T>
#else
  internal abstract class DataReaderMapper<T> : IDataReaderMapper<T>
#endif
  {
    internal class Enumerator : IEnumerable<T>
    {
      readonly DataReaderMapper<T> mapper_;
      readonly Action<T> post_map_;
      readonly IDataReader reader_;

      #region .ctor
      public Enumerator(IDataReader reader, DataReaderMapper<T> mapper,
        Action<T> post_map) {
        mapper_ = mapper;
        reader_ = reader;
        post_map_ = post_map;
      }
      #endregion

      IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
      }

      public IEnumerator<T> GetEnumerator() {
        T t;
        while (mapper_.Map(reader_, true, out t)) {
          post_map_(t);
          yield return t;
        }
      }
    }

    internal CallableDelegate<T> loader_;
    internal List<Action<IDataReader, T>> links_;

    internal DataReaderMapper() {
      loader_ = NewT;
      links_ = Enumerable.Empty<Action<IDataReader, T>>().ToList();
    }

#if DEBUG
    public void Save(string assembly_file_name) {
      Dynamics_.AssemblyBuilder.Save(assembly_file_name);
    }
#endif

    /// <summary>
    /// Reads the first row from <paramref name="reader"/> and maps it
    /// to a object of type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="reader">
    /// A <see cref="IDataReader"/> containing the data to be mapped.
    /// </param>
    /// <returns>
    /// A object of type <typeparamref name="T"/> whose properties contains
    /// the values readed from <paramref name="reader"/> accordingly to the
    /// defined map.
    /// </returns>
    /// <exception cref="NoResultException">
    /// The <paramref name="reader"/> does not contain any data.
    /// </exception>
    public virtual T Map(IDataReader reader) {
      T t;
      GetOrdinals(reader);
      if (!Map(reader, true, out t)) {
        throw new NoResultException();
      }
      return t;
    }

    /// <summary>
    /// Reads the first row from <paramref name="reader"/> and maps it to a
    /// object of type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="reader">
    /// A <see cref="IDataReader"/> containing the data to be mapped.
    /// </param>
    /// <param name="t">
    /// When this method returns contains a object of type
    /// <typeparamref name="T"/> whose properties contains the values readed
    /// from <paramref name="reader"/> accordingly to the defined map.
    /// </param>
    /// <returns>
    /// <c>true</c> if <paramref name="reader"/> contains at least one row;
    /// otherwise <c>false</c>.
    /// </returns>
    public virtual bool Map(IDataReader reader, out T t) {
      GetOrdinals(reader);
      return Map(reader, true, out t);
    }

    /// <summary>
    /// Maps the current row of the <paramref name="reader"/> and maps it to
    /// a object of type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="reader">
    /// A <see cref="IDataReader"/> containing the data to be mapped.
    /// </param>
    /// <returns>
    /// A object of type <typeparamref name="T"/> whose properties contains
    /// the values readed from <paramref name="reader"/> accordingly to the
    /// defined map.
    /// </returns>
    /// <exception cref="NoResultException">
    /// The <paramref name="reader"/> does not contain any data.
    /// </exception>
    /// <remarks>
    /// This method is usually used to map the same row to more than one
    /// object. Generally by combining multiple
    /// <see cref="DataReaderMapper{T}"/>s.
    /// </remarks>
    public virtual T MapCurrent(IDataReader reader) {
      T t;
      // Ensure that the ordinals array is populated.
      GetOrdinals(reader);
      Map(reader, false, out t);
      return t;
    }

    /// <summary>
    /// Reads the the first row of the <paramref name="reader"/>, maps it
    /// to a object of type <typeparamref name="T"/> and executes the given
    /// <paramref name="post_map"/> delegate.
    /// </summary>
    /// <param name="reader">
    /// A <see cref="IDataReader"/> containing the data to be mapped.
    /// </param>
    /// <param name="post_map">
    /// A <see cref="Action{T}"/> to be executed after the mapping operation.
    /// </param>
    /// <returns>
    /// A object of type <typeparamref name="T"/> whose properties contains
    /// the values readed from <paramref name="reader"/> accordingly to the
    /// defined map.
    /// </returns>
    /// <exception cref="NoResultException">
    /// The <paramref name="reader"/> does not contain any data.
    /// </exception>
    public virtual T Map(IDataReader reader, Action<T> post_map) {
      T t = Map(reader);
      post_map(t);
      return t;
    }

    /// <summary>
    /// Reads all the rows of the <paramref name="reader"/> and maps it to a
    /// collection of objects of type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="reader">
    /// A <see cref="IDataReader"/> containing the data to be mapped.
    /// </param>
    /// <param name="defer">
    /// A value that indicates if the returned <see cref="IEnumerable{T}"/>
    /// should be deffered.
    /// </param>
    /// <returns>
    /// A <see cref="IEnumerable{T}"/> containing objects whose properties
    /// is set to the values readed from <paramref name="reader"/> accordingly
    /// to the defined map.
    /// </returns>
    /// <remarks>
    /// If the <paramref name="defer"/> is set to true the
    /// <paramref name="reader"/> will remain opened until the enumerable
    /// is resolved.
    /// <para>
    /// Its a good practive to pass a deferred enumerable between applications
    /// layers. You usually set the <paramref name="defer"/> to true if
    /// you want to use the result of the enumerable to execute another action
    /// on the same application layer (Ex. use the result of one query in
    /// another query on the application data access layer).
    /// </para>
    /// </remarks>
    public virtual IEnumerable<T> Map(IDataReader reader, bool defer) {
      return Map(reader, defer, (Action<T>) delegate { });
    }

    /// <summary>
    /// Reads all the rows of the <paramref name="reader"/>, maps it to a
    /// collection of objects of type <typeparamref name="T"/> and executes the
    /// given <paramref name="post_map"/> delegate for each mapped row.
    /// </summary>
    /// <param name="reader">
    /// A <see cref="IDataReader"/> containing the data to be mapped.
    /// </param>
    /// <param name="defer">
    /// A value that indicates if the returned <see cref="IEnumerable{T}"/>
    /// should be deffered.
    /// </param>
    /// <param name="post_map">
    /// A <see cref="Action{T}"/> to be executed for each row after it is
    /// mapped.
    /// </param>
    /// <returns>
    /// A <see cref="IEnumerable{T}"/> containing objects whose properties
    /// is set to the values readed from <paramref name="reader"/> accordingly
    /// to the defined map.
    /// </returns>
    /// <remarks>
    /// If the <paramref name="defer"/> is set to true the
    /// <paramref name="reader"/> will remain opened until the enumerable
    /// is resolved.
    /// <para>
    /// Its a good practive to pass a deferred enumerable between applications
    /// layers. You usually set the <paramref name="defer"/> to true if
    /// you want to use the result of the enumerable to execute another action
    /// on the same application layer (Ex. use the result of one query in
    /// another query on the application data access layer).
    /// </para>
    /// </remarks>
    public virtual IEnumerable<T> Map(IDataReader reader, bool defer,
      Action<T> post_map) {
      GetOrdinals(reader);
      var enumerable = new Enumerator(reader, this, post_map);
      if (defer) {
        return enumerable;
      }
      return new List<T>(enumerable);
    }

    internal bool Map(IDataReader reader, bool read, out T t) {
      if (read) {
        if (reader.Read()) {
          t = Link(reader, MapInternal(reader));
          return true;
        }
        t = default(T);
        return false;
      }
      t = Link(reader, MapInternal(reader));
      return true;
    }

    T Link(IDataReader reader, T t) {
      foreach (Action<IDataReader, T> action in links_) {
        action(reader, t);
      }
      return t;
    }

    internal abstract T MapInternal(IDataReader reader);

    internal virtual void GetOrdinals(IDataReader reader) {
      throw new NotImplementedException();
    }

    internal virtual T NewT() {
      throw new NotImplementedException();
    }
  }
}
