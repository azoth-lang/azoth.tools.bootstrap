using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.ConstValue;
using Xunit;
using static Azoth.Tools.Bootstrap.Compiler.Types.Capabilities.Capability;

namespace Azoth.Tools.Bootstrap.Tests.Unit.Compiler.Types;

[Trait("Category", "Types")]
public class DataTypeExtensionsTests
{
    /// <summary>
    /// A numeric conversion is required
    /// </summary>
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(int.MaxValue)]
    [InlineData((long)int.MaxValue + 1)]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    [InlineData((long)int.MinValue - 1)]
    public void Integer_constant_types_not_assignable_to_int32(long value)
    {
        var constType = new IntegerConstValueType(value);

        var assignable = DataType.Int32.IsAssignableFrom(constType);

        Assert.False(assignable);
    }

    [Fact]
    public void Underlying_reference_type_of_reference_type_is_itself()
    {
        var referenceType = CapabilityType.CreateClass(Mutable, "Package", "Foo", false, false, "Bar");

        var underlyingType = referenceType.UnderlyingReferenceType();

        Assert.Same(referenceType, underlyingType);
    }

    [Fact]
    public void Underlying_reference_type_of_optional_reference_type_is_reference_type()
    {
        var referenceType = CapabilityType.CreateClass(Mutable, "Package", "Foo", false, false, "Bar");
        var optionalType = new OptionalType(referenceType);

        var underlyingType = optionalType.UnderlyingReferenceType();

        Assert.Same(referenceType, underlyingType);
    }

    [Fact]
    public void No_underlying_reference_type_for_optional_value_type()
    {
        var optionalType = new OptionalType(DataType.Bool);

        var underlyingType = optionalType.UnderlyingReferenceType();

        Assert.Null(underlyingType);
    }

    [Fact]
    public void No_underlying_reference_type_for_value_type()
    {
        var valueType = DataType.Int32;

        var underlyingType = valueType.UnderlyingReferenceType();

        Assert.Null(underlyingType);
    }
}
