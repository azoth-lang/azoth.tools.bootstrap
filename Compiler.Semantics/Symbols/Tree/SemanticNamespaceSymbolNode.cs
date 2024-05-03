using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;

internal sealed class SemanticNamespaceSymbolNode : SemanticChildSymbolNode, INamespaceSymbolNode
{
    public override NamespaceSymbol Symbol { get; }

    public SemanticNamespaceSymbolNode(NamespaceSymbol symbol)
    {
        Symbol = symbol;
    }
}
