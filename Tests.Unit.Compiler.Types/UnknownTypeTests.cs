using Azoth.Tools.Bootstrap.Compiler.Types;
using Xunit;

namespace Azoth.Tools.Bootstrap.Tests.Unit.Compiler.Types;

[Trait("Category", "Types")]
public class UnknownTypeTests
{
    [Fact]
    public void Equal_to_itself()
    {
        Assert.Equal(UnknownType.Instance, UnknownType.Instance);
    }
}
