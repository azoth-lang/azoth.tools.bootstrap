using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree.SymbolNodes;

internal sealed class EmptyTypeSymbolNode : BuiltInTypeSymbolNode, IPrimitiveTypeSymbolNode
{
    public override EmptyTypeSymbol Symbol { get; }
    public override SpecialTypeName Name => Symbol.Name;

    public EmptyTypeSymbolNode(EmptyTypeSymbol symbol)
    {
        Symbol = symbol;
    }
}
