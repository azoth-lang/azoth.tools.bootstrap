using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.NameBinding;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class NamespaceDefinitionNode : ChildNode, INamespaceDefinitionNode
{
    public override ISyntax? Syntax => null;
    public IPackageFacetDeclarationNode Facet => Parent.InheritedFacet(this, this);
    public IdentifierName Name => Symbol.Name;
    public NamespaceSymbol Symbol { get; }
    public IFixedList<INamespaceDefinitionNode> MemberNamespaces { get; }
    public IFixedList<IPackageMemberDefinitionNode> PackageMembers { get; }
    public IFixedList<INamespaceMemberDefinitionNode> Members { get; }
    private FixedDictionary<StandardName, IFixedSet<INamespaceMemberDeclarationNode>>? membersByName;
    private bool membersByNameCached;
    public FixedDictionary<StandardName, IFixedSet<INamespaceMemberDeclarationNode>> MembersByName
        => GrammarAttribute.IsCached(in membersByNameCached) ? membersByName!
            : this.Synthetic(ref membersByNameCached, ref membersByName,
                NameLookupAspect.NamespaceDeclaration_MembersByName);
    private ValueAttribute<IFixedList<INamespaceMemberDeclarationNode>> nestedMembers;
    public IFixedList<INamespaceMemberDeclarationNode> NestedMembers
        => nestedMembers.TryGetValue(out var value) ? value : nestedMembers.GetValue(GetNestedMembers);
    private FixedDictionary<StandardName, IFixedSet<INamespaceMemberDeclarationNode>>? nestedMembersByName;
    private bool nestedMembersByNameCached;
    public FixedDictionary<StandardName, IFixedSet<INamespaceMemberDeclarationNode>> NestedMembersByName
        => GrammarAttribute.IsCached(in nestedMembersByNameCached) ? nestedMembersByName!
            : this.Synthetic(ref nestedMembersByNameCached, ref nestedMembersByName,
                NameLookupAspect.NamespaceDeclaration_NestedMembersByName);

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
        => MembersByName.GetValueOrDefault(named) ?? [];

    private IFixedList<INamespaceMemberDeclarationNode> GetNestedMembers()
        => Members.OfType<INamespaceDeclarationNode>()
                  .SelectMany(ns => ns.Members.Concat(ns.NestedMembers)).ToFixedList();

    public IEnumerable<INamespaceMemberDeclarationNode> NestedMembersNamed(StandardName named)
        => NestedMembersByName.GetValueOrDefault(named) ?? [];
}
