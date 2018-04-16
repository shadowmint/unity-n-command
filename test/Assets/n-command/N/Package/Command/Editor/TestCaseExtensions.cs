using System;
using System.Threading.Tasks;
using N.Package.Test;

namespace N.Package.Command.Editor
{
  public static class TestCaseExtensions
  {
    public static void RunAsyncTest(this TestCase self, Func<Task> task)
    {
      task().Wait();
    }
  }
}