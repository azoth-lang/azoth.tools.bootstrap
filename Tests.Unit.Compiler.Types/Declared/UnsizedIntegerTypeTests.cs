using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Xunit;

namespace Azoth.Tools.Bootstrap.Tests.Unit.Compiler.Types.Declared;

[Trait("Category", "Types")]
public class UnsizedIntegerTypeTests
{
    [Fact]
    public void Size_is_unsigned()
    {
        var type = PointerSizedIntegerType.Size;

        Assert.False(type.IsSigned);
    }

    [Fact]
    public void Offset_is_signed()
    {
        var type = PointerSizedIntegerType.Offset;

        Assert.True(type.IsSigned);
    }

    [Fact]
    public void Types_equal_to_themselves_and_not_others()
    {
        Assert.Equal(PointerSizedIntegerType.Size, PointerSizedIntegerType.Size);
        Assert.Equal(PointerSizedIntegerType.Offset, PointerSizedIntegerType.Offset);

        Assert.NotEqual(PointerSizedIntegerType.Size, PointerSizedIntegerType.Offset);
    }
}
