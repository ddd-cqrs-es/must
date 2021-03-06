﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlServerCe;
using System.Transactions;
using Nohros.Data;
using Nohros.Data.SqlCe.Extensions;
using Nohros.Extensions;
using Nohros.Logging;
using Nohros.Resources;

namespace Nohros.Data.SqlCe
{
  public class AddStateQuery
  {
    const string kClassName = "Nohros.Data.SqlServer.AddStateQuery";

    readonly MustLogger logger_ = MustLogger.ForCurrentProcess;
    readonly SqlCeConnectionProvider sql_connection_provider_;

    public AddStateQuery(SqlCeConnectionProvider sql_connection_provider) {
      sql_connection_provider_ = sql_connection_provider;
      logger_ = MustLogger.ForCurrentProcess;
      SupressTransactions = true;
    }

    public void Execute(string state_name, string table_name, object state) {
      using (
        new TransactionScope(SupressTransactions
          ? TransactionScopeOption.Suppress
          : TransactionScopeOption.Required)) {
        using (
          SqlCeConnection conn = sql_connection_provider_.CreateConnection())
        using (var builder = new CommandBuilder(conn)) {
          IDbCommand cmd = builder
            .SetText(@"
insert into " + table_name + @"(name, state)
values(@name, @state)")
            .SetType(CommandType.Text)
            .AddParameter("@name", state_name)
            .AddParameterWithValue("@state", state)
            .Build();
          try {
            conn.Open();
            cmd.ExecuteNonQuery();
          } catch (SqlCeException e) {
            throw e.AsProviderException();
          }
        }
      }
    }

    public bool SupressTransactions { get; set; }
  }
}
