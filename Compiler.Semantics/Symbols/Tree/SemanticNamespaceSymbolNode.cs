using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;
using DotNet.Collections.Generic;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;

internal sealed class SemanticNamespaceSymbolNode : SemanticDeclarationSymbolNode, INamespaceSymbolNode
{
    public override IdentifierName Name => Symbol.Name;
    public override NamespaceSymbol Symbol { get; }

    public IFixedList<INamespaceMemberSymbolNode> Members { get; }
    private MultiMapHashSet<StandardName, INamespaceMemberSymbolNode>? membersByName;

    private ValueAttribute<IFixedList<INamespaceMemberSymbolNode>> nestedMembers;
    public IFixedList<INamespaceMemberSymbolNode> NestedMembers
        => nestedMembers.TryGetValue(out var value) ? value : nestedMembers.GetValue(GetNestedMembers);
    private MultiMapHashSet<StandardName, INamespaceMemberSymbolNode>? nestedMembersByName;

    public SemanticNamespaceSymbolNode(NamespaceSymbol symbol, IEnumerable<INamespaceMemberSymbolNode> members)
    {
        Symbol = symbol;
        Members = ChildList.Attach(this, members);
    }

    public IEnumerable<INamespaceMemberSymbolNode> MembersNamed(StandardName named)
        => Members.MembersNamed(ref membersByName, named);

    private IFixedList<INamespaceMemberSymbolNode> GetNestedMembers()
        => Members.OfType<INamespaceSymbolNode>()
                  .SelectMany(ns => ns.Members.Concat(ns.NestedMembers)).ToFixedList();

    public IEnumerable<INamespaceMemberSymbolNode> NestedMembersNamed(StandardName named)
        => NestedMembers.MembersNamed(ref nestedMembersByName, named);
}
