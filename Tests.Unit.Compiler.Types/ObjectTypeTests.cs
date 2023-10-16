using Azoth.Tools.Bootstrap.Compiler.Types;
using Xunit;
using static Azoth.Tools.Bootstrap.Compiler.Types.ReferenceCapability;

namespace Azoth.Tools.Bootstrap.Tests.Unit.Compiler.Types;

[Trait("Category", "Types")]
public class ObjectTypeTests
{
    [Fact]
    public void Has_reference_semantics()
    {
        var type = ObjectType.Create(Mutable, "Package", "Foo", false, false, true, "Bar");

        Assert.Equal(TypeSemantics.Reference, type.Semantics);
    }

    [Fact]
    public void Convert_to_non_constant_type_is_same_type()
    {
        var type = ObjectType.Create(Isolated, "Package", "Foo", false, false, true, "Bar");

        var nonConstant = type.ToNonConstantType();

        Assert.Same(type, nonConstant);
    }

    [Fact]
    public void With_same_name_and_reference_capability_are_equal()
    {
        var type1 = ObjectType.Create(Isolated, "Package", "Foo", false, false, true, "Bar");
        var type2 = ObjectType.Create(Isolated, "Package", "Foo", false, false, true, "Bar");

        Assert.Equal(type1, type2);
    }
}
