using System.Collections.Generic;
using N.Package.Command;

public class DeferredCommand : ICommand<string>
{
  public string Value { get; set; }
}

public class SpamCommand : ICommand
{
}