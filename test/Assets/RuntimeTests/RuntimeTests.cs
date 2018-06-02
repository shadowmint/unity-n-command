using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using N.Package.Bind;
using N.Package.Command;
using N.Package.Test.Runtime;
using N.Package.Promises;
using packages.N.Package.Command;
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
      .ExecuteAsync(_registry)
      .Promise()
      .Then(Unreachable, (err) => Completed())
      .Dispatch();
  }

  [RuntimeTest]
  public void ResolveCommandAfterDelay()
  {
    new DeferredCommand() {Value = "Hi!"}
      .ExecuteAsync<DeferredCommand, string>(_registry)
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

    Action maybeResolved = () =>
    {
      if (passed + failed >= 100)
      {
        Log($"{passed} passed, {failed} failed (should be ~50/50)");
        Completed();
      }
    };

    var service = new CommandService(_registry);
    for (var i = 0; i < 100; i++)
    {
      var promise = service.Execute(new SpamCommand());
      Assert(promise != null);
      promise.Then(() =>
      {
        passed += 1;
        maybeResolved();
      }, (err) =>
      {
        failed += 1;
        maybeResolved();
      });
    }
  }
  
  [RuntimeTest]
  public void TestForgottenPromiseIsRejected()
  {
    var service = new CommandService(_registry);
    service.SetCommandTimeout(1f);
    service.Execute(new ForgottenCommand()).Then(Unreachable, (err) =>
    {
      // Ok! That was a forgotten command.
      Completed();
    });
  }
}