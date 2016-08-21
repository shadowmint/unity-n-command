using System.Collections.Generic;
using N.Package.Events;

namespace N.Package.Command
{
  /// Run actions all at once and report when they're done.
  public class CommandGroup : ICommand
  {
    /// Set of deferred child actions
    private readonly List<ICommand> _children = new List<ICommand>();

    /// Set of result states for commands
    private List<CommandExecutedEvent> _results = new List<CommandExecutedEvent>();

    private int _resolved;

    private int _success;

    private readonly EventHandler _eventHandler = new EventHandler();

    /// Add another action to this set
    public CommandGroup Add(ICommand action)
    {
      _children.Add(action);
      return this;
    }

    public EventHandler EventHandler
    {
      get { return _eventHandler; }
    }

    /// Set of result states for commands
    public List<CommandExecutedEvent> Results
    {
      get { return _results; }
    }

    public bool CanExecute()
    {
      return _children.Count > 0;
    }

    /// Run this action
    public void Execute()
    {
      _children.ForEach((i) =>
      {
        i.OnCompleted(MaybeCompleted);
        i.Execute();
      });
    }

    /// Maybe resolve this command if no others left?
    private void MaybeCompleted(CommandExecutedEvent ep)
    {
      _resolved += 1;
      _results.Add(ep);
      _success += ep.Success ? 1 : 0;

      if (_resolved != _children.Count) return;

      if (_success == _children.Count)
      {
        this.Completed();
      }
      else
      {
        this.Failed();
      }
    }
  }
}