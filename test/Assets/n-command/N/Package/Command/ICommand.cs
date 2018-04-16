using N.Package.Events;

namespace N.Package.Command
{
  /// <summary>
  /// The base interface for any command type.
  /// </summary>
  public interface ICommand
  {
  }

  /// <summary>
  /// The base interface for any command that returns a value.
  /// </summary>
  public interface ICommand<TResult> : ICommand
  {
  }
}