using Azoth.Tools.Bootstrap.Compiler.Types;
using Xunit;

namespace Azoth.Tools.Bootstrap.Tests.Unit.Compiler.Types;

[Trait("Category", "Types")]
public class SizedIntegerTypeTests
{
    [Fact]
    public void Byte_has_8_bits()
    {
        var type = FixedSizeIntegerType.Byte;

        Assert.Equal(8, type.Bits);
    }

    [Fact]
    public void Byte_is_unsigned()
    {
        var type = FixedSizeIntegerType.Byte;

        Assert.False(type.IsSigned);
    }

    [Fact]
    public void Int_has_copy_semantics()
    {
        var type = FixedSizeIntegerType.Int32;

        Assert.Equal(TypeSemantics.Copy, type.Semantics);
    }

    [Fact]
    public void Types_equal_to_themselves_and_not_others()
    {
        Assert.Equal(FixedSizeIntegerType.Int32, FixedSizeIntegerType.Int32);
        Assert.Equal(FixedSizeIntegerType.UInt32, FixedSizeIntegerType.UInt32);
        Assert.Equal(FixedSizeIntegerType.Byte, FixedSizeIntegerType.Byte);

        Assert.NotEqual(FixedSizeIntegerType.Int32, FixedSizeIntegerType.UInt32);
        Assert.NotEqual(FixedSizeIntegerType.Int32, FixedSizeIntegerType.Byte);
    }
}
