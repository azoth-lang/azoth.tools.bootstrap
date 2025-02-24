using Azoth.Tools.Bootstrap.Framework;
using Xunit;

namespace Azoth.Tools.Bootstrap.Tests.Unit.Framework;

public class BoolExtensionsTests
{
    [Fact]
    public void InlinedImpliesWithBoolDoesShortCircuit()
    {
        var actual = False(out var leftCalled).Implies(True(out var rightCalled));

        Assert.True(actual);
        Assert.True(leftCalled);
        Assert.False(rightCalled);
    }

    private static bool False(out bool called)
    {
        called = true;
        return false;
    }

    private static bool True(out bool called)
    {
        called = true;
        return true;
    }
}
