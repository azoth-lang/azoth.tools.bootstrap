using Azoth.Tools.Bootstrap.Framework;
using Xunit;

namespace Azoth.Tools.Bootstrap.Tests.Unit.Framework;

public class BoolExtensionsTests
{
    [Fact]
    public void ImpliesWithBoolDoesNotShortCircuit()
    {
        var actual = False(out var leftCalled).Implies(True(out var rightCalled));

        Assert.True(actual);
        Assert.True(leftCalled);
        Assert.True(rightCalled);
    }

    [Fact]
    public void ImpliesWithFuncDoesShortCircuit()
    {
        bool rightCalled = false;
        var actual = False(out var leftCalled).Implies(() => True(out rightCalled));

        Assert.True(actual);
        Assert.True(leftCalled);
        Assert.False(rightCalled);
    }

    private bool False(out bool called)
    {
        called = true;
        return false;
    }

    private bool True(out bool called)
    {
        called = true;
        return true;
    }
}
