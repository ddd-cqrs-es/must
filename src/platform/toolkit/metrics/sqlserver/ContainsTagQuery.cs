using System;

namespace Nohros.Metrics.Sql
{
  public partial class SqlMetricsDao
  {
    public bool ContainsTag(string name, string value, long tags_id) {
      long tag_id;
      return
        sql_query_executor_
          .ExecuteScalar(".mtc_get_id_of_tag",
            builder =>
              builder
                .AddParameter("@tags_id", tags_id)
                .AddParameter("@name", name)
                .AddParameter("@value", value), out tag_id);
    }
  }
}
