using Azoth.Tools.Bootstrap.Compiler.Types;
using Xunit;
using static Azoth.Tools.Bootstrap.Compiler.Types.Capabilities.Capability;

namespace Azoth.Tools.Bootstrap.Tests.Unit.Compiler.Types;

[Trait("Category", "Types")]
public class ReferenceTypeTests
{
    [Fact]
    public void Convert_to_non_constant_type_is_same_type()
    {
        var type = CapabilityType.CreateClass(Isolated, "Package", "Foo", false, false, "Bar");

        var nonConstant = type.ToNonConstantType();

        Assert.Same(type, nonConstant);
    }

    [Fact]
    public void With_same_name_and_reference_capability_are_equal()
    {
        var type1 = CapabilityType.CreateClass(Isolated, "Package", "Foo", false, false, "Bar");
        var type2 = CapabilityType.CreateClass(Isolated, "Package", "Foo", false, false, "Bar");

        Assert.Equal(type1, type2);
    }
}
