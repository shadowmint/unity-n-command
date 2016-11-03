#if N_COMMAND_TESTS

using N.Package.Bind;
using NUnit.Framework;
using N.Package.Command;
using N.Package.Events;
using N.Package.Core.Tests;

namespace Tests.N.Package.Command
{
  public class TestCommandSync : ICommand
  {
    private readonly EventHandler _eventHandler = new EventHandler();

    public EventHandler EventHandler
    {
      get { return _eventHandler; }
    }

    public bool CanExecute()
    {
      return true;
    }

    public void Execute()
    {
      this.Completed();
    }
  }

  public class CommandTests : TestCase
  {
    [Test]
    public void test_can_create_and_run_commands()
    {
      var completed = false;
      var command = new TestCommandSync();
      command.OnCompleted((ep) => { completed = true; });
      command.Execute();
      Assert(completed);
    }
  }
}

#endif