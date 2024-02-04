using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Xunit;

namespace Azoth.Tools.Bootstrap.Tests.Unit.Compiler.Types;

[Trait("Category", "Types")]
public class BoolValueTypeTests
{
    [Fact]
    public void Is_a_boolean_type()
    {
        var type = BoolValueType.True;

        Assert.OfType<BoolType>(type);
    }

    [Fact]
    public void Is_constant()
    {
        var type = BoolValueType.True;

        Assert.True(type.IsTypeOfValue);
    }

    [Fact]
    public void True_type_has_true_value()
    {
        var type = BoolValueType.True;

        Assert.True(type.Value);
    }

    [Fact]
    public void False_type_has_false_value()
    {
        var type = BoolValueType.False;

        Assert.False(type.Value);
    }

    [Fact]
    public void True_has_special_name_and_ToILString()
    {
        var type = BoolValueType.True;

        Assert.Equal(SpecialTypeName.True, type.Name);
        Assert.Equal("Value[true]", type.ToILString());
    }

    [Fact]
    public void False_has_special_name_and_ToILString()
    {
        var type = BoolValueType.False;

        Assert.Equal(SpecialTypeName.False, type.Name);
        Assert.Equal("Value[false]", type.ToILString());
    }

    [Fact]
    public void Converts_to_non_constant_bool_type()
    {
        var type = BoolValueType.False;

        var nonConstant = type.ToNonConstantType();

        Assert.Same(DataType.Bool, nonConstant);
    }

    [Fact]
    public void Bool_constant_types_with_same_value_are_equal()
    {
        Assert.Equal(BoolValueType.True, BoolValueType.True);
        Assert.Equal(BoolValueType.False, BoolValueType.False);
    }

    [Fact]
    public void Any_types_with_different_reference_capabilities_are_not_equal()
    {
        Assert.NotEqual(BoolValueType.True, BoolValueType.False);
    }
}
