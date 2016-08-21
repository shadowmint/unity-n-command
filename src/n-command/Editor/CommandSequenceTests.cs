#if N_COMMAND_TESTS

using NUnit.Framework;
using N.Package.Command;
using N.Package.Core.Tests;

namespace Tests.N.Package.Command
{
  public class CommandSequenceTests : TestCase
  {
    [Test]
    public void test_can_create_and_run_commands()
    {
      var subs = 0;
      var completed = false;

      var seq = new CommandSequence();
      seq.Add(new DelegateCommand(() =>
      {
        subs += 5;
        Assert(!completed);
      })).Add(new DelegateCommand(() =>
      {
        subs += 5;
        Assert(!completed);
      }));

      seq.OnCompleted((ep) => { completed = true; });
      seq.Execute();

      Assert(completed);
      Assert(subs == 10);
    }
  }
}

#endif