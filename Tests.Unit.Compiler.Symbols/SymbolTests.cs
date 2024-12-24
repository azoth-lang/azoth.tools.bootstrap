using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Xunit;

namespace Azoth.Tools.Bootstrap.Tests.Unit.Compiler.Symbols;

[Trait("Category", "Symbols")]
public class SymbolTests : SymbolTestFixture
{
    [Fact]
    public void Symbol_not_in_namespace_is_global()
    {
        var symbol = new BuiltInTypeSymbol(BareTypeConstructor.Byte);

        Assert.True(symbol.IsGlobal);
    }

    [Fact]
    public void Symbol_in_namespace_is_not_global()
    {
        var ns = Namespace();
        var type = BareTypeConstructor.CreateClass(ns.Package!.Name, ns.Name, isAbstract: false,
            isConst: false, "My_Class");
        var symbol = new OrdinaryTypeSymbol(ns, type);

        Assert.False(symbol.IsGlobal);
    }
}
