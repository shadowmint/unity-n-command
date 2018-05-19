using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N.Package.Command
{
  public class CommandExecutionError : Exception
  {
    public ICommand Command { get; private set; }

    public CommandExecutionError(ICommand command, string msg) : base(msg)
    {
      Command = command;
    }

    public CommandExecutionError(ICommand command, string msg, Exception innerException) : base(msg, innerException)
    {
      Command = command;
    }
  }
}