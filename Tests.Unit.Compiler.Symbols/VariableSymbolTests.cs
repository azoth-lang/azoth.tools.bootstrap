using Xunit;

namespace Azoth.Tools.Bootstrap.Tests.Unit.Compiler.Symbols;

[Trait("Category", "Symbols")]
public class VariableSymbolTests : SymbolTestFixture
{
    [Fact]
    public void Has_properties_constructed_with()
    {
        var func = Func();
        var dataType = DataType();
        var symbol = Parameter("foo", func, 42, true, true, dataType);

        Assert.Equal(func, symbol.ContainingSymbol);
        Assert.Equal(Name("foo"), symbol.Name);
        Assert.Equal(42, symbol.DeclarationNumber);
        Assert.True(symbol.IsMutableBinding);
        Assert.True(symbol.IsLentBinding);
        Assert.Equal(dataType, symbol.Type);
        Assert.True(symbol.IsParameter);
    }

    [Fact]
    public void Variables_with_same_name_mutability_and_type_are_equal()
    {
        var varA = LocalVariable("a");
        var varACopy = LocalVariable(varA);

        Assert.Equal(varA, varACopy);
    }

    [Fact]
    public void Variables_with_different_mutability_are_not_equal()
    {
        var varA1 = LocalVariable("a", mut: true);
        var varA2 = LocalVariable(varA1, mut: false);

        Assert.NotEqual(varA1, varA2);
    }

    [Fact]
    public void Variables_with_different_types_are_not_equal()
    {
        var varA1 = LocalVariable("a", type: DataType("T1"));
        var varA2 = LocalVariable(varA1, type: DataType("T2"));

        Assert.NotEqual(varA1, varA2);
    }
}
