using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;
using DotNet.Collections.Generic;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;

internal sealed class NamespaceDefinitionNode : ChildNode, INamespaceDefinitionNode
{
    public override ISyntax? Syntax => null;
    public IPackageFacetDeclarationNode Facet => Parent.InheritedFacet(this, this);
    public IdentifierName Name => Symbol.Name;
    public NamespaceSymbol Symbol { get; }
    public IFixedList<INamespaceDefinitionNode> MemberNamespaces { get; }
    public IFixedList<IPackageMemberDefinitionNode> PackageMembers { get; }
    public IFixedList<INamespaceMemberDefinitionNode> Members { get; }
    private MultiMapHashSet<StandardName, INamespaceMemberDeclarationNode>? membersByName;

    private ValueAttribute<IFixedList<INamespaceMemberDeclarationNode>> nestedMembers;
    public IFixedList<INamespaceMemberDeclarationNode> NestedMembers
        => nestedMembers.TryGetValue(out var value) ? value : nestedMembers.GetValue(GetNestedMembers);

    private MultiMapHashSet<StandardName, INamespaceMemberDeclarationNode>? nestedMembersByName;

    public NamespaceDefinitionNode(
        NamespaceSymbol symbol,
        IEnumerable<INamespaceDefinitionNode> memberNamespaces,
        IEnumerable<IPackageMemberDefinitionNode> packageMembers)
    {
        Symbol = symbol;
        MemberNamespaces = ChildList.Attach(this, memberNamespaces);
        PackageMembers = packageMembers.ToFixedList();
        Members = MemberNamespaces.Concat<INamespaceMemberDefinitionNode>(PackageMembers).ToFixedList();
    }

    public IEnumerable<INamespaceMemberDeclarationNode> MembersNamed(StandardName named)
        => Members.MembersNamed(ref membersByName, named);

    private IFixedList<INamespaceMemberDeclarationNode> GetNestedMembers()
        => Members.OfType<INamespaceDeclarationNode>()
                  .SelectMany(ns => ns.Members.Concat(ns.NestedMembers)).ToFixedList();

    public IEnumerable<INamespaceMemberDeclarationNode> NestedMembersNamed(StandardName named)
        => NestedMembers.MembersNamed(ref nestedMembersByName, named);
}
