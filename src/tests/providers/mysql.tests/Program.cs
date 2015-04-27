using System;

using Nohros.Concurrent;
using mysql.tests;

namespace Nohros.Tests
{
  public static class Program
  {
    public static void Main() {
      var mapper = new QueryMapper();
      mapper.MySql();
      Console.ReadLine();
    }
  }
}
