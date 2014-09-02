using System.Collections.Generic;
using Nohros.Data;

namespace Nohros.Metrics.Sql
{
  public partial class SqlMetricsDao
  {
    public IEnumerable<long> GetSeriesIds(string name, int hash, int count) {
      return sql_query_executor_
        .ExecuteQuery(".mtc_get_id_of_serie",
          reader => Mappers.Long().Map(reader, false),
          builder =>
            builder
              .AddParameter("@name", name)
              .AddParameter("@hash", hash)
              .AddParameter("@tags_count", count));
    }
  }
}
