#if N_COMMAND_TESTS

using N.Package.Bind;
using N.Package.Bind.Core;
using NUnit.Framework;
using N.Package.Command;
using N.Package.Core.Tests;

namespace Tests.N.Package.Command
{
  public class ExecuteParallelCommandTests : TestCase
  {
    [Test]
    public void test_can_create_and_run_commands()
    {
      Registry.Default.Reset();
      Registry.Default.Register<UtilityCommandHandler>();
      Registry.Default.Register<IServiceRegistry, ServiceRegistry>(Registry.Default);

      var subs = 0;
      var completed = false;

      var seq = new ExecuteParallelCommand();
      seq.Add(new DelegateCommand(() =>
      {
        subs += 5;
        Assert(!completed);
      })).Add(new DelegateCommand(() =>
      {
        subs += 5;
        Assert(!completed);
      }));

      seq.OnCompleted(ep => { completed = true; });
      seq.Execute(Registry.Default);

      Assert(completed);
      Assert(subs == 10);
    }
  }
}

#endif