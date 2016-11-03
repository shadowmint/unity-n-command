using System;
using System.Collections.Generic;
using EventHandler = N.Package.Events.EventHandler;

namespace N.Package.Command
{
  public class ExecuteDeferredCommand : ICommand
  {
    private EventHandler _eventHandler = new EventHandler();

    public EventHandler EventHandler
    {
      get { return _eventHandler; }
      set { _eventHandler = value; }
    }

    public ICommand Command { get; set; }
    public float Delay { get; set; }
  }

  public class ExecuteDeferredCommandEvent
  {
    public ExecuteDeferredCommand Command { get; set; }
  }
}