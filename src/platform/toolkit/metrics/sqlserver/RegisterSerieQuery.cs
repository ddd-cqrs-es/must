using System;

namespace Nohros.Metrics.Sql
{
  public partial class SqlMetricsDao
  {
    public long RegisterSerie(string name, int hash, int count) {
      return sql_query_executor_
        .ExecuteScalar<long>(schema_ + ".mtc_add_serie",
          builder =>
            builder
              .AddParameter("@name", name)
              .AddParameter("@hash", hash)
              .AddParameter("@count", count));
    }
  }
}
