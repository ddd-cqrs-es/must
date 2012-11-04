﻿using System;
using System.Collections.Generic;

namespace Nohros.Metrics
{
  public class AbstractMetricsRegistry
  {
    const int kExpectedMetricCount = 1024;
    readonly Dictionary<MetricName, IMetric> metrics_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="MetricsRegistry"/> class.
    /// </summary>
    protected AbstractMetricsRegistry() {
      metrics_ = new Dictionary<MetricName, IMetric>(kExpectedMetricCount);
    }
    #endregion

    /// <summary>
    /// Gets the counter that is associated with the specified
    /// <see cref="MetricName"/> or create a new one if no association exists.
    /// </summary>
    /// <param name="name">
    /// The name of the metric.
    /// </param>
    /// <returns>
    /// A <see cref="Counter"/> that could be identified by the specified
    /// <paramref name="name"/>.
    /// </returns>
    public virtual Counter GetCounter(MetricName name) {
      Counter counter;
      if (!TryGetMetric(name, out counter)) {
        counter = new Counter();
        Add(name, counter);
      }
      return counter;
    }

    /// <summary>
    /// Given a new <see cref="Gauge{T}"/>, registers it under the given metric
    /// name.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name">
    /// The name of the metric.
    /// </param>
    /// <param name="metric">
    /// The gauge metric to be added.
    /// </param>
    /// <returns></returns>
    public virtual void AddGauge<T>(MetricName name, Gauge<T> metric) {
      Gauge<T> gauge;
      if (!TryGetMetric(name, out gauge)) {
        Add(name, gauge);
      }
    }

    /// <param name="name">
    /// The name of the metric to get.
    /// </param>
    /// <param name="gauge">
    /// When this method returns, contains the gauge associated with the
    /// specified metric name, if a metric name is found; otherwise, the
    /// <c>null</c>.
    /// </param>
    /// <returns>
    /// <c>true</c> if a <see cref="Gauge{T}"/> associated with the
    /// <paramref name="name"/> exists; otherwise, <c>false</c>.
    /// </returns>
    public virtual bool TryGetGauge<T>(MetricName name, out Gauge<T> gauge) {
      return TryGetMetric(name, out gauge);
    }

    /// <param name="name">
    /// The name of the metric to get.
    /// </param>
    /// <param name="counter">
    /// When this method returns, contains the counter associated with the
    /// specified metric name, if a metric name is found; otherwise, the
    /// <c>null</c>.
    /// </param>
    /// <returns>
    /// <c>true</c> if a <see cref="Counter"/> associated with the
    /// <paramref name="name"/> exists; otherwise, <c>false</c>.
    /// </returns>
    public bool TryGetCounter(MetricName name, out Counter counter) {
      return TryGetMetric(name, out counter);
    }

    /// <param name="name">
    /// The name of the metric to get.
    /// </param>
    /// <param name="metric">
    /// When this method returns, contains the timer associated with the
    /// specified metric name, if a metric name is found; otherwise, the
    /// <c>null</c>.
    /// </param>
    /// <returns>
    /// <c>true</c> if a <see cref="Counter"/> associated with the
    /// <paramref name="name"/> exists; otherwise, <c>false</c>.
    /// </returns>
    public bool TryGetMetric<T>(MetricName name, out T metric)
      where T : class, IMetric {
      IMetric i_metric;
      if (metrics_.TryGetValue(name, out i_metric)) {
        metric = i_metric as T;
        return metric != null;
      }
      metric = null;
      return false;
    }

    /// <inheritdoc/>
    public virtual void Add(MetricName name, IMetric metric) {
      metrics_.Add(name, metric);
      OnMetricAdded(name, metric);
    }

    /// <summary>
    /// Raised when a new metric is added to the registry.
    /// </summary>
    public virtual event MetricAddedEventHandler MetricAdded;

    protected void OnMetricAdded(MetricName name, IMetric metric) {
      Listeners.SafeInvoke(MetricAdded,
        (MetricAddedEventHandler handler) => handler(name, metric));
    }

    /// <inheritdoc/>
    public virtual IMetric this[MetricName name] {
      get {
        if (name == null) {
          throw new ArgumentNullException("name");
        }
        IMetric metric;
        if (!TryGetMetric(name, out metric)) {
          throw new KeyNotFoundException("name");
        }
        return metric;
      }
      set {
        if (name == null) {
          throw new ArgumentNullException("name");
        }
        metrics_[name] = value;
      }
    }
  }
}
