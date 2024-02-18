using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.ConstValue;
using Xunit;

namespace Azoth.Tools.Bootstrap.Tests.Unit.Compiler.Types;

[Trait("Category", "Types")]
public class BoolConstValueTypeTests
{
    [Fact]
    public void Is_constant()
    {
        var type = BoolConstValueType.True;

        Assert.True(type.IsTypeOfConstValue);
    }

    [Fact]
    public void True_type_has_true_value()
    {
        var type = BoolConstValueType.True;

        Assert.True(type.Value);
    }

    [Fact]
    public void False_type_has_false_value()
    {
        var type = BoolConstValueType.False;

        Assert.False(type.Value);
    }

    [Fact]
    public void True_has_special_name_and_ToILString()
    {
        var type = BoolConstValueType.True;

        Assert.Equal(SpecialTypeName.True, type.Name);
        Assert.Equal("Value[true]", type.ToILString());
    }

    [Fact]
    public void False_has_special_name_and_ToILString()
    {
        var type = BoolConstValueType.False;

        Assert.Equal(SpecialTypeName.False, type.Name);
        Assert.Equal("Value[false]", type.ToILString());
    }

    [Fact]
    public void Converts_to_non_constant_bool_type()
    {
        var type = BoolConstValueType.False;

        var nonConstant = type.ToNonConstantType();

        Assert.Same(DataType.Bool, nonConstant);
    }

    [Fact]
    public void Bool_constant_types_with_same_value_are_equal()
    {
        Assert.Equal(BoolConstValueType.True, BoolConstValueType.True);
        Assert.Equal(BoolConstValueType.False, BoolConstValueType.False);
    }

    [Fact]
    public void Bool_constant_types_with_different_value_are_not_equal()
    {
        Assert.NotEqual(BoolConstValueType.True, BoolConstValueType.False);
    }

    [Fact]
    public void Bool_type_not_equal_to_bool_constant_type()
    {
        Assert.NotEqual<NonEmptyType>(BoolType.Instance, BoolConstValueType.True);
        Assert.NotEqual<NonEmptyType>(BoolType.Instance, BoolConstValueType.False);
    }
}
