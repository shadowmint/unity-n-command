#if N_COMMAND_TESTS

using System.Threading.Tasks;
using N.Package.Bind;
using N.Package.Test;
using N.Package.Promises;
using NUnit.Framework;

namespace N.Package.Command.Editor
{
  public class AutoResolveCommandTests : TestCase
  {
    [Test]
    public void TestResolveHandlers()
    {
      var resolver = new ServiceRegistry();
      resolver.Register<TestCommandAsyncHandler>();
      resolver.Register<TestCommandSyncHandler>();
      resolver.Register<TestCommandSyncWithValueHandler>();

      this.RunAsyncTest(async () =>
      {
        await new TestCommandSync().ExecuteAsync(resolver);
        await new TestCommandAsync().ExecuteAsync(resolver);
        var result = await new TestCommandSyncWithValue().ExecuteAsync<TestCommandSyncWithValue, string>(resolver);
        Assert(result == "Hello World");
      });
    }
  }
}

#endif