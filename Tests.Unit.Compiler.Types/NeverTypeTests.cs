using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Xunit;

namespace Azoth.Tools.Bootstrap.Tests.Unit.Compiler.Types;

[Trait("Category", "Types")]
public class NeverTypeTests
{
    [Fact]
    public void Is_empty_data_type()
    {
        var type = NeverType.Instance;

        Assert.OfType<EmptyType>(type);
    }

    [Fact]
    public void Is_known_type()
    {
        var type = NeverType.Instance;

        Assert.True(type.IsFullyKnown);
    }

    [Fact]
    public void Has_special_name_never()
    {
        var type = NeverType.Instance;

        Assert.Equal(SpecialTypeName.Never, type.Name);
    }

    [Fact]
    public void Has_proper_ToSourceCodeString()
    {
        var type = NeverType.Instance;

        Assert.Equal("never", type.ToSourceCodeString());
    }

    [Fact]
    public void Convert_to_read_only_has_no_effect()
    {
        var type = NeverType.Instance;

        var @readonly = type.WithoutWrite();

        Assert.Equal(type, @readonly);
    }

    [Fact]
    public void Is_equal_to_itself()
    {
        Assert.Equal(NeverType.Instance, NeverType.Instance);
    }
}
