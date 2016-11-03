using System;
using System.Collections.Generic;
using EventHandler = N.Package.Events.EventHandler;

namespace N.Package.Command
{
  public class ExecuteSequenceCommand : ICommand
  {
    private EventHandler _eventHandler = new EventHandler();

    public EventHandler EventHandler
    {
      get { return _eventHandler; }
      set { _eventHandler = value; }
    }

    // Args
    public List<ICommand> Commands { get; set; }

    // Results
    public int Resolved { get; set; }
    public int Failures { get; set; }
    public List<Exception> Exceptions { get; set; }

    public ExecuteSequenceCommand Add(ICommand command)
    {
      if (Commands == null)
      {
        Commands = new List<ICommand>();
      }

      Commands.Add(command);
      return this;
    }
  }
}