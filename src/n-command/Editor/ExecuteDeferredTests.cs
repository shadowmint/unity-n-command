#if N_COMMAND_TESTS

using N.Package.Bind;
using N.Package.Bind.Core;
using NUnit.Framework;
using N.Package.Command;
using N.Package.Core.Tests;
using N.Package.Events;
using N.Package.Events.Internal;
using NUnit.Framework.Constraints;

namespace Tests.N.Package.Command
{
  public class ExecuteDeferredTests : TestCase
  {
    [Test]
    public void test_deferred_event_can_be_executed_in_test_mode()
    {
      Registry.Default.Reset();
      Registry.Default.Register<UtilityCommandHandler>();
      Registry.Default.Register<IServiceRegistry, ServiceRegistry>(Registry.Default);

      EventManager.Instance.Clear(true);
      EventStream.Default.Dispose();

      var resolved = false;
      var command = new ExecuteDeferredCommand()
      {
        Command = new DelegateCommand(() => { resolved = true; }),
        Delay = 1f
      };

      var completed = false;
      command.OnCompleted(ep => completed = true);
      command.Execute(Registry.Default);

      Assert(!resolved);
      Assert(!completed);

      EventManager.Instance.Update(1f);

      Assert(resolved);
      Assert(completed);
    }

    [Test]
    public void test_can_create_and_run_commands()
    {
      Registry.Default.Reset();
      Registry.Default.Register<UtilityCommandHandler>();
      Registry.Default.Register<IServiceRegistry, ServiceRegistry>(Registry.Default);

      EventManager.Instance.Clear(true);
      EventStream.Default.Dispose();

      var subs = 0;
      var completed = false;

      var seq = new ExecuteSequenceCommand();
      seq.Add(new ExecuteDeferredCommand()
      {
        Command = new DelegateCommand(() =>
        {
          subs += 5;
          Assert(!completed);
        }),
        Delay = 1f
      });

      seq.Add(new ExecuteDeferredCommand()
      {
        Command = new DelegateCommand(() =>
        {
          subs += 10;
          Assert(!completed);
        }),
        Delay = 1f
      });

      seq.OnCompleted(ep => { completed = true; });
      seq.Execute(Registry.Default);

      EventManager.Instance.Update(1f);
      EventManager.Instance.Update(1f);

      Assert(completed);
      Assert(subs == 15);

      EventManager.Instance.Clear(true);
    }
  }
}

#endif