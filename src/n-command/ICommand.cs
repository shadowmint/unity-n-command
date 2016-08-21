using N.Package.Events;

namespace N.Package.Command
{
  /// ICommand is the base interface for executable command types.
  public interface ICommand
  {
    /// The event handler for this command
    EventHandler EventHandler { get; }

    /// Can this command be executed?
    /// Override this with command param validation.
    bool CanExecute();

    /// Execute this command
    void Execute();
  }
}