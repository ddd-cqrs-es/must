using System;

namespace Nohros.Metrics
{
  public sealed class ExponentialWeightedMovingAverages
  {
    const int kInterval = 5;
    const double kMinutesPerSecond = 60.0;
    const int kOneMinute = 1;
    const int kFiveMinutes = 5;
    const int kFifteenMinutes = 15;

    static readonly double kM1Alpha = 1 -
      Math.Exp(-kInterval/kMinutesPerSecond/kOneMinute);

    static readonly double kM5Alpha = 1 -
      Math.Exp(-kInterval/kMinutesPerSecond/kFiveMinutes);

    static readonly double kM15Alpha = 1 -
      Math.Exp(-kInterval/kMinutesPerSecond/kFifteenMinutes);

    public static ExponentialWeightedMovingAverage OneMinute() {
      return new ExponentialWeightedMovingAverage(
        kM1Alpha, kInterval, TimeUnit.Seconds);
    }

    public static ExponentialWeightedMovingAverage FiveMinute() {
      return new ExponentialWeightedMovingAverage(
        kM5Alpha, kInterval, TimeUnit.Seconds);
    }

    public static ExponentialWeightedMovingAverage FifteenMinute() {
      return new ExponentialWeightedMovingAverage(
        kM15Alpha, kInterval, TimeUnit.Seconds);
    }
  }
}
