using System;
using System.Reflection;
using System.Xml;
using N.Package.Bind.Core;
using N.Package.Command;

namespace N.Package.Command
{
  public static class CommandExtensions
  {
    // Constants
    private const string ResolveMethod = "Resolve";
    private const string ExecuteMethod = "Execute";

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

    /// Given a command and a registry, resolve and execute the command.
    public static void Execute(this ICommand command, IServiceRegistry resolver)
    {
      var handlerType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());
      var resolveFunc = resolver.GetType().GetMethod(ResolveMethod).MakeGenericMethod(handlerType);
      var handler = resolveFunc.Invoke(resolver, new object[] {});
      if (handler == null) return;
      var invokeMethod = handler.GetType().GetMethod(ExecuteMethod, new[] {command.GetType()});
      try
      {
        invokeMethod.Invoke(handler, new object[] {command});
      }
      catch (TargetInvocationException ex)
      {
        ThrowInnerException(ex);
      }
    }

    /// Given a command, a registry and a callback resolve and execute the command.
    public static void Execute(this ICommand command, IServiceRegistry resolver, Action<CommandExecutedEvent> callback)
    {
      command.OnCompleted(callback);
      command.Execute(resolver);
    }

    private static void ThrowInnerException(TargetInvocationException ex)
    {
      if (ex.InnerException == null)
      {
        throw new NullReferenceException("TargetInvocationException did not contain an InnerException", ex);
      }

      Exception exception = null;
      try
      {
        exception =
          (Exception)
          Activator.CreateInstance(ex.InnerException.GetType(), ex.InnerException.Message, ex.InnerException);
      }
      catch
      {
        // ignored
      }

      if (exception == null || exception.InnerException == null || ex.InnerException.Message != exception.Message)
      {
        throw ex.InnerException;
      }
      throw exception;
    }
  }
}