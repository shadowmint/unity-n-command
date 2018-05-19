using System.Threading.Tasks;

namespace N.Package.Command
{
  /// Command handlers execute commands.
  /// Command handlers should be stateless.
  public interface ICommandHandler<in T> where T : ICommand
  {
    /// Execute the given command instance
    Task Execute(T command);
  }

  /// Command handlers execute commands.
  /// Command handlers should be stateless.
  public interface ICommandHandler<in T, TResult> where T : ICommand<TResult>
  {
    /// Execute the given command instance
    Task<TResult> Execute(T command);
  }
}