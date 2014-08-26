using System.Collections.Generic;
using Nohros.Data;

namespace Nohros.Metrics.Sql
{
  public partial class SqlMetricsDao
  {
    public IEnumerable<long> GetTagsIds(int hash, int count) {
      return sql_query_executor_
        .ExecuteQuery(".mtc_get_id_of_tags",
          reader => Mappers.Long().Map(reader, false),
          builder =>
            builder
              .AddParameter("@hash", hash)
              .AddParameter("@count", count));
    }
  }
}
