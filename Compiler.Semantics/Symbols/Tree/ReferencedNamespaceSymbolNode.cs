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
        => members.TryGetValue(out var value) ? value : members.GetValue(GetMembers);
    private MultiMapHashSet<StandardName, INamespaceMemberSymbolNode>? membersByName;

    private ValueAttribute<IFixedList<INamespaceMemberSymbolNode>> nestedMembers;
    public IFixedList<INamespaceMemberSymbolNode> NestedMembers
        => nestedMembers.TryGetValue(out var value) ? value : nestedMembers.GetValue(GetNestedMembers);
    private MultiMapHashSet<StandardName, INamespaceMemberSymbolNode>? nestedMembersByName;

    public ReferencedNamespaceSymbolNode(NamespaceSymbol symbol)
    {
        Symbol = symbol;
    }

    private new IFixedList<INamespaceMemberSymbolNode> GetMembers()
        => ChildList.Attach(this, base.GetMembers().Cast<INamespaceMemberSymbolNode>());

    private IFixedList<INamespaceMemberSymbolNode> GetNestedMembers()
        => Members.OfType<INamespaceSymbolNode>()
                  .SelectMany(ns => ns.Members.Concat(ns.NestedMembers)).ToFixedList();

    public IEnumerable<INamespaceMemberSymbolNode> MembersNamed(StandardName named)
        => Members.MembersNamed(ref membersByName, named);

    public IEnumerable<INamespaceMemberSymbolNode> NestedMembersNamed(StandardName named)
        => NestedMembers.MembersNamed(ref nestedMembersByName, named);

}
