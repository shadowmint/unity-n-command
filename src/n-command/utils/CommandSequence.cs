using System.Collections.Generic;
using N.Package.Events;

namespace N.Package.Command
{
  /// Run actions one after each other
  public class CommandSequence : ICommand
  {
    /// Set of deferred child actions
    private readonly Queue<ICommand> _children = new Queue<ICommand>();

    private ICommand _currentAction;

    private readonly EventHandler _eventHandler = new EventHandler();

    /// Execute the next action
    public void NextAction(CommandExecutedEvent item)
    {
      if (_children.Count > 0)
      {
        _currentAction = _children.Dequeue();
        _currentAction.OnCompleted(NextAction);
        _currentAction.Execute();
      }
      else
      {
        this.Completed();
      }
    }

    /// Add another action to this set
    public CommandSequence Add(ICommand action)
    {
      _children.Enqueue(action);
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