#if N_COMMAND_TESTS

using System.Threading.Tasks;
using N.Package.Test;
using NUnit.Framework;

namespace N.Package.Command.Editor
{
  public class TestCommandAsync : ICommand
  {
  }

  public class TestCommandAsyncHandler : ICommandHandler<TestCommandAsync>
  {
    public Task Execute(TestCommandAsync command)
    {
      return this.Resolve();
    }
  }

  public class AsyncCommandTests : TestCase
  {
    [Test]
    public void TestRunCommandAsync()
    {
      this.RunAsyncTest(async () =>
      {
        var handler = new TestCommandAsyncHandler();
        await handler.Execute(new TestCommandAsync());
      });
    }
  }
}

#endif