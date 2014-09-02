using System;

namespace Nohros.Metrics.Sql
{
  public partial class SqlMetricsDao
  {
    public void RegisterTag(string name, string value, long serie_id) {
      sql_query_executor_
        .ExecuteNonQuery(".mtc_add_tag",
          builder =>
            builder
              .AddParameter("@name", name)
              .AddParameter("@value", value)
              .AddParameter("@serie_id", serie_id));
    }
  }
}
