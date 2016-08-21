# n-command

Helper module for executing high level potentially async actions in unity.

Also supports deferred and chain actions.

## Usage

See the tests in the `Editor/` folder for each class for usage examples.

    public class TestCommandSync : ICommand
    {
      private readonly EventHandler _eventHandler = new EventHandler();
      public EventHandler EventHandler
      {
        get { return _eventHandler; }
      }

      public bool CanExecute()
      {
        return true;
      }

      public void Execute()
      {
        this.Completed();
      }
    }

    ...

    var command = new TestCommandSync();
    command.OnCompleted((ep) => { ... });
    command.Execute();

Notice that commands choose when they resolve themselves; the Execute()
method may defer calling `Completed()` or `Failed()` until an async action
such as user input has occurred.

## Install

From your unity project folder:

    npm init
    npm install shadowmint/unity-n-command --save
    echo Assets/packages >> .gitignore
    echo Assets/packages.meta >> .gitignore

The package and all its dependencies will be installed in
your Assets/packages folder.

## Development

Setup and run tests:

    npm install
    npm install ..
    cd test
    npm install
    gulp

Remember that changes made to the test folder are not saved to the package
unless they are copied back into the source folder.

To reinstall the files from the src folder, run `npm install ..` again.

### Tests

All tests are wrapped in `#if ...` blocks to prevent test spam.

You can enable tests in: Player settings > Other Settings > Scripting Define Symbols

The test key for this package is: N_COMMAND_TESTS
