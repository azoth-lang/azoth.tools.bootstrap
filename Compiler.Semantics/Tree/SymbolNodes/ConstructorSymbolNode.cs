using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree.SymbolNodes;

internal sealed class ConstructorSymbolNode : FacetChildSymbolNode, IConstructorSymbolNode
{
    public override ConstructorSymbol Symbol { get; }
    public override IdentifierName? Name => Symbol.Name;

    public ConstructorSymbolNode(ConstructorSymbol symbol)
    {
        Symbol = symbol;
    }
}
