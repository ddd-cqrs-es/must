using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Transactions;
using Nohros.Data.SqlServer.Extensions;
using Nohros.Logging;
using Nohros.Resources;
using Nohros.Extensions;

namespace Nohros.Data.SqlServer
{
  public class GetStateQuery
  {
    const string kClassName = "Nohros.Data.SqlServer.GetStateQuery";

    readonly MustLogger logger_;
    readonly SqlConnectionProvider sql_connection_provider_;

    public GetStateQuery(SqlConnectionProvider sql_connection_provider) {
      sql_connection_provider_ = sql_connection_provider;
      logger_ = MustLogger.ForCurrentProcess;
      SupressTransactions = true;
    }

    public bool Execute<T>(string state_name, string table_name, out T state) {
      using (var scope =
        new TransactionScope(SupressTransactions
          ? TransactionScopeOption.Suppress
          : TransactionScopeOption.Required)) {
        using (SqlConnection conn = sql_connection_provider_.CreateConnection())
        using (var builder = new CommandBuilder(conn)) {
          IDbCommand cmd = builder
            .SetText("select state from " + table_name +
              " where state_name=@name")
            .SetType(CommandType.Text)
            .AddParameter("@name", state_name)
            .Build();
          try {
            conn.Open();
            object obj = cmd.ExecuteScalar();
            if (obj == null) {
              state = default(T);
              scope.Complete();
              return false;
            }
            state = (T) obj;
            scope.Complete();
            return true;
          } catch (SqlException e) {
            logger_.Error(
              StringResources.Log_MethodThrowsException.Fmt("Execute",
                kClassName),
              e);
            throw e.AsProviderException();
          }
        }
      }
    }

    public IEnumerable<T> Execute<T>(string state_name, string table_name) {
      using (var scope =
        new TransactionScope(SupressTransactions
          ? TransactionScopeOption.Suppress
          : TransactionScopeOption.Required)) {
        using (SqlConnection conn = sql_connection_provider_.CreateConnection())
        using (var builder = new CommandBuilder(conn)) {
          IDbCommand cmd = builder
            .SetText("select state from " + table_name +
              " where state_name like @name")
            .SetType(CommandType.Text)
            .AddParameter("@name", state_name)
            .Build();
          try {
            conn.Open();
            var list = new List<T>();
            using (IDataReader reader = cmd.ExecuteReader()) {
              while (reader.Read()) {
                list.Add((T) reader.GetValue(0));
              }
            }
            scope.Complete();
            return list;
          } catch (SqlException e) {
            logger_.Error(
              StringResources.Log_MethodThrowsException.Fmt("Execute",
                kClassName),
              e);
            throw e.AsProviderException();
          }
        }
      }
    }

    public IEnumerable<T> Execute<T>(string state_name, string table_name,
      int limit, bool remove) {
      using (var scope =
        new TransactionScope(SupressTransactions
          ? TransactionScopeOption.Suppress
          : TransactionScopeOption.Required)) {
        using (SqlConnection conn = sql_connection_provider_.CreateConnection())
        using (var builder = new CommandBuilder(conn)) {
          IDbCommand cmd = builder
            .SetText(GetText(table_name, true, remove))
            .SetType(CommandType.Text)
            .AddParameter("@name", state_name)
            .AddParameter("@limite", limit)
            .Build();
          try {
            conn.Open();
            var list = new List<T>();
            using (IDataReader reader = cmd.ExecuteReader()) {
              while (reader.Read()) {
                list.Add((T) reader.GetValue(0));
              }
            }
            scope.Complete();
            return list;
          } catch (SqlException e) {
            logger_.Error(
              StringResources.Log_MethodThrowsException.Fmt("Execute",
                kClassName),
              e);
            throw e.AsProviderException();
          }
        }
      }
    }

    string GetText(string table_name, bool likely, bool remove) {
      return remove
        ? "delete top(@limite) from " + table_name
          + " where state_name " + (likely ? "like" : "=") + " @name"
        : "select top(@limite) state from " + table_name
          + " where state_name " + (likely ? "like" : "=") + " @name";
    }

    public bool SupressTransactions { get; set; }
  }
}
