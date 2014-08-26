using System;

namespace Nohros.Metrics.Sql
{
  public partial class SqlMetricsDao
  {
    public void RegisterTag(string name, string value, long tags_id) {
      sql_query_executor_
        .ExecuteNonQuery(".mtc_add_tag",
          builder =>
            builder
              .AddParameter("@name", name)
              .AddParameter("@value", value)
              .AddParameter("@tags_id", tags_id));
    }
  }
}
