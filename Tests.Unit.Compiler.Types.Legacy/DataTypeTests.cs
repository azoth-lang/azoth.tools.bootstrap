using Azoth.Tools.Bootstrap.Compiler.Types.Legacy;
using Xunit;

namespace Azoth.Tools.Bootstrap.Tests.Unit.Compiler.Types.Legacy;

[Trait("Category", "Types")]
public class DataTypeTests
{
    [Fact]
    public void None_type_is_optional_never()
    {
        var none = IType.None;

        Assert.OfType<OptionalType>(none);
        Assert.Equal(NeverType.Instance, none.Referent);
    }
}
