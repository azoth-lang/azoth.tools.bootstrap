using Azoth.Tools.Bootstrap.Compiler.Types;
using Xunit;

namespace Azoth.Tools.Bootstrap.Tests.Unit.Compiler.Types;

[Trait("Category", "Types")]
public class UnknownTypeTests
{
    [Fact]
    public void Unknown_is_NOT_a_known_type()
    {
        var type = UnknownType.Instance;

        Assert.False(type.IsFullyKnown);
    }

    [Fact]
    public void Is_not_constant()
    {
        var type = UnknownType.Instance;

        Assert.False(type.IsTypeOfConstValue);
    }

    [Fact]
    public void Equal_to_itself()
    {
        Assert.Equal(UnknownType.Instance, UnknownType.Instance);
    }
}
