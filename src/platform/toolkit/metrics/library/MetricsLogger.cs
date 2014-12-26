using System;
using Nohros.Logging;

namespace Nohros.Metrics
{
  public class MetricsLogger : ForwardingLogger
  {
    static readonly MetricsLogger current_process_logger_;

    static MetricsLogger() {
      current_process_logger_ = new MetricsLogger(new NOPLogger());
    }

    public MetricsLogger(ILogger logger) : base(logger) {
    }

    public static MetricsLogger ForCurrentProcess { get; set; }
  }
}
