using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Nohros.Concurrent;
using Nohros.Extensions;

namespace Nohros.Metrics.Tests
{
  public class Program
  {
    static long count_ ;
    public static void Main(string[] args) {
      var test = new Test();
      var watch = new Stopwatch();

      var measures = new List<double>();
      for (int p = 1; p < Environment.ProcessorCount + 1; ++p) {
        for (int i = 0; i < 10; i++) {
          test.Run(watch, p);
          measures.Add(watch.ElapsedMilliseconds);
        }
        Console.WriteLine("With {0} threads:{1}".Fmt(p, measures.Average()));
      }
      Console.WriteLine("Done");
      Console.WriteLine(count_);
      Console.ReadLine();
    }

    public class Test
    {
      Mailbox<int> mailbox_1_;
      ConcurrentQueue<int> concurrent_;
      readonly AutoResetEvent sync_;

      public Test() {
        sync_ = new AutoResetEvent(false);
      }

      public void Run(Stopwatch watch, int concurrency) {
        mailbox_1_ = new Mailbox<int>(message => { });
        //concurrent_ = new ConcurrentQueue<int>();

        for (int i = 0; i < concurrency - 1; i++) {
          new BackgroundThreadFactory()
            .CreateThread(Loop)
            .Start();
        }
        //mailbox_1_.Send(() => Start(watch));
        Start(watch);
        Loop();
        Stop(watch);
        mailbox_1_.Shutdown();
        //mailbox_1_.Send(() => Stop(watch));
        //sync_.WaitOne();
      }

      void Start(Stopwatch watch) {
        watch.Restart();
      }

      void Stop(Stopwatch watch) {
        watch.Stop();
        sync_.Set();
      }

      void Loop() {
        for (int i = 0; i < 10000000; i++) {
          mailbox_1_.Send(i);
          //Interlocked.Increment(ref count_);
          //Interlocked.Increment(ref count_);
          //Interlocked.Increment(ref count_);
          //concurrent_.Enqueue(i);
        }
      }
    }
  }
}
