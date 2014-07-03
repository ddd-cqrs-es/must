using System;
using System.Data;
using System.Data.SqlClient;
using Nohros.Data.SqlServer.Extensions;
using Nohros.Extensions;
using Nohros.Logging;
using Nohros.Resources;

namespace Nohros.Data.SqlServer
{
  /// <summary>
  /// Execute SQL commands againts a SQL Server database.
  /// </summary>
  public class SqlQueryExecutor
  {
    const string kClassName = "Nohros.Data.SqlServer.SqlQueryExecutor";

    readonly SqlConnectionProvider sql_connection_provider_;
    readonly CommandType default_command_type_;
    readonly MustLogger logger_;

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlQueryExecutor"/> class
    /// by using the given sql connection provider and default command type.
    /// </summary>
    /// <param name="sql_connection_provider">
    /// A <see cref="SqlConnectionProvider"/> that can be used to create
    /// connections for a sql server.
    /// </param>
    /// <param name="default_command_type">
    /// The <see cref="CommandType"/> to be used when executing a
    /// method that does not contain a parameter of that type.
    /// </param>
    public SqlQueryExecutor(SqlConnectionProvider sql_connection_provider,
      CommandType default_command_type = CommandType.Text) {
      sql_connection_provider_ = sql_connection_provider;
      default_command_type_ = default_command_type;
      logger_ = MustLogger.ForCurrentProcess;
    }

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
    /// <param name="mapper">
    /// A <see cref="Func{T, TResult}"/> that maps the <see cref="IDataReader"/>
    /// created with the result of the given <paramref name="query"/> to a
    /// object of the type <typeparamref name="T"/>.
    /// </param>
    /// <returns>
    /// The object produced by the execution of the method
    /// <paramref name="mapper"/>.
    /// </returns>
    public T ExecuteQuery<T>(string query, Func<IDataReader, T> mapper) {
      return ExecuteQuery(query, mapper, builder => { },
        default_command_type_);
    }

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
    /// <param name="mapper">
    /// A <see cref="Func{T, TResult}"/> that maps the <see cref="IDataReader"/>
    /// created with the result of the given <paramref name="query"/> to a
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
    public T ExecuteQuery<T>(string query,
      Func<IDataReader, T> mapper, CommandType command_type) {
      return ExecuteQuery(query, mapper, builder => { }, command_type);
    }

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
    /// <param name="mapper">
    /// A <see cref="Func{T, TResult}"/> that maps the <see cref="IDataReader"/>
    /// created with the result of the given <paramref name="query"/> to a
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
    public T ExecuteQuery<T>(string query, Func<IDataReader, T> mapper,
      Action<CommandBuilder> set_parameters) {
      return ExecuteQuery(query, mapper, set_parameters, default_command_type_);
    }

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
    /// A <see cref="Func{T, TResult}"/> that maps the <see cref="IDataReader"/>
    /// created with the result of the given <paramref name="query"/> to a
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
    public T ExecuteQuery<T>(string query, Func<IDataReader, T> mapper,
      Action<CommandBuilder> set_parameters, CommandType command_type) {
      using (SqlConnection conn = sql_connection_provider_.CreateConnection())
      using (var builder = new CommandBuilder(conn)) {
        IDbCommand cmd = builder
          .SetText(query)
          .SetType(command_type)
          .Set(set_parameters)
          .Build();
        try {
          conn.Open();
          using (IDataReader reader = cmd.ExecuteReader()) {
            return mapper(reader);
          }
        } catch (SqlException e) {
          logger_.Error(
            StringResources
              .Log_MethodThrowsException
              .Fmt("ExecuteQuery", kClassName), e);
          throw e.AsProviderException();
        }
      }
    }

    /// <summary>
    /// Executes the command described by <see cref="query"/> on the server and
    /// runs the number of rows affected.
    /// </summary>
    /// <param name="query">
    /// The query to be executed on the server.
    /// </param>
    /// <returns>
    /// The number of rows affected.
    /// </returns>
    public int ExecuteNonQuery(string query) {
      return ExecuteNonQuery(query, builder => { }, default_command_type_);
    }

    /// <summary>
    /// Executes the command described by <see cref="query"/> on the server and
    /// runs the number of rows affected.
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
    public int ExecuteNonQuery(string query, CommandType command_type) {
      return ExecuteNonQuery(query, builder => { }, command_type);
    }

    /// <summary>
    /// Executes the command described by <see cref="query"/> on the server and
    /// runs the number of rows affected.
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
    public int ExecuteNonQuery(string query,
      Action<CommandBuilder> set_parameters) {
      return ExecuteNonQuery(query, set_parameters, default_command_type_);
    }

    /// <summary>
    /// Executes the command described by <see cref="query"/> on the server and
    /// runs the number of rows affected.
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
    public int ExecuteNonQuery(string query,
      Action<CommandBuilder> set_parameters, CommandType command_type) {
      using (SqlConnection conn = sql_connection_provider_.CreateConnection())
      using (var builder = new CommandBuilder(conn)) {
        IDbCommand cmd = builder
          .SetText(query)
          .SetType(command_type)
          .Set(set_parameters)
          .Build();
        try {
          conn.Open();
          return cmd.ExecuteNonQuery();
        } catch (SqlException e) {
          logger_.Error(
            StringResources
              .Log_MethodThrowsException
              .Fmt("ExecuteNonQuery", kClassName), e);
          throw e.AsProviderException();
        }
      }
    }
  }
}
