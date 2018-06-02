using System;
using System.Threading.Tasks;
using N.Package.Bind;
using N.Package.Bind.Core;
using N.Package.Promises;

namespace N.Package.Command
{
  public static class CommandExtensions
  {
    public static async Task ExecuteAsync<T>(this T command, IServiceRegistry registry) where T : ICommand
    {
      try
      {
        registry = registry ?? Registry.Default;
        var handler = registry.Resolve<ICommandHandler<T>>();
        await handler.Execute(command);
      }
      catch (Exception error)
      {
        throw new CommandExecutionError(command, "Failed to execute task", error);
      }
    }

    public static async Task<TResult> ExecuteAsync<T, TResult>(this T command, IServiceRegistry registry) where T : ICommand<TResult>
    {
      try
      {
        registry = registry ?? Registry.Default;
        var handler = registry.Resolve<ICommandHandler<T, TResult>>();
        return await handler.Execute(command);
      }
      catch (Exception error)
      {
        throw new CommandExecutionError(command, "Failed to execute task", error);
      }
    }

    public static Task Reject<T>(this T commandHandler, CommandExecutionError error) where T : ICommand
    {
      var deferred = new Deferred();
      deferred.Reject(error);
      return deferred.Task;
    }

    public static Task Reject<T, TResult>(this T commandHandler, CommandExecutionError error) where T : ICommand<TResult>
    {
      var deferred = new Deferred();
      deferred.Reject(error);
      return deferred.Task;
    }

    public static Task Resolve<T>(this ICommandHandler<T> commandHandler) where T : ICommand
    {
      var deferred = new Deferred();
      deferred.Resolve(true);
      return deferred.Task;
    }

    public static Task<TResult> Resolve<T, TResult>(this ICommandHandler<T, TResult> commandHandler, TResult result) where T : ICommand<TResult>
    {
      var deferred = new Deferred<TResult>();
      deferred.Resolve(result);
      return deferred.Task;
    }
  }
}