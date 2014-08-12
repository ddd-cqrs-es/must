using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Nohros.Metrics
{
  /// <summary>
  /// A set of <see cref="Measure"/>.
  /// </summary>
  public struct MetricValueSet : IEnumerable<Measure>
  {
    readonly IMetric metric_;
    readonly Measure[] values_;

    /// <summary>
    /// Initializes a new instance of the <see cref="MetricValueSet"/> class
    /// by using the given metric and a instantaneous values.
    /// </summary>
    /// <param name="metric">
    /// The metric that produces the <paramref name="value"/>.
    /// </param>
    /// <param name="value">
    /// A instantaneous value of the given <paramref name="metric"/>.
    /// </param>
    public MetricValueSet(IMetric metric, Measure value)
      : this(metric, new[] {value}) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MetricValueSet"/> class
    /// by using the given <see cref="metric"/> and its associated 
    /// instantaneous values.
    /// </summary>
    /// <param name="metric">
    /// The metric that produces the <paramref name="values"/>.
    /// </param>
    /// <param name="values">
    /// A collection of <see cref="Measure"/> containing instantaneous
    /// values of the given <paramref name="metric"/>.
    /// </param>
    public MetricValueSet(IMetric metric, Measure[] values) {
      metric_ = metric;
      values_ = values;
    }

    IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }

    /// <summary>
    /// Gets a enumerator that iterates through the metric's values of the set.
    /// </summary>
    /// <returns>
    /// A enumerator that iterates through the metric's values of the set.
    /// </returns>
    public IEnumerator<Measure> GetEnumerator() {
      foreach (var value in values_) {
        yield return value;
      }
    }

    /// <summary>
    /// Gets the associated <see cref="IMetric"/>.
    /// </summary>
    public IMetric Metric {
      get { return metric_; }
    }

    /// <summary>
    /// Gets a collection of instanteneous values.
    /// </summary>
    public ReadOnlyCollection<Measure> Values {
      get { return new ReadOnlyCollection<Measure>(values_); }
    }
  }
}
