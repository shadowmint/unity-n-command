using N.Package.Bind;
using N.Package.Bind.Core;
using N.Package.Command;
using N.Package.Command.Infrastructure;
using N.Package.Promises;
using UnityEngine;

namespace packages.N.Package.Command
{
  public class CommandService
  {
    private readonly IServiceRegistry _registry;

    public CommandService(IServiceRegistry registry)
    {
      _registry = registry;
    }

    public Promise Execute<T>(T command) where T : ICommand
    {
      var deferred = new Deferred();
      var task = command.ExecuteAsync(_registry);
      var promise = task.Promise();
      promise.Then(() => deferred.Resolve(), err => deferred.Reject(err));
      var outerPromise = deferred.Task.Promise();
      CommandWorker.Get().Register(deferred, promise, outerPromise, command);
      return outerPromise;
    }

    public Promise<TRtn> Execute<T, TRtn>(T command) where T : ICommand<TRtn>
    {
      var deferred = new Deferred<TRtn>();
      var task = command.ExecuteAsync<T, TRtn>(_registry);
      var promise = task.Promise();
      promise.Then((rtn) => deferred.Resolve(rtn), err => deferred.Reject(err));
      var outerPromise = deferred.Task.Promise();
      CommandWorker.Get().Register(deferred, promise, outerPromise, command);
      return outerPromise;
    }

    /// <summary>
    /// Reject orphan commands after this long, even if they are still running.
    /// </summary>
    public void SetCommandTimeout(float timeout)
    {
      CommandWorker.Get().ExpiredCommandThreshold = timeout;
    }

    /// <summary>
    /// Start emitting warnings if the set of unresolved commands get bigger than this.
    /// </summary>
    public void SetCommandQueueWarnThreshold(int threshold)
    {
      CommandWorker.Get().DeferredCommandThreshold = threshold;
    }
  }
}