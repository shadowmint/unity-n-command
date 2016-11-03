using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N.Package.Command
{
  public class CommandExecutionError : Exception
  {
    public ICommand Command { get; set; }

    public CommandExecutionError(string msg) : base(msg)
    {
    }
  }
}