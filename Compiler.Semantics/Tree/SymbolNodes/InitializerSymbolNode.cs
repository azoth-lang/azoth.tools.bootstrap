using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree.SymbolNodes;
internal class InitializerSymbolNode : PackageFacetChildSymbolNode, IInitializerSymbolNode
{
    public override InitializerSymbol Symbol { get; }
    public override IdentifierName? Name => Symbol.Name;

    public InitializerSymbolNode(InitializerSymbol symbol)
    {
        Symbol = symbol;
    }
}
