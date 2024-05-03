using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;

internal class ReferencedNamespaceSymbolNode : ReferencedChildSymbolNode, INamespaceSymbolNode
{
    public override NamespaceSymbol Symbol { get; }

    public ReferencedNamespaceSymbolNode(NamespaceSymbol symbol)
    {
        Symbol = symbol;
    }
}
