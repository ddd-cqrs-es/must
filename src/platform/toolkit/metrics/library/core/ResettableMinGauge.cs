using System;

namespace Nohros.Metrics
{
  public class ResettableMinGauge : IMetric, IResettable
  {
    readonly MinGauge min_gauge_;

    public ResettableMinGauge(MetricConfig config) {
      min_gauge_ = new MinGauge(config);
    }

    protected internal Measure Compute() {
      return min_gauge_.Compute();
    }

    public MetricConfig Config {
      get { return min_gauge_.Config; }
    }

    public void GetMeasure(Action<Measure> callback) {
      min_gauge_.GetMeasure(callback);
    }

    public void GetMeasure<T>(Action<Measure, T> callback, T state) {
      min_gauge_.GetMeasure(callback, state);
    }

    public void Reset() {
      min_gauge_.Reset();
    }

    public void Update(long v) {
      min_gauge_.Update(v);
    }
  }
}
