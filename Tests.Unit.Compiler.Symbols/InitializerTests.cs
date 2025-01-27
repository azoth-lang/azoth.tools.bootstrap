using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Xunit;

namespace Azoth.Tools.Bootstrap.Tests.Unit.Compiler.Symbols;

[Trait("Category", "Symbols")]
public class InitializerTests : SymbolTestFixture
{
    [Fact]
    public void Default_initializer_has_correct_properties()
    {
        var type = Type();
        var defaultInitializer = InitializerSymbol.CreateDefault(type);

        Assert.Equal(type, defaultInitializer.ContainingSymbol);
        Assert.Null(defaultInitializer.Name);
        Assert.Empty(defaultInitializer.ParameterTypes);
        Assert.Equal(0, defaultInitializer.Arity);
    }
}
