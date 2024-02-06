using Azoth.Tools.Bootstrap.Compiler.Types;
using Xunit;
using static Azoth.Tools.Bootstrap.Compiler.Types.Capabilities.ReferenceCapability;

namespace Azoth.Tools.Bootstrap.Tests.Unit.Compiler.Types;

[Trait("Category", "Types")]
public class AnyTypeTests
{
    [Fact]
    public void Is_reference_type()
    {
        var type = new AnyType(Isolated);

        Assert.OfType<ReferenceType>(type);
    }

    [Fact]
    public void Is_not_constant()
    {
        var type = new AnyType(Isolated);

        Assert.False(type.IsTypeOfValue);
    }

    [Fact]
    public void Is_known_type()
    {
        var type = new AnyType(Isolated);

        Assert.True(type.IsFullyKnown);
    }

    [Fact]
    public void Is_not_empty_type()
    {
        var type = new AnyType(Isolated);

        Assert.False(type.IsEmpty);
    }

    [Fact]
    public void Has_reference_semantics()
    {
        var type = new AnyType(Isolated);

        Assert.Equal(TypeSemantics.Reference, type.Semantics);
    }

    //[Theory]
    //[InlineData(Isolated, "iso Any")]
    //[InlineData(OwnedMutable, "owned mut Any")]
    //[InlineData(Borrowed, "mut Any")]
    //[InlineData(Shared, "Any")]
    //public void ToSourceCodeString_includes_reference_capability(ReferenceCapability capability, string expected)
    //{
    //    var type = new AnyType(capability);

    //    Assert.Equal(expected, type.ToSourceCodeString());
    //}

    //[Theory]
    //[InlineData(Isolated, "iso Any")]
    //[InlineData(OwnedMutable, "owned mut Any")]
    //[InlineData(Borrowed, "mut Any")]
    //[InlineData(Shared, "shared Any")]
    //public void ToILString_includes_reference_capability(ReferenceCapability capability, string expected)
    //{
    //    var type = new AnyType(capability);

    //    Assert.Equal(expected, type.ToILString());
    //}

    //[Theory]
    //[InlineData(Isolated)]
    //[InlineData(OwnedMutable)]
    //[InlineData(Borrowed)]
    //[InlineData(Shared)]
    //public void Has_reference_capability_constructed_with(ReferenceCapability capability)
    //{
    //    var type = new AnyType(capability);

    //    Assert.Equal(capability, type.ReferenceCapability);
    //}

    //[Theory]
    //[InlineData(Isolated)]
    //[InlineData(OwnedMutable)]
    //[InlineData(Borrowed)]
    //[InlineData(Shared)]
    //public void Can_convert_to_reference_capability(ReferenceCapability capability)
    //{
    //    var type = new AnyType(Isolated);

    //    var converted = type.To(capability);

    //    Assert.Equal(capability, converted.ReferenceCapability);
    //}

    [Fact]
    public void Any_types_with_same_reference_capability_are_equal()
    {
        var type1 = new AnyType(ReadOnly);
        var type2 = new AnyType(ReadOnly);

        Assert.Equal(type1, type2);
    }

    [Fact]
    public void Any_types_with_different_reference_capabilities_are_not_equal()
    {
        var type1 = new AnyType(ReadOnly);
        var type2 = new AnyType(Mutable);

        Assert.NotEqual(type1, type2);
    }
}
