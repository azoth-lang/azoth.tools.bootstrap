using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;

internal sealed class SemanticNamespaceSymbolNode : SemanticChildSymbolNode, INamespaceSymbolNode
{
    public override NamespaceSymbol Symbol { get; }
    public IFixedList<INamespaceMemberSymbolNode> Members { get; }

    public SemanticNamespaceSymbolNode(NamespaceSymbol symbol, IEnumerable<INamespaceMemberSymbolNode> members)
    {
        Symbol = symbol;
        Members = FixedList.Create(members);
    }
}
