using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Xunit;

namespace Azoth.Tools.Bootstrap.Tests.Unit.Compiler.Symbols;

[Trait("Category", "Symbols")]
public class ConstructorTests : SymbolTestFixture
{
    [Fact]
    public void Default_constructor_has_correct_properties()
    {
        var type = Type();
        var defaultConstructor = ConstructorSymbol.CreateDefault(type);

        Assert.Equal(type, defaultConstructor.ContainingSymbol);
        Assert.Null(defaultConstructor.Name);
        Assert.Empty(defaultConstructor.ParameterDataTypes);
        Assert.Equal(0, defaultConstructor.Arity);
    }
}
