using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Declared;
using Xunit;

namespace Azoth.Tools.Bootstrap.Tests.Unit.Compiler.Types.Legacy.Declared;

[Trait("Category", "Types")]
public class BoolTypeTests
{
    [Fact]
    public void Has_special_name_bool()
    {
        var type = BoolType.Instance;

        Assert.Equal(SpecialTypeName.Bool, type.Name);
    }

    [Fact]
    public void Has_proper_ToString()
    {
        var type = BoolType.Instance;

        Assert.Equal("bool", type.ToString());
    }

    [Fact]
    public void Bool_type_equal_to_itself()
    {
        Assert.Equal(BoolType.Instance, BoolType.Instance);
    }
}
