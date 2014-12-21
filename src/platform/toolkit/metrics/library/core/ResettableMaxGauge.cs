using System;

namespace Nohros.Metrics
{
  public class ResettableMaxGauge : IMetric, IResettable
  {
    readonly MaxGauge max_gauge_;

    public ResettableMaxGauge(MetricConfig config) {
      max_gauge_ = new MaxGauge(config);
    }

    protected internal Measure Compute() {
      return max_gauge_.Compute();
    }

    public MetricConfig Config {
      get { return max_gauge_.Config; }
    }

    public void GetMeasure(Action<Measure> callback) {
      max_gauge_.GetMeasure(callback);
    }

    public void GetMeasure<T>(Action<Measure, T> callback, T state) {
      max_gauge_.GetMeasure(callback, state);
    }

    public void Reset() {
      max_gauge_.Reset();
    }

    public void Update(long v) {
      max_gauge_.Update(v);
    }
  }
}
