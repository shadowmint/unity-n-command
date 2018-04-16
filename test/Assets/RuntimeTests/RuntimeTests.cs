using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using N.Package.Bind;
using N.Package.Command;
using N.Package.Test.Runtime;
using N.Packages.Promises;
using UnityEngine;

public class RuntimeTests : RuntimeTest
{
  private ServiceRegistry _registry;

  private void Start()
  {
    _registry = new ServiceRegistry();
    _registry.Register<DeferredCommandHandler>();
  }

  [RuntimeTest]
  public void CommandWithNoHandlerShouldFail()
  {
    new DeferredCommandWithNoHandler()
      .Execute(_registry)
      .Promise()
      .Then(Unreachable, (err) => Completed())
      .Dispatch();
  }

  [RuntimeTest]
  public void ResolveCommandAfterDelay()
  {
    new DeferredCommand() {Value = "Hi!"}
      .Execute<DeferredCommand, string>(_registry)
      .Promise()
      .Then((output) =>
      {
        Log($"Got: {output}");
        Completed();
      }, Failed)
      .Dispatch();
  }

  [RuntimeTest]
  public void RunSpamCommands()
  {
    var passed = 0;
    var failed = 0;
    var runner = _registry.Resolve<ICommandHandler<SpamCommand>>();

    var tasks = new List<Task>();
    Action maybeResolved = () =>
    {     
      if (passed + failed >= 100)
      {
        Log($"{passed} passed, {failed} failed (should be ~50/50)");
        Completed();
      }
    };

    for (var i = 0; i < 100; i++)
    {
      var task = runner.Execute(new SpamCommand());
      Assert(task != null);
      tasks.Add(task);
      task.Promise().Then(() =>
      {
        passed += 1;
        maybeResolved();
      }, (err) =>
      {
        failed += 1;
        maybeResolved();
      }).Dispatch();
    }
  }
}