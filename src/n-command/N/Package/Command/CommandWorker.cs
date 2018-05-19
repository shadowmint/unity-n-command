using System;
using System.Collections;
using System.Threading.Tasks;
using N.Packages.Promises;
using UnityEngine;

namespace DefaultNamespace
{
  /// <summary>
  /// CommandWorker creates a singleton GameObject to run worker instances on using
  /// Default, or you can manually create an instance.
  /// </summary>
  public class CommandWorker : MonoBehaviour
  {
    private const string DefaultCommandWorkerName = "DefaultCommandWorker";

    [Tooltip("Set this true to abort this worker, whatever it is")]
    public bool Abort;

    private bool _running;

    private Func<IEnumerator> Task { get; set; }

    private Deferred Deferred { get; set; }

    private static GameObject _default;

    private static GameObject Default
    {
      get
      {
        try
        {
          if (_default.transform.name == DefaultCommandWorkerName)
          {
            return _default;
          }
        }
        catch (Exception error)
        {
          // Invalid object, etc.
        }

        var instance = new GameObject();
        instance.hideFlags = HideFlags.HideInHierarchy;
        instance.transform.name = DefaultCommandWorkerName;
        _default = instance;
        return _default;
      }
    }

    public static Promise Run(Func<IEnumerator> task, string name = "AsyncWorker")
    {
      var worker = Default.AddComponent<CommandWorker>();
      worker.Task = task;
      worker.Deferred = new Deferred();
      return new Promise(worker.Deferred.Task);
    }

    public static Task RunAsync(Func<IEnumerator> task, string name = "AsyncWorker")
    {
      var worker = Default.AddComponent<CommandWorker>();
      worker.Task = task;
      worker.Deferred = new Deferred();
      worker.Deferred.Task.Dispatch();
      return worker.Deferred.Task;
    }

    public void Start()
    {
      StartCoroutine(CoroutineWrapper());
    }

    private IEnumerator CoroutineWrapper()
    {
      var task = Task?.Invoke();
      if (task == null) yield break;

      _running = true;

      while (true)
      {
        object current;
        try
        {
          if (task.MoveNext() == false)
          {
            break;
          }

          current = task.Current;
        }
        catch (Exception error)
        {
          Deferred.Reject(error);
          break;
        }

        yield return current;
      }

      Deferred.Resolve();
      _running = false;
    }

    public void Update()
    {
      if (!_running || Abort)
      {
        Destroy(this);
      }
    }
  }
}