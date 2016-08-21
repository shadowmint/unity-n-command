using System.Collections.Generic;
using N.Package.Events;

namespace N.Package.Command
{
  internal struct RequestedCommand
  {
    public float Delay;
    public ICommand Command;
  }

  /// Run actions one after each other
  public class TimedCommandSequence : ICommand
  {
    /// Set of deferred child actions
    private readonly Queue<RequestedCommand> _children = new Queue<RequestedCommand>();

    private ICommand _currentAction;

    private readonly EventHandler _eventHandler = new EventHandler();

    public TimedCommandSequence()
    {
      _eventHandler.AddEventHandler<TimedCommandSequence>((ep) =>
      {
        if (_currentAction != null)
        {
          _currentAction.Execute();
        }
      });
    }

    /// Execute the next action
    public void NextAction(CommandExecutedEvent item)
    {
      if (_children.Count > 0)
      {
        var next = _children.Dequeue();
        _currentAction = next.Command;
        _currentAction.OnCompleted(NextAction);
        _eventHandler.TriggerDeferred(this, next.Delay);
      }
      else
      {
        this.Completed();
      }
    }

    /// Add another action to this set
    public TimedCommandSequence Add(ICommand action, float delay)
    {
      _children.Enqueue(new RequestedCommand()
      {
        Command = action,
        Delay = delay
      });
      return this;
    }

    public EventHandler EventHandler
    {
      get { return _eventHandler; }
    }

    public bool CanExecute()
    {
      return _children.Count > 0;
    }

    /// Run this action
    public void Execute()
    {
      NextAction(null);
    }
  }
}