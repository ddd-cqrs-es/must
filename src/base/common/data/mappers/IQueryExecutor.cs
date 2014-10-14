using System;
using System.Data;

namespace Nohros.Data
{
  /// <summary>
  /// Execute SQL commands againts a database.
  /// </summary>
  public interface IQueryExecutor
  {
    /// <summary>
    /// Executes the command described by <see cref="query"/> on the server and
    /// returns a <see cref="IDataReaderMapper{T}"/> that can be used to
    /// map the result set to a object of the type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the object that should be returned.
    /// </typeparam>
    /// <param name="query">
    /// The query to be executed on the server.
    /// </param>
    /// <param name="mapper">
    /// A <see cref="Func{TResult}"/> that builds a
    /// <see cref="IDataReaderMapper{T}"/> that can be used to map
    /// result of the given <paramref name="query"/> to a
    /// object of the type <typeparamref name="T"/>.
    /// </param>
    /// <returns>
    /// The object produced by the execution of the method
    /// <paramref name="mapper"/>.
    /// </returns>
    /// <remarks>
    /// The mapper returned from the <see cref="Func{TResult}"/> is cached
    /// internally and associated with the given <paramref name="query"/>. The
    /// cache is never flushed. If you are generating SQL strings on the fly
    /// without using parameters it is possible you hit memory issues.
    /// </remarks>
    IQueryMapper<T> ExecuteQuery<T>(string query,
      Func<IDataReaderMapper<T>> mapper);

    /// <summary>
    /// Executes the command described by <see cref="query"/> on the server and
    /// returns a <see cref="IDataReaderMapper{T}"/> that can be used to
    /// map the result set to a object of the type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the object that should be returned.
    /// </typeparam>
    /// <param name="query">
    /// The query to be executed on the server.
    /// </param>
    /// <param name="mapper">
    /// A <see cref="Func{TResult}"/> that builds a
    /// <see cref="IDataReaderMapper{T}"/> that can be used to map
    /// result of the given <paramref name="query"/> to a
    /// object of the type <typeparamref name="T"/>.
    /// </param>
    /// <param name="command_type">
    /// The type of the command that is described by the
    /// <paramref name="query"/> parameter.
    /// </param>
    /// <returns>
    /// The object produced by the execution of the method
    /// <paramref name="mapper"/>.
    /// </returns>
    /// <remarks>
    /// The mapper returned from the <see cref="Func{TResult}"/> is cached
    /// internally and associated with the given <paramref name="query"/>. The
    /// cache is never flushed. If you are generating SQL strings on the fly
    /// without using parameters it is possible you hit memory issues.
    /// </remarks>
    IQueryMapper<T> ExecuteQuery<T>(string query,
      Func<IDataReaderMapper<T>> mapper, CommandType command_type);

    /// <summary>
    /// Executes the command described by <see cref="query"/> on the server and
    /// returns a <see cref="IDataReaderMapper{T}"/> that can be used to
    /// map the result set to a object of the type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the object that should be returned.
    /// </typeparam>
    /// <param name="query">
    /// The query to be executed on the server.
    /// </param>
    /// <param name="mapper">
    /// A <see cref="Func{TResult}"/> that builds a
    /// <see cref="IDataReaderMapper{T}"/> that can be used to map
    /// result of the given <paramref name="query"/> to a
    /// object of the type <typeparamref name="T"/>.
    /// </param>
    /// <param name="set_parameters">
    /// A <see cref="Action{T}"/> that allows the caller to set the values
    /// of the parameters defined on the given query.
    /// </param>
    /// <returns>
    /// The object produced by the execution of the method
    /// <paramref name="mapper"/>.
    /// </returns>
    /// <remarks>
    /// The mapper returned from the <see cref="Func{TResult}"/> is cached
    /// internally and associated with the given <paramref name="query"/>. The
    /// cache is never flushed. If you are generating SQL strings on the fly
    /// without using parameters it is possible you hit memory issues.
    /// </remarks>
    IQueryMapper<T> ExecuteQuery<T>(string query,
      Func<IDataReaderMapper<T>> mapper,
      Action<CommandBuilder> set_parameters);

    /// <summary>
    /// Executes the command described by <see cref="query"/> on the server and
    /// runs the method <paramref name="mapper"/> to map the result set to
    /// the type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the object that should be returned.
    /// </typeparam>
    /// <param name="query">
    /// The query to be executed on the server.
    /// </param>
    /// <param name="command_type">
    /// The type of the command that is described by the
    /// <paramref name="query"/> parameter.
    /// </param>
    /// <param name="mapper">
    /// A <see cref="Func{TResult}"/> that builds a
    /// <see cref="IDataReaderMapper{T}"/> that can be used to map
    /// result of the given <paramref name="query"/> to a
    /// object of the type <typeparamref name="T"/>.
    /// </param>
    /// <param name="set_parameters">
    /// A <see cref="Action{T}"/> that allows the caller to set the values
    /// of the parameters defined on the given query.
    /// </param>
    /// <returns>
    /// The object produced by the execution of the method
    /// <paramref name="mapper"/>.
    /// </returns>
    /// <remarks>
    /// The mapper returned from the <see cref="Func{TResult}"/> is cached
    /// internally and associated with the given <paramref name="query"/>. The
    /// cache is never flushed. If you are generating SQL strings on the fly
    /// without using parameters it is possible you hit memory issues.
    /// </remarks>
    IQueryMapper<T> ExecuteQuery<T>(string query,
      Func<IDataReaderMapper<T>> mapper,
      Action<CommandBuilder> set_parameters,
      CommandType command_type);


    /// <summary>
    /// Executes the command described by <see cref="query"/> on the server and
    /// returns the number of rows affected.
    /// </summary>
    /// <param name="query">
    /// The query to be executed on the server.
    /// </param>
    /// <returns>
    /// The number of rows affected.
    /// </returns>
    int ExecuteNonQuery(string query);

    /// <summary>
    /// Executes the command described by <see cref="query"/> on the server and
    /// returns the number of rows affected.
    /// </summary>
    /// <param name="query">
    /// The query to be executed on the server.
    /// </param>
    /// <param name="command_type">
    /// The type of the command that is described by the
    /// <paramref name="query"/> parameter.
    /// </param>
    /// <returns>
    /// The number of rows affected.
    /// </returns>
    int ExecuteNonQuery(string query, CommandType command_type);

    /// <summary>
    /// Executes the command described by <see cref="query"/> on the server and
    /// returns the number of rows affected.
    /// </summary>
    /// <param name="query">
    /// The query to be executed on the server.
    /// </param>
    /// <param name="set_parameters">
    /// A <see cref="Action{T}"/> that allows the caller to set the values
    /// of the parameters defined on the given query.
    /// </param>
    /// <returns>
    /// The number of rows affected.
    /// </returns>
    int ExecuteNonQuery(string query, Action<CommandBuilder> set_parameters);

    /// <summary>
    /// Executes the command described by <see cref="query"/> on the server and
    /// returns the number of rows affected.
    /// </summary>
    /// <param name="query">
    /// The query to be executed on the server.
    /// </param>
    /// <param name="command_type">
    /// The type of the command that is described by the
    /// <paramref name="query"/> parameter.
    /// </param>
    /// <param name="set_parameters">
    /// A <see cref="Action{T}"/> that allows the caller to set the values
    /// of the parameters defined on the given query.
    /// </param>
    /// <returns>
    /// The number of rows affected.
    /// </returns>
    int ExecuteNonQuery(string query, Action<CommandBuilder> set_parameters,
      CommandType command_type);

    /// <summary>
    /// Executes the command described by <see cref="query"/> on the server and
    /// returned the value of the first column of the first row of the
    /// resulting recordset.
    /// </summary>
    /// <param name="query">
    /// The query to be executed on the server.
    /// </param>
    /// <returns>
    /// The value of the first column of the first row of the recordset
    /// resulted from the execution of the <paramref name="query"/>.
    /// </returns>
    T ExecuteScalar<T>(string query);

    /// <summary>
    /// Executes the command described by <see cref="query"/> on the server and
    /// returned the value of the first column of the first row of the
    /// resulting recordset.
    /// </summary>
    /// <param name="query">
    /// The query to be executed on the server.
    /// </param>
    /// <param name="t">
    /// When this method returns contains the value of the first column of
    /// the first row of the recordset resulted from the execution of the
    /// <paramref name="query"/> if the recordset is not empty; otherwise,
    /// the default value for the type <typeparamref name="T"/>.
    /// </param>
    /// <returns>
    /// <c>true</c> if the recordset resulted from the execution of the
    /// <paramref name="query"/> is not empty; otherwise, <c>false</c>.
    /// </returns>
    bool ExecuteScalar<T>(string query, out T t);

    /// <summary>
    /// Executes the command described by <see cref="query"/> on the server and
    /// returned the value of the first column of the first row of the
    /// resulting recordset.
    /// </summary>
    /// <param name="query">
    /// The query to be executed on the server.
    /// </param>
    /// <param name="command_type">
    /// The type of the command that is described by the
    /// <paramref name="query"/> parameter.
    /// </param>
    /// <returns>
    /// The value of the first column of the first row of the recordset
    /// resulted from the execution of the <paramref name="query"/>.
    /// </returns>
    T ExecuteScalar<T>(string query, CommandType command_type);

    /// <summary>
    /// Executes the command described by <see cref="query"/> on the server and
    /// returned the value of the first column of the first row of the
    /// resulting recordset.
    /// </summary>
    /// <param name="query">
    /// The query to be executed on the server.
    /// </param>
    /// <param name="command_type">
    /// The type of the command that is described by the
    /// <paramref name="query"/> parameter.
    /// </param>
    /// <param name="t">
    /// When this method returns contains the value of the first column of
    /// the first row of the recordset resulted from the execution of the
    /// <paramref name="query"/> if the recordset is not empty; otherwise,
    /// the default value for the type <typeparamref name="T"/>.
    /// </param>
    /// <returns>
    /// <c>true</c> if the recordset resulted from the execution of the
    /// <paramref name="query"/> is not empty; otherwise, <c>false</c>.
    /// </returns>
    bool ExecuteScalar<T>(string query, CommandType command_type, out T t);

    /// <summary>
    /// Executes the command described by <see cref="query"/> on the server and
    /// returned the value of the first column of the first row of the
    /// resulting recordset.
    /// </summary>
    /// <param name="query">
    /// The query to be executed on the server.
    /// </param>
    /// <param name="set_parameters">
    /// A <see cref="Action{T}"/> that allows the caller to set the values
    /// of the parameters defined on the given query.
    /// </param>
    /// <returns>
    /// The value of the first column of the first row of the recordset
    /// resulted from the execution of the <paramref name="query"/>.
    /// </returns>
    T ExecuteScalar<T>(string query, Action<CommandBuilder> set_parameters);

    /// <summary>
    /// Executes the command described by <see cref="query"/> on the server and
    /// returned the value of the first column of the first row of the
    /// resulting recordset.
    /// </summary>
    /// <param name="query">
    /// The query to be executed on the server.
    /// </param>
    /// <param name="set_parameters">
    /// A <see cref="Action{T}"/> that allows the caller to set the values
    /// of the parameters defined on the given query.
    /// </param>
    /// <param name="t">
    /// When this method returns contains the value of the first column of
    /// the first row of the recordset resulted from the execution of the
    /// <paramref name="query"/> if the recordset is not empty; otherwise,
    /// the default value for the type <typeparamref name="T"/>.
    /// </param>
    /// <returns>
    /// <c>true</c> if the recordset resulted from the execution of the
    /// <paramref name="query"/> is not empty; otherwise, <c>false</c>.
    /// </returns>
    bool ExecuteScalar<T>(string query, Action<CommandBuilder> set_parameters,
      out T t);

    /// <summary>
    /// Executes the command described by <see cref="query"/> on the server and
    /// returned the value of the first column of the first row of the
    /// resulting recordset.
    /// </summary>
    /// <param name="query">
    /// The query to be executed on the server.
    /// </param>
    /// <param name="command_type">
    /// The type of the command that is described by the
    /// <paramref name="query"/> parameter.
    /// </param>
    /// <param name="set_parameters">
    /// A <see cref="Action{T}"/> that allows the caller to set the values
    /// of the parameters defined on the given query.
    /// </param>
    /// <returns>
    /// The value of the first column of the first row of the recordset
    /// resulted from the execution of the <paramref name="query"/>.
    /// </returns>
    T ExecuteScalar<T>(string query, Action<CommandBuilder> set_parameters,
      CommandType command_type);

    /// <summary>
    /// Executes the command described by <see cref="query"/> on the server and
    /// returned the value of the first column of the first row of the
    /// resulting recordset.
    /// </summary>
    /// <param name="query">
    /// The query to be executed on the server.
    /// </param>
    /// <param name="command_type">
    /// The type of the command that is described by the
    /// <paramref name="query"/> parameter.
    /// </param>
    /// <param name="set_parameters">
    /// A <see cref="Action{T}"/> that allows the caller to set the values
    /// of the parameters defined on the given query.
    /// </param>
    /// <param name="t">
    /// When this method returns contains the value of the first column of
    /// the first row of the recordset resulted from the execution of the
    /// <paramref name="query"/> if the recordset is not empty; otherwise,
    /// the default value for the type <typeparamref name="T"/>.
    /// </param>
    /// <returns>
    /// <c>true</c> if the recordset resulted from the execution of the
    /// <paramref name="query"/> is not empty; otherwise, <c>false</c>.
    /// </returns>
    bool ExecuteScalar<T>(string query, Action<CommandBuilder> set_parameters,
      CommandType command_type, out T t);
  }
}
