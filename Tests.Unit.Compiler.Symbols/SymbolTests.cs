using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Declared;
using Xunit;

namespace Azoth.Tools.Bootstrap.Tests.Unit.Compiler.Symbols;

[Trait("Category", "Symbols")]
public class SymbolTests : SymbolTestFixture
{
    [Fact]
    public void Symbol_not_in_namespace_is_global()
    {
        var symbol = new BuiltInTypeSymbol(DeclaredType.Byte);

        Assert.True(symbol.IsGlobal);
    }

    [Fact]
    public void Symbol_in_namespace_is_not_global()
    {
        var ns = Namespace();
        var type = ObjectType.CreateClass(ns.Package!.Name, ns.Name, isAbstract: false,
            isConst: false, "My_Class");
        var symbol = new OrdinaryTypeSymbol(ns, type);

        Assert.False(symbol.IsGlobal);
    }
}
