using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Transactions;
using Nohros.Data;
using Nohros.Data.SqlServer.Extensions;
using Nohros.Logging;
using Nohros.Resources;
using Nohros.Extensions;
using Nohros.Resources;

namespace Nohros.Data.SqlServer
{
  public class SetIfEqualsToQuery
  {
    const string kClassName = "Nohros.Data.SqlCe.SetIfGreaterThanQuery";

    readonly MustLogger logger_ = MustLogger.ForCurrentProcess;
    readonly SqlConnectionProvider sql_connection_provider_;

    /// <summary>
    /// Initializes a new instance of the <see cref="SetIfGreaterThanQuery"/>
    /// using the given <param ref="sql_connection_provider" />
    /// </summary>
    /// <param name="sql_connection_provider">
    /// A <see cref="SqlConnectionProvider"/> object that can be used to
    /// create connections to the data provider.
    /// </param>
    public SetIfEqualsToQuery(SqlConnectionProvider sql_connection_provider) {
      sql_connection_provider_ = sql_connection_provider;
      logger_ = MustLogger.ForCurrentProcess;
      SupressTransactions = true;
    }

    public bool Execute(string name, string table_name, object state) {
      using (var scope =
        new TransactionScope(SupressTransactions
          ? TransactionScopeOption.Suppress
          : TransactionScopeOption.Required)) {
        using (
          SqlConnection conn = sql_connection_provider_.CreateConnection())
        using (var builder = new CommandBuilder(conn)) {
          IDbCommand cmd = builder
            .SetText(@"
update " + table_name + @"
set state = @state" + @"
where state_name = @name
  and state = @state")
            .SetType(CommandType.Text)
            .AddParameter("@name", name)
            .AddParameterWithValue("@state", state)
            .Build();
          try {
            conn.Open();
            scope.Complete();
            return cmd.ExecuteNonQuery() > 0;
          } catch (SqlException e) {
            throw e.AsProviderException();
          }
        }
      }
    }

    public bool SupressTransactions { get; set; }
  }
}
