using System;

namespace Nohros.Metrics.Sql
{
  public partial class SqlMetricsDao
  {
    public bool ContainsTag(string name, string value, long tags_id) {
      bool contains;
      sql_query_executor_
        .ExecuteScalar(".mtc_serie_contains_tag",
          builder =>
            builder
              .AddParameter("@serie_id", tags_id)
              .AddParameter("@name", name)
              .AddParameter("@value", value), out contains);
      return contains;
    }
  }
}
