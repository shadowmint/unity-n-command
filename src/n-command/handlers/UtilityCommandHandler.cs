using System;
using System.Linq;
using N.Package.Bind.Core;
using N.Package.Events;

namespace N.Package.Command
{
  /// Utility handler for handling common aggregate command types.
  public class UtilityCommandHandler : ICommandHandler<ExecuteParallelCommand>, ICommandHandler<ExecuteSequenceCommand>,
    ICommandHandler<ExecuteDeferredCommand>, ICommandHandler<DelegateCommand>
  {
    public IServiceRegistry ServiceRegistry { get; set; }

    public void Execute(ExecuteParallelCommand command)
    {
      Action<CommandExecutedEvent> onCommandCompleted = (ep) =>
      {
        if (ep.Success)
        {
          command.Resolved += 1;
        }
        else
        {
          command.Failures += 1;
          if (ep.Failure != null)
          {
            command.Exceptions.Add(ep.Failure);
          }
        }

        if (command.Resolved + command.Failures < command.Commands.Count) return;
        if (command.Failures > 0)
        {
          command.Failed();
        }
        else
        {
          command.Completed();
        }
      };

      command.Commands.ForEach(cmd => cmd.Execute(ServiceRegistry, onCommandCompleted));
    }

    public void Execute(ExecuteSequenceCommand command)
    {
      Action<CommandExecutedEvent> executeNextCommand = null;
      executeNextCommand = (ep) =>
      {
        if (ep != null)
        {
          if (ep.Success)
          {
            command.Resolved += 1;
          }
          else
          {
            command.Failures += 1;
            if (ep.Failure != null)
            {
              command.Exceptions.Add(ep.Failure);
            }
          }
        }

        var next = command.Commands.FirstOrDefault();
        if (next == null)
        {
          if (command.Failures > 0)
          {
            command.Failed();
          }
          else
          {
            command.Completed();
          }
        }
        else
        {
          command.Commands.RemoveAt(0);
          next.Execute(ServiceRegistry, executeNextCommand);
        }
      };

      executeNextCommand(null);
    }

    public void Execute(ExecuteDeferredCommand command)
    {
      command.EventHandler.AddEventHandler<ExecuteDeferredCommandEvent>((ep) =>
      {
        if (ep.Command == null) return;
        ep.Command.Command.OnCompleted((inner) =>
        {
          if (inner.Success)
          {
            ep.Command.Completed();
          }
          else
          {
            ep.Command.Failed(inner.Failure);
          }
        });
        ep.Command.Command.Execute(ServiceRegistry);
      });
      command.EventHandler.TriggerDeferred(new ExecuteDeferredCommandEvent() {Command = command}, command.Delay);
    }

    public void Execute(DelegateCommand command)
    {
      try
      {
        command.Command();
      }
      catch (Exception err)
      {
        command.Failed(err);
        return;
      }
      command.Completed();
    }
  }
}