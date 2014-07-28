using System;

namespace Nohros.Metrics
{
  public sealed class Histograms
  {
    public static UniformHistogram Uniform() {
      return new UniformHistogram(Samples.Uniform());
    }

    public static BiasedHistogram Biased() {
      return new BiasedHistogram(Samples.Biased());
    }
  }
}
