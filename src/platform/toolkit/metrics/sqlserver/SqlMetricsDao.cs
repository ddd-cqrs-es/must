using System;
using System.Data;
using Nohros.Data.SqlServer;

namespace Nohros.Metrics.Sql
{
  public partial class SqlMetricsDao : IMetricsDao
  {
    readonly SqlConnectionProvider sql_connection_provider_;
    readonly SqlQueryExecutor sql_query_executor_;
    readonly string schema_;

    public SqlMetricsDao(SqlConnectionProvider sql_connection_provider) {
      sql_connection_provider_ = sql_connection_provider;
      sql_query_executor_ = new SqlQueryExecutor(sql_connection_provider,
        CommandType.StoredProcedure);
      schema_ = sql_connection_provider_.Schema;
    }
  }
}
