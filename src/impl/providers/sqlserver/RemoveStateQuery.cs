using System;
using System.Data;
using System.Data.SqlClient;
using System.Transactions;
using Nohros.Data.SqlServer.Extensions;
using Nohros.Logging;
using Nohros.Resources;
using Nohros.Extensions;

namespace Nohros.Data.SqlServer
{
  public class RemoveStateQuery
  {
    const string kClassName = "Nohros.Data.SqlServer.RemoveStateQuery";

    readonly MustLogger logger_ = MustLogger.ForCurrentProcess;
    readonly SqlConnectionProvider sql_connection_provider_;

    public RemoveStateQuery(SqlConnectionProvider sql_connection_provider) {
      sql_connection_provider_ = sql_connection_provider;
      logger_ = MustLogger.ForCurrentProcess;
      SupressTransactions = true;
    }

    public bool Execute<T>(string state_name, string table_name) {
      using (var scope =
        new TransactionScope(SupressTransactions
          ? TransactionScopeOption.Suppress
          : TransactionScopeOption.Required)) {
        using (SqlConnection conn = sql_connection_provider_.CreateConnection())
        using (var builder = new CommandBuilder(conn)) {
          IDbCommand cmd = builder
            .SetText("delete from " + table_name +
              " where state_name=@name")
            .SetType(CommandType.Text)
            .AddParameter("@name", state_name)
            .Build();
          try {
            conn.Open();
            int affected = cmd.ExecuteNonQuery();
            scope.Complete();
            return affected != 0;
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

    public bool SupressTransactions { get; set; }
  }
}
