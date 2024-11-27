using Azoth.Tools.Bootstrap.Compiler.Types.Legacy;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.ConstValue;
using Xunit;

namespace Azoth.Tools.Bootstrap.Tests.Unit.Compiler.Types.Legacy.ConstValue;

[Trait("Category", "Types")]
public class IntegerConstValueTypeTests
{
    [Theory]
    [InlineData(-23)]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(42)]
    [InlineData(234_324_234_325)]
    public void Has_integer_value(long value)
    {
        var type = new IntegerConstValueType(value);

        Assert.Equal(value, type.Value);
    }

    [Fact]
    public void Converts_to_non_constant_int_type()
    {
        // TODO larger values need to convert to larger integer types

        var type = new IntegerConstValueType(42);

        var nonConstant = type.ToNonConstValueType();

        Assert.Same(IType.Int, nonConstant);
    }

    [Theory]
    [InlineData(-234234)]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(234234524)]
    public void Integer_constant_types_with_same_value_are_equal(int value)
    {
        var type1 = new IntegerConstValueType(value);
        var type2 = new IntegerConstValueType(value);

        Assert.Equal(type1, type2);
    }
}
