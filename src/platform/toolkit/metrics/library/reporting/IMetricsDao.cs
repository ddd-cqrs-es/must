using System;
using System.Collections.Generic;

namespace Nohros.Metrics.Reporting
{
  public interface IMetricsDao
  {
    IEnumerable<long> GetTagsIds(int hash, int count);

    long RegisterTags(int hash, int count);
    void RegisterTag(long tags_id, string name, string value);
    bool ContainsTag(long tags_id, string name, string value);
    void RegisterMeasure(long tags_id, double value, DateTime timestamp);
  }
}
