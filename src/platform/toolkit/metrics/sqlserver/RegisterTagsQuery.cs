using System;

namespace Nohros.Metrics.Sql
{
  public partial class SqlMetricsDao
  {
    public long RegisterTags(int hash, int count) {
      return sql_query_executor_
        .ExecuteScalar<long>(schema_ + ".mtc_add_tags",
          builder =>
            builder
              .AddParameter("@hash", hash)
              .AddParameter("@count", count));
    }
  }
}
