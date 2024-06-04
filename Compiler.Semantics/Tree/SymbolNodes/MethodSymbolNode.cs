using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree.SymbolNodes;

internal abstract class MethodSymbolNode : PackageFacetChildSymbolNode, IMethodSymbolNode
{
    public override MethodSymbol Symbol { get; }

    public override IdentifierName Name => Symbol.Name;
    StandardName INamedDeclarationNode.Name => Name;

    private protected MethodSymbolNode(MethodSymbol symbol)
    {
        Symbol = symbol;
    }
}
