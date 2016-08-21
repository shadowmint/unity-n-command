using N.Package.Events;

namespace N.Package.Command
{
  /// Command handlers execute commands.
  /// Command handlers should be stateless.
  public interface ICommandHandler<in T> where T : ICommand
  {
    /// Execute the given command instance
    void Execute(T command);
  }
}