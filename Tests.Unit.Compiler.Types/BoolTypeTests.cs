using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Xunit;

namespace Azoth.Tools.Bootstrap.Tests.Unit.Compiler.Types;

[Trait("Category", "Types")]
public class BoolTypeTests
{
    [Fact]
    public void Is_simple_value_type()
    {
        var type = BoolType.Instance;

        Assert.OfType<SimpleType>(type);
        Assert.OfType<ValueType>(type);
    }

    [Fact]
    public void Is_not_constant()
    {
        var type = BoolType.Instance;

        Assert.False(type.IsTypeOfConstant);
    }

    [Fact]
    public void Is_known_type()
    {
        var type = BoolType.Instance;

        Assert.True(type.IsFullyKnown);
    }

    [Fact]
    public void Is_not_empty_type()
    {
        var type = BoolType.Instance;

        Assert.False(type.IsEmpty);
    }

    [Fact]
    public void Has_copy_semantics()
    {
        var type = BoolType.Instance;

        Assert.Equal(TypeSemantics.CopyValue, type.Semantics);
    }

    [Fact]
    public void Has_special_name_bool()
    {
        var type = BoolType.Instance;

        Assert.Equal(SpecialTypeName.Bool, type.Name);
    }

    [Fact]
    public void Has_proper_ToSourceCodeString()
    {
        var type = BoolType.Instance;

        Assert.Equal("bool", type.ToSourceCodeString());
    }

    [Fact]
    public void Convert_to_read_only_has_no_effect()
    {
        var type = BoolType.Instance;

        var @readonly = type.WithoutWrite();

        Assert.Equal(type, @readonly);
    }

    [Fact]
    public void Bool_type_equal_to_itself()
    {
        Assert.Equal(BoolType.Instance, BoolType.Instance);
    }

    [Fact]
    public void Bool_type_not_equal_to_bool_constant_type()
    {
        Assert.NotEqual(BoolType.Instance, DataType.True);
        Assert.NotEqual(BoolType.Instance, DataType.True);
    }
}
