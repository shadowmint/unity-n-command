using System.Collections.Generic;
using System.Threading.Tasks;
using N.Package.Bind;
using N.Package.Command;
using N.Package.Test.Runtime;
using N.Packages.Promises;
using UnityEditor.PackageManager;

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
    for (var i = 0; i < 100; i++)
    {
      tasks.Add(runner.Execute(new SpamCommand()));
    }

    Task.WhenAll(tasks).Promise().Then(() =>
    {
      tasks.ForEach((t) =>
      {
        if (t.IsCompleted)
        {
          passed += 1;
        }
        else
        {
          failed += 1;
        }

        Log($"Passed: {passed}, Failed: {failed} (should be ~50/50)");
        Completed();
      });
    }, Failed).Dispatch();
  }
}