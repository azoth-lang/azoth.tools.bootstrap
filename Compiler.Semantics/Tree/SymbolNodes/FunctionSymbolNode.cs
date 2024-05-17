using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree.SymbolNodes;

internal sealed class FunctionSymbolNode : PackageFacetChildSymbolNode, IFunctionSymbolNode
{
    public override StandardName Name => base.Name!;
    public override FunctionSymbol Symbol { get; }

    public FunctionSymbolNode(FunctionSymbol symbol)
    {
        Symbol = symbol;
    }
}
