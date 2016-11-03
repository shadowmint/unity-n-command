using N.Package.Events;

namespace N.Package.Command
{
  /// ICommand is the base interface for executable command types.
  public interface ICommand
  {
    /// The event handler for this command
    EventHandler EventHandler { get; }
  }
}