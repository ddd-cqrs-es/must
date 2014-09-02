using System;
using System.Data;

namespace Nohros.Metrics.Sql
{
  public partial class SqlMetricsDao
  {
    public void RegisterMeasure(long serie_id, double value, DateTime timestamp) {
      sql_query_executor_
        .ExecuteNonQuery(".mtc_add_measure",
          builder =>
            builder
              .AddParameter("@measure", value, DbType.Double)
              .AddParameter("@timestamp", timestamp)
              .AddParameter("@serie_id", serie_id));
    }
  }
}
