using System;

namespace N.Package.Command
{
  /// This event is raised when a command has completed its execution.
  public class CommandExecutedEvent
  {
    /// The command that finished
    public ICommand Command { get; set; }

    /// If the command was successful
    public bool Success { get; set; }

    /// If an error was failed during execution this is the error
    public Exception Failure { get; set; }
  }
}