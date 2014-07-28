using System;
using NUnit.Framework;
using Nohros.Concurrent;

namespace Nohros.Metrics
{
  [TestFixture]
  public class AsyncCounterTest
  {
    [Test]
    public void should_start_counting_at_zero() {
      var counter = new AsyncCounter();
      long count = -1;
      counter.GetCount((l, timestamp) => count = l);
      Assert.That(count, Is.EqualTo(0));
    }

    [Test]
    public void should_increment_counter_by_one() {
      var counter = new AsyncCounter();
      long count = 9;
      counter.Increment(c => count = c.Count);
      Assert.That(count, Is.EqualTo(1));
    }

    [Test]
    public void should_increment_counter_by_given_delta() {
      var counter = new AsyncCounter();
      long count = 3;
      counter.Increment(15, c => count = c.Count);
      Assert.That(count, Is.EqualTo(15));
    }

    [Test]
    public void should_decrement_counter_by_one() {
      var counter = new AsyncCounter();
      long count = 10;
      counter.Decrement(c => count = c.Count);
      Assert.That(count, Is.EqualTo(-1));
    }

    [Test]
    public void should_decrement_counter_by_given_delta() {
      var counter = new AsyncCounter();
      long count = 15;
      counter.Decrement(12, c => count = c.Count);
      Assert.That(count, Is.EqualTo(-12));
    }
  }
}
