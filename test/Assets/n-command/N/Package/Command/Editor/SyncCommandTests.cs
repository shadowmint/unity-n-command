#if N_COMMAND_TESTS

using System.Threading.Tasks;
using N.Package.Test;
using N.Packages.Promises;
using NUnit.Framework;

namespace N.Package.Command.Editor
{
  public class TestCommandSync : ICommand
  {
  }

  public class TestCommandSyncHandler : ICommandHandler<TestCommandSync>
  {
    public Task Execute(TestCommandSync command)
    {
      return this.Resolve();
    }
  }

  public class TestCommandSyncWithValue : ICommand<string>
  {
  }

  public class TestCommandSyncWithValueHandler : ICommandHandler<TestCommandSyncWithValue, string>
  {
    public Task<string> Execute(TestCommandSyncWithValue command)
    {
      return this.Resolve("Hello World");
    }
  }

  public class SyncCommandTests : TestCase
  {
    [Test]
    public void TestRunCommandSync()
    {
      var completed = false;
      var handler = new TestCommandSyncHandler();
      var promise = handler.Execute(new TestCommandSync()).Promise().Then(() => { completed = true; });
      Assert(promise.Check());
      Assert(completed);
    }
    
    [Test]
    public void TestRunCommandSyncWithValue()
    {
      var completed = "";
      var handler = new TestCommandSyncWithValueHandler();
      var promise = handler.Execute(new TestCommandSyncWithValue()).Promise().Then((v) => { completed = v; });
      Assert(promise.Check());
      Assert(completed == "Hello World");
    }
  }
}

#endif