using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;
using DotNet.Collections.Generic;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree.SymbolNodes;

internal class NamespaceSymbolNode : PackageFacetChildSymbolNode, INamespaceSymbolNode
{
    public override NamespaceSymbol Symbol { get; }

    public override IdentifierName Name => Symbol.Name;
    TypeName INamedDeclarationNode.Name => Name;
    StandardName INamespaceMemberDeclarationNode.Name => Name;

    private ValueAttribute<IFixedList<INamespaceMemberDeclarationNode>> members;
    public IFixedList<INamespaceMemberDeclarationNode> Members
        => members.TryGetValue(out var value) ? value : members.GetValue(GetMembers);
    private MultiMapHashSet<StandardName, INamespaceMemberDeclarationNode>? membersByName;

    private ValueAttribute<IFixedList<INamespaceMemberDeclarationNode>> nestedMembers;
    public IFixedList<INamespaceMemberDeclarationNode> NestedMembers
        => nestedMembers.TryGetValue(out var value) ? value : nestedMembers.GetValue(GetNestedMembers);
    private MultiMapHashSet<StandardName, INamespaceMemberDeclarationNode>? nestedMembersByName;

    public NamespaceSymbolNode(NamespaceSymbol symbol)
    {
        Symbol = symbol;
    }

    private new IFixedList<INamespaceMemberDeclarationNode> GetMembers()
        => ChildList.Attach(this, base.GetMembers().Cast<INamespaceMemberDeclarationNode>());

    private IFixedList<INamespaceMemberDeclarationNode> GetNestedMembers()
        => Members.OfType<INamespaceDeclarationNode>()
                  .SelectMany(ns => ns.Members.Concat(ns.NestedMembers)).ToFixedList();

    public IEnumerable<INamespaceMemberDeclarationNode> MembersNamed(StandardName named)
        => Members.MembersNamed(ref membersByName, named);

    public IEnumerable<INamespaceMemberDeclarationNode> NestedMembersNamed(StandardName named)
        => NestedMembers.MembersNamed(ref nestedMembersByName, named);
}
