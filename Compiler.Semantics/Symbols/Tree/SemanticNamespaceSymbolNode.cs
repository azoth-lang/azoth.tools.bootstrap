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

    public IFixedList<INamespaceMemberDeclarationNode> Members { get; }
    private MultiMapHashSet<StandardName, INamespaceMemberDeclarationNode>? membersByName;

    private ValueAttribute<IFixedList<INamespaceMemberDeclarationNode>> nestedMembers;
    public IFixedList<INamespaceMemberDeclarationNode> NestedMembers
        => nestedMembers.TryGetValue(out var value) ? value : nestedMembers.GetValue(GetNestedMembers);
    private MultiMapHashSet<StandardName, INamespaceMemberDeclarationNode>? nestedMembersByName;

    public SemanticNamespaceSymbolNode(NamespaceSymbol symbol, IEnumerable<INamespaceMemberDeclarationNode> members)
    {
        Symbol = symbol;
        Members = ChildList.Attach(this, members);
    }

    public IEnumerable<INamespaceMemberDeclarationNode> MembersNamed(StandardName named)
        => Members.MembersNamed(ref membersByName, named);

    private IFixedList<INamespaceMemberDeclarationNode> GetNestedMembers()
        => Members.OfType<INamespaceDeclarationNode>()
                  .SelectMany(ns => ns.Members.Concat(ns.NestedMembers)).ToFixedList();

    public IEnumerable<INamespaceMemberDeclarationNode> NestedMembersNamed(StandardName named)
        => NestedMembers.MembersNamed(ref nestedMembersByName, named);
}
