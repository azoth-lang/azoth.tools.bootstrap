using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy;
using Xunit;

namespace Azoth.Tools.Bootstrap.Tests.Unit.Compiler.Types.Legacy;

[Trait("Category", "Types")]
public class VoidTypeTests
{
    [Fact]
    public void Is_empty_data_type()
    {
        var type = VoidType.Instance;

        Assert.OfType<EmptyType>(type);
    }

    [Fact]
    public void Has_special_name_never()
    {
        var type = VoidType.Instance;

        Assert.Equal(SpecialTypeName.Void, type.Name);
    }

    [Fact]
    public void Has_proper_ToSourceCodeString()
    {
        var type = VoidType.Instance;

        Assert.Equal("void", type.ToSourceCodeString());
    }

    [Fact]
    public void Convert_to_read_only_has_no_effect()
    {
        var type = NeverType.Instance;

        var @readonly = type.WithoutWrite();

        Assert.Equal(type, @readonly);
    }


    [Fact]
    public void Equal_to_itself()
    {
        Assert.Equal(NeverType.Instance, NeverType.Instance);
    }
}
