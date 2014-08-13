using System;

using Nohros.Concurrent;

namespace Nohros.Tests
{
  public static class Program
  {
    public static void Main() {
      long received_long = 0;
      var mailbox = new Mailbox<long>(Console.WriteLine);

      for (int i = 0, j = 15; i < j; i++) {
        mailbox.Send(i);
      }
      Console.ReadLine();
    }

    public static string Desencrypta(string mSTRING, string mSENHA) {
      int length;
      int[] numArray = new int[4];
      numArray[0] = 112;
      numArray[1] = 95;
      numArray[2] = 127;
      numArray[3] = 77;
      int[] numArray1 = numArray;
      mSTRING = mSTRING.Trim().Replace("''", "'");
      string str = "";
      int num = mSTRING.Length - 1;
      for (int i = 0; i <= num; i++) {
        if (mSTRING.Substring(i, 1)[0] >= 40) {
          int num1 = mSTRING.Substring(i, 1)[0] - 40 + 216;
          if (mSENHA.Length > 0) {
            length = i % mSENHA.Length;
            num1 = num1 - mSENHA.Substring(length, 1)[0];
          }
          length = i % (int)numArray1.Length;
          num1 = num1 - numArray1[length];
          if (num1 < 40) {
            num1 = num1 + 216;
          }
          if (num1 > 256) {
            num1 = num1 - 216;
          }
          str = string.Concat(str, num1);
        } else {
          str = string.Concat(str, mSTRING.Substring(i, 1));
        }
      }
      return str;
    }
  }
}
