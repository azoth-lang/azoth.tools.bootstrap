using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;

internal class ReferencedNamespaceSymbolNode : ReferencedChildSymbolNode, INamespaceSymbolNode
{
    public override NamespaceSymbol Symbol { get; }

    private ValueAttribute<IFixedList<INamespaceMemberSymbolNode>> members;
    public IFixedList<INamespaceMemberSymbolNode> Members
        => members.TryGetValue(out var value) ? value
            : members.GetValue(this, GetMembers);

    public ReferencedNamespaceSymbolNode(NamespaceSymbol symbol)
    {
        Symbol = symbol;
    }

    private IFixedList<INamespaceMemberSymbolNode> GetMembers(INamespaceSymbolNode _)
        => base.GetMembers().Cast<INamespaceMemberSymbolNode>().ToFixedList();
}
