using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using N.Package.Promises;
using N.Package.Promises.Infrastructure;
using UnityEngine;

namespace N.Package.Command.Infrastructure
{
  public class CommandWorker : MonoBehaviour
  {
    /// <summary>
    /// Warn if the total number of pending commands hits this limit.
    /// You can override this value using CommandService.
    /// </summary>
    public int DeferredCommandThreshold { get; set; } = 1024;

    /// <summary>
    /// If a command is older than this, just dispose of it, it's probably broken.
    /// You can override this value using CommandService.
    /// </summary>
    public float ExpiredCommandThreshold { get; set; } = 10f;

    private static CommandWorker _instance;

    private readonly Queue<CommandQueueItem> _items = new Queue<CommandQueueItem>();

    public static CommandWorker Get()
    {
      if (_instance != null) return _instance;
      var container = new GameObject();
      container.transform.name = typeof(CommandWorker).FullName ?? "CommandWorker";

      try
      {
        DontDestroyOnLoad(container);
      }
      catch (Exception)
      {
        // Editor
      }

      container.hideFlags = HideFlags.HideInHierarchy;
      _instance = container.AddComponent<CommandWorker>();
      return _instance;
    }

    public void Register(Deferred task, IPromise promise, IPromise outerPromise, ICommand command)
    {
      _items.Enqueue(new CommandQueueItem(task.Reject, promise, outerPromise, command));
      if (_items.Count > DeferredCommandThreshold)
      {
        Debug.LogWarning($"Warning! There are {_items.Count} unresolved commands pending!");
      }
    }

    public void Register<TRtn>(Deferred<TRtn> task, IPromise promise, IPromise outerPromise, ICommand command)
    {
      _items.Enqueue(new CommandQueueItem(task.Reject, promise, outerPromise, command));
      if (_items.Count > DeferredCommandThreshold)
      {
        Debug.LogWarning($"Warning! There are {_items.Count} unresolved commands pending!");
      }
    }

    public void Update()
    {
      var count = _items.Count;
      for (var i = 0; i < count; i++)
      {
        var item = _items.Dequeue();
        if (item.Promise.Check())
        {
          item.OuterPromise.Check();
          continue;
        }

        item.Elapsed += Time.deltaTime;
        if (item.Elapsed > ExpiredCommandThreshold)
        {
          Debug.Log("Rejecting old promise");
          item.Reject(new CommandExecutionError(item.Command, $"{ExpiredCommandThreshold} elapsed without command resolving. Automatically rejecting it."));
          item.OuterPromise.Check();
          continue;
        }

        _items.Enqueue(item);
      }
    }

    private struct CommandQueueItem
    {
      public CommandQueueItem(Action<Exception> onReject, IPromise innerPromise, IPromise outerPromise, ICommand command)
      {
        Reject = onReject;
        Promise = innerPromise;
        OuterPromise = outerPromise;
        Elapsed = 0f;
        Command = command;
      }

      public Action<Exception> Reject { get; }
      public IPromise Promise { get; }
      public IPromise OuterPromise { get; }
      public ICommand Command { get; }
      public float Elapsed { get; set; }
    }
  }
}