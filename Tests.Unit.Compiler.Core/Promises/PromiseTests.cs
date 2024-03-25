using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Xunit;

namespace Azoth.Tools.Bootstrap.Tests.Unit.Compiler.Core.Promises;

public class PromiseTests
{
    [Fact]
    public void Promise_Null_IsFulfilled()
    {
        var promise = Promise.Null<object>();
        Assert.True(promise.IsFulfilled);
    }
}
