using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;
using DotNet.Collections.Generic;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;

internal class ReferencedNamespaceSymbolNode : ReferencedDeclarationSymbolNode, INamespaceSymbolNode
{
    public override NamespaceSymbol Symbol { get; }
    public override IdentifierName Name => Symbol.Name;

    private ValueAttribute<IFixedList<INamespaceMemberSymbolNode>> members;
    public IFixedList<INamespaceMemberSymbolNode> Members
        => members.TryGetValue(out var value) ? value
            : members.GetValue(this, GetMembers);
    private MultiMapHashSet<StandardName, INamespaceMemberSymbolNode>? membersByName;

    public ReferencedNamespaceSymbolNode(NamespaceSymbol symbol)
    {
        Symbol = symbol;
    }

    private IFixedList<INamespaceMemberSymbolNode> GetMembers(INamespaceSymbolNode _)
        => GetMembers().Cast<INamespaceMemberSymbolNode>().ToFixedList();

    public IEnumerable<INamespaceMemberSymbolNode> MembersNamed(IdentifierName named)
        => Members.MembersNamed(ref membersByName, named);
}
