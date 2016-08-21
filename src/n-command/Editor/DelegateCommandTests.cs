#if N_COMMAND_TESTS

using NUnit.Framework;
using N.Package.Command;
using N.Package.Core.Tests;
using N.Package.Events;
using N.Package.Events.Internal;

namespace Tests.N.Package.Command
{
  public class DelegateCommandTests : TestCase
  {
    [Test]
    public void test_delegate_command()
    {
      var completed = false;

      var cmd = new DelegateCommand(() =>
      {
        completed = true;
      });

      cmd.Execute();
      Assert(completed);
    }

    [Test]
    public void test_delegate_command_async()
    {
      //EventStream.Default.
      EventManager.Instance.Clear(true);
      var completed = false;

      var cmd = new DelegateCommand(() =>
      {
        completed = true;
      }, 1f);

      cmd.Execute();
      Assert(!completed);

      EventManager.Instance.Update(1f);
      Assert(completed);

      EventManager.Instance.Clear(true);
    }
  }
}

#endif