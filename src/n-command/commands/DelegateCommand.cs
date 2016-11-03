using System;
using System.Collections.Generic;
using EventHandler = N.Package.Events.EventHandler;

namespace N.Package.Command
{
  /// ...for when you're too lazy to create a real command handler, eg. in tests.
  public class DelegateCommand : ICommand
  {
    private EventHandler _eventHandler = new EventHandler();

    public EventHandler EventHandler
    {
      get { return _eventHandler; }
      set { _eventHandler = value; }
    }

    public Action Command { get; set; }

    public DelegateCommand(Action command)
    {
      Command = command;
    }
  }
}