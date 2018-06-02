using System;
using System.Collections;
using System.Threading.Tasks;
using N.Package.Command;
using N.Package.Promises;
using UnityEngine;
using Random = UnityEngine.Random;

public class DeferredCommandHandler : ICommandHandler<DeferredCommand, string>, ICommandHandler<SpamCommand>, ICommandHandler<ForgottenCommand>
{
  public Task<string> Execute(DeferredCommand command)
  {
    var deferred = new Deferred<string>();
    AsyncWorker.Run(() => WaitThenResolve(command, deferred));
    return deferred.Task;
  }

  private IEnumerator WaitThenResolve(DeferredCommand command, Deferred<string> deferred)
  {
    yield return new WaitForSeconds(1f);
    deferred.Resolve(command.Value + " RESOLVED");
  }

  public Task Execute(SpamCommand command)
  {
    var deferred = new Deferred();
    AsyncWorker.Run(() => WaitFrameThenResolve(command, deferred));
    return deferred.Task;
  }

  private IEnumerator WaitFrameThenResolve(SpamCommand command, Deferred deferred)
  {
    yield return new WaitForEndOfFrame();

    while (Random.value > 0.5f)
    {
      yield return new WaitForEndOfFrame();
    }

    if (Random.value > 0.5f)
    {
      deferred.Resolve();
    }
    else
    {
      deferred.Reject(new Exception("Nope"));
    }
  }

  public Task Execute(ForgottenCommand command)
  {
    // Woops, forgot to resolve this one...
    var deferred = new Deferred();
    return deferred.Task;
  }
}
