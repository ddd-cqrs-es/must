using System;
using NUnit.Framework;
using Telerik.JustMock;

namespace Nohros.Logging.Tests
{
  public class ChainedLoggerTests
  {
    [Test]
    public void should_call_each_logger_in_chain() {
      var logger = Mock.Create<ILogger>();
      Mock
        .Arrange(() => logger.Info(Arg.IsAny<string>()))
        .OccursOnce();

      var logger2 = Mock.Create<ILogger>();
      Mock
        .Arrange(() => logger2.Info(Arg.IsAny<string>()))
        .OccursOnce();

      var chain = new ChainedLogger(new ILogger[] {logger, logger2});
      chain.Info("My log message");

      Mock.AssertAll(logger);
      Mock.AssertAll(logger2);
    }
  }
}
