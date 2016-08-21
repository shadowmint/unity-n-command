using System;
using N.Package.Events;
using EventHandler = N.Package.Events.EventHandler;

namespace N.Package.Command
{
  /// A basic command using a delegate.
  public class DelegateCommand : ICommand
  {
    private readonly EventHandler _eventHandler = new EventHandler();
    private readonly Action _command;
    private readonly bool _async;
    private readonly float _delay;

    public DelegateCommand(Action action)
    {
      _command = action;
    }

    public DelegateCommand(Action action, float delay)
    {
      _command = action;
      _delay = delay;
      _async = true;
    }

    public EventHandler EventHandler
    {
      get { return _eventHandler; }
    }

    public bool CanExecute()
    {
      return _command != null;
    }

    /// Run this action
    public void Execute()
    {
      if (_async)
      {
        Action<DelegateCommand> invoker = null;
        invoker = (ep) =>
        {
          _eventHandler.RemoveEventHandler(invoker);
          ExecuteNow();
        };
        _eventHandler.AddEventHandler<DelegateCommand>(invoker);
        _eventHandler.TriggerDeferred(this, _delay);
      }
      else
      {
        ExecuteNow();
      }
    }

    /// Actually invoke~
    private void ExecuteNow()
    {
      _command();
      this.Completed();
    }
  }
}