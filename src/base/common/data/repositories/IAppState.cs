using System;
using System.Collections.Generic;

namespace Nohros.Data
{
  /// <summary>
  /// Extensions method for <see cref="IAppState"/> interface.
  /// </summary>
  public static class AppStateExtensions
  {
    /// <summary>
    /// Sets, in an atomic operation, the values of the state associated
    /// with the key <paramref name="name"/> if the current value is greater
    /// the given state or if the key is not found.
    /// </summary>
    /// <param name="states">
    /// A <see cref="IAppState"/> to extend.
    /// </param>
    /// <param name="name">
    /// The name of the state to set.
    /// </param>
    /// <param name="state">
    /// The value of the state to set.
    /// </param>
    /// <param name="comparand">
    /// The value that is compared to the current state.
    /// </param>
    /// <exception cref="NotSupportedException">
    /// The local database does not support the type <see cref="T"/>
    /// </exception>
    public static bool SetIfGreaterThan<T>(this IAppState states, string name,
      T state, T comparand) {
      return states.SetIf(ComparisonOperator.GreaterThan, name, state, comparand);
    }

    /// <summary>
    /// Sets, in an atomic operation, the values of the state associated
    /// with the key <paramref name="name"/> if the current value is less
    /// the given state or if the key is not found.
    /// </summary>
    /// <param name="states">
    /// A <see cref="IAppState"/> to extend.
    /// </param>
    /// <param name="name">
    /// The name of the state to set.
    /// </param>
    /// <param name="state">
    /// The value of the state to set.
    /// </param>
    /// <param name="comparand">
    /// The value that is compared to the current state.
    /// </param>
    /// <exception cref="NotSupportedException">
    /// The local database does not support the type <see cref="T"/>
    /// </exception>
    public static bool SetIfLessThan<T>(this IAppState states, string name,
      T state, T comparand) {
      return states.SetIf(ComparisonOperator.LessThan, name, state, comparand);
    }

    /// <summary>
    /// Sets, in an atomic operation, the values of the state associated
    /// with the key <paramref name="name"/> if the current value is less
    /// the given state or if the key is not found.
    /// </summary>
    /// <param name="states">
    /// A <see cref="IAppState"/> to extend.
    /// </param>
    /// <param name="name">
    /// The name of the state to set.
    /// </param>
    /// <param name="state">
    /// The value of the state to set.
    /// </param>
    /// <param name="comparand">
    /// The value that is compared to the current state.
    /// </param>
    /// <exception cref="NotSupportedException">
    /// The local database does not support the type <see cref="T"/>
    /// </exception>
    public static bool SetIfEqualsTo<T>(this IAppState states, string name,
      T state, T comparand) {
      return states.SetIf(ComparisonOperator.Equals, name, state, comparand);
    }

    /// <summary>
    /// Increments (increases by one) the value of the state associated with
    /// the given <paramref name="name"/> as an atomic operation or associates
    /// the the specified <paramref name="name"/> with the value one.
    /// </summary>
    /// <param name="states">
    /// A <see cref="IAppState"/> to extend.
    /// </param>
    /// <param name="name">
    /// The name of the state to be increased.
    /// </param>
    /// <exception cref="NotSupportedException">
    /// The local database does not support the type <see cref="int"/>
    /// </exception>
    public static void Increment(this IAppState states, string name) {
      states.Merge(name, 1);
    }
  }

  /// <summary>
  /// Represents an application state repository which is an object that
  /// contains a collection of key/value pairs representing application states.
  /// </summary>
  public interface IAppState
  {
    /// <summary>
    /// Determines whether the state's collection contains a specific state.
    /// </summary>
    /// <param name="name">
    /// The name of the state to locate in the <see cref="IAppState"/>.
    /// </param>
    /// <returns>
    /// <c>true</c> if the <paramref name="name"/> is found; otherwise,
    /// <c>false</c>.
    /// </returns>
    bool Contains<T>(string name);

    /// <summary>
    /// Gets the state associated with the key <paramref name="name"/>.
    /// </summary>
    /// <param name="name">
    /// The name of the state to get.
    /// </param>
    /// <returns>
    /// The value of the state associated with the key <paramref name="name"/>.
    /// </returns>
    /// <exception cref="NotSupportedException">
    /// The type <typeparamref name="T"/> is not supported by the local
    /// database.
    /// <typeparamref name="T"/>.
    /// <exception cref="NoResultException">
    /// There is no state associated with the key <paramref name="name"/>.
    /// </exception>
    /// </exception>
    T Get<T>(string name);

    /// <summary>
    /// Get all the states that is associated with a key that starts with
    /// the given <paramref name="prefix"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the states to get.
    /// </typeparam>
    /// <param name="prefix">
    /// A string that a key should starts with in order to be returned.
    /// </param>
    /// <returns>
    /// All the states that is associated with a key that starts with
    /// the given <paramref name="prefix"/>.
    /// </returns>
    IEnumerable<T> GetForPrefix<T>(string prefix);

    /// <summary>
    /// Get all the states that is associated with a key that starts with
    /// the given <paramref name="prefix"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the states to get.
    /// </typeparam>
    /// <param name="prefix">
    /// A string that a key should starts with in order to be returned.
    /// </param>
    /// <param name="limit">
    /// The maximum number of states to return.
    /// </param>
    /// <returns>
    /// All the states that is associated with a key that starts with
    /// the given <paramref name="prefix"/>.
    /// </returns>
    IEnumerable<T> GetForPrefix<T>(string prefix, int limit);

    /// <summary>
    /// Get all the states that is associated with a key that starts with
    /// the given <paramref name="prefix"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the states to get.
    /// </typeparam>
    /// <param name="prefix">
    /// A string that a key should starts with in order to be returned.
    /// </param>
    /// <param name="remove">
    /// A value that indicates if the returned states should be automatically
    /// removed.
    /// </param>
    /// <returns>
    /// All the states that is associated with a key that starts with
    /// the given <paramref name="prefix"/>.
    /// </returns>
    IEnumerable<T> GetForPrefix<T>(string prefix, bool remove);

    /// <summary>
    /// Get all the states that is associated with a key that starts with
    /// the given <paramref name="prefix"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the states to get.
    /// </typeparam>
    /// <param name="prefix">
    /// A string that a key should starts with in order to be returned.
    /// </param>
    /// <param name="limit">
    /// The maximum number of states to return.
    /// </param>
    /// <param name="remove">
    /// A value that indicates if the returned states should be automatically
    /// removed.
    /// </param>
    /// <returns>
    /// All the states that is associated with a key that starts with
    /// the given <paramref name="prefix"/>.
    /// </returns>
    IEnumerable<T> GetForPrefix<T>(string prefix, int limit, bool remove);

    /// <summary>
    /// Returns the state associated with the key <paramref name="name"/> or
    /// the given <paramref name="def"/> if a <paramref name="name"/> state is
    /// not found.
    /// </summary>
    /// <param name="name">
    /// The name of the state to get.
    /// </param>
    /// <param name="def">
    /// The value to be returned if <paramref name="name"/> state is not found.
    /// </param>
    /// <returns>
    /// The value of the state associated with the key <paramref name="name"/>.
    /// </returns>
    /// <exception cref="NotSupportedException">
    /// The type <typeparamref name="T"/> is not supported by the local
    /// database.
    /// <typeparamref name="T"/>.
    /// <exception cref="NoResultException">
    /// There is no state associated with the key <paramref name="name"/>.
    /// </exception>
    /// </exception>
    T Get<T>(string name, T def);

    /// <summary>
    /// Gets the state associated with the key <paramref name="name"/>.
    /// </summary>
    /// <param name="name">
    /// The name of the state to get.
    /// </param>
    /// <param name="state">
    /// The value of the state associated with the key <paramref name="name"/>.
    /// </param>
    /// <returns>
    /// <c>true</c> if there is a state associated with the key
    /// <paramref name="name"/>; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="NotSupportedException">
    /// The type <typeparamref name="T"/> is not supported by the local
    /// database.
    /// <typeparamref name="T"/>.
    /// </exception>
    bool Get<T>(string name, out T state);

    /// <summary>
    /// Sets the values of the state associated with the key
    /// <paramref name="name"/>.
    /// </summary>
    /// <param name="name">
    /// The name of the state to set.
    /// </param>
    /// <param name="state">
    /// The value of the state to set.
    /// </param>
    /// <exception cref="NotSupportedException">
    /// The local database does not support the type <typeparamref name="T"/>.
    /// </exception>
    void Set<T>(string name, T state);

    /// <summary>
    /// Set, in an atomic operation, the value of the state associated
    /// with the key <paramref name="name"/> if the operation described by
    /// the <paramref name="op"/> when applied over the current
    /// state's value and using the given <paramref name="comparand"/> is
    /// <c>true</c> or if the key is not found.
    /// </summary>
    /// <param name="name">
    /// The name of the state to set.
    /// </param>
    /// <param name="state">
    /// The value that replaces the current state if the compariso results in
    /// equality.
    /// </param>
    /// <param name="comparand">
    /// The value that is compared to the current state.
    /// </param>
    /// <param name="op">
    /// The operation to be applied over the current state's value using the
    /// given <paramref name="comparand"/>.
    /// </param>
    /// <returns>
    /// <c>true</c> if the state was set; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="NotSupportedException">
    /// The local database does not support the type <see cref="string"/>
    /// </exception>
    bool SetIf<T>(ComparisonOperator op, string name, T state, T comparand);

    /// <summary>
    /// Removes the state associated with the given <paramref name="name"/>.
    /// </summary>
    /// <param name="name">
    /// The name of the state to be removed.
    /// </param>
    /// <returns>
    /// <c>true</c> if a key was found and removed; otherwise, <c>false</c>.
    /// </returns>
    bool Remove<T>(string name);

    /// <summary>
    /// Removes all the states that is associated with a key that starts with
    /// the given <paramref name="prefix"/>.
    /// </summary>
    /// <param name="prefix">
    /// A string that a key should starts with in order to be returned.
    /// </param>
    /// <returns>
    /// The number of states that was removed.
    /// </returns>
    int RemoveForPrefix<T>(string prefix);

    /// <summary>
    /// Merges, in an atomic operation, the current state's value with the
    /// given <paramref name="state"/> or associates the given
    /// <paramref name="state"/> with the specified <paramref name="name"/>
    /// if <paramref name="name"/> if not found.
    /// </summary>
    /// <typeparam name="T">
    /// The state's data type.
    /// </typeparam>
    /// <param name="name">
    /// The name of the state to be merged.
    /// </param>
    /// <param name="state">
    /// The value to be merged with the current state.
    /// </param>
    /// <exception cref="NotSupportedException">
    /// The local database does not support the type <see cref="string"/>
    /// </exception>
    void Merge<T>(string name, T state);
  }
}
