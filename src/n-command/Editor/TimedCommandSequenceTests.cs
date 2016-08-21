#if N_COMMAND_TESTS

using NUnit.Framework;
using N.Package.Command;
using N.Package.Core.Tests;
using N.Package.Events.Internal;

namespace Tests.N.Package.Command
{
  public class TimedCommandSequenceTests : TestCase
  {
    [Test]
    public void test_can_create_and_run_commands()
    {
      EventManager.Instance.Clear(true);

      var subs = 0;
      var completed = false;

      var seq = new TimedCommandSequence();
      seq.Add(new DelegateCommand(() =>
      {
        subs += 5;
        Assert(!completed);
      }), 1f).Add(new DelegateCommand(() =>
      {
        subs += 5;
        Assert(!completed);
      }), 1f);

      seq.OnCompleted((ep) => { completed = true; });
      seq.Execute();

      EventManager.Instance.Update(1f);
      EventManager.Instance.Update(1f);

      Assert(completed);
      Assert(subs == 10);

      EventManager.Instance.Clear(true);
    }
  }
}

#endif