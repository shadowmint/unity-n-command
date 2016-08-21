using System;
using N.Package.Command;

namespace N.Package.Command
{
  public static class CommandExtensions
  {
    /// Bind a one-time event handler for command completion.
    public static void OnCompleted(this ICommand command, Action<CommandExecutedEvent> onCompleteHandler)
    {
      Action<CommandExecutedEvent> wrapper = null;
      wrapper = (ep) =>
      {
        command.EventHandler.RemoveEventHandler(wrapper);
        onCompleteHandler(ep);
      };
      command.EventHandler.AddEventHandler(wrapper);
    }

    /// Trigger a CommandExecutedEvent on the command.
    public static void Completed(this ICommand command)
    {
      command.EventHandler.Trigger(new CommandExecutedEvent()
      {
        Command = command,
        Success = true
      });
    }

    /// Trigger a CommandExecutedEvent on the command.
    public static void Failed(this ICommand command, Exception failure = null)
    {
      command.EventHandler.Trigger(new CommandExecutedEvent()
      {
        Command = command,
        Success = false,
        Failure = failure
      });
    }
  }
}