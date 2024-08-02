using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree.SymbolNodes;

internal sealed class PrimitiveTypeSymbolNode : BuiltInTypeSymbolNode, IPrimitiveTypeSymbolNode
{
    public override PrimitiveTypeSymbol Symbol { get; }
    public override SpecialTypeName Name => Symbol.Name;

    public PrimitiveTypeSymbolNode(PrimitiveTypeSymbol symbol)
    {
        Symbol = symbol;
    }
}
