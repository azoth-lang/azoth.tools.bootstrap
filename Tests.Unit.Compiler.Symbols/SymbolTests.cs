using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Moq;
using Xunit;
using DT = Azoth.Tools.Bootstrap.Compiler.Types.DataType;

namespace Azoth.Tools.Bootstrap.Tests.Unit.Compiler.Symbols;

[Trait("Category", "Symbols")]
public class SymbolTests : SymbolTestFixture
{
    [Fact]
    public void Symbol_not_in_namespace_is_global()
    {
        var symbol = new PrimitiveTypeSymbol(DT.Void);

        Assert.True(symbol.IsGlobal);
    }

    [Fact]
    public void Symbol_in_namespace_is_not_global()
    {
        var ns = Namespace();
        var className = Name("My_Class");
        var type = DeclaredObjectType.Create(ns.Package!.Name, ns.Name, className, false);
        var symbol = new ObjectTypeSymbol(ns, type);

        Assert.False(symbol.IsGlobal);
    }
}
