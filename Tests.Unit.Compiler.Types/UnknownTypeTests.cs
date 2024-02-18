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

    /// <summary>
    /// It has never semantics because it is assignable to anything
    /// </summary>
    [Fact]
    public void Unknown_has_never_semantics()
    {
        var type = UnknownType.Instance;

        Assert.Equal(TypeSemantics.Never, type.Semantics);
    }

    [Fact]
    public void Equal_to_itself()
    {
        Assert.Equal(UnknownType.Instance, UnknownType.Instance);
    }
}
