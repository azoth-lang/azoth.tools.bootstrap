using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Declarations;
using Azoth.Tools.Bootstrap.Compiler.Semantics.NameBinding;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

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
    private FixedDictionary<StandardName, IFixedSet<INamespaceMemberDeclarationNode>>? membersByName;
    private bool membersByNameCached;
    public FixedDictionary<StandardName, IFixedSet<INamespaceMemberDeclarationNode>> MembersByName
        => GrammarAttribute.IsCached(in membersByNameCached) ? membersByName!
            : this.Synthetic(ref membersByNameCached, ref membersByName,
                NameLookupAspect.NamespaceDeclaration_MembersByName);
    private ValueAttribute<IFixedList<INamespaceMemberDeclarationNode>> nestedMembers;
    public IFixedList<INamespaceMemberDeclarationNode> NestedMembers
        => nestedMembers.TryGetValue(out var value) ? value
            : nestedMembers.GetValue(() => DeclarationsAspect.NamespaceDeclaration_NestedMembers(this));
    private FixedDictionary<StandardName, IFixedSet<INamespaceMemberDeclarationNode>>? nestedMembersByName;
    private bool nestedMembersByNameCached;
    public FixedDictionary<StandardName, IFixedSet<INamespaceMemberDeclarationNode>> NestedMembersByName
        => GrammarAttribute.IsCached(in nestedMembersByNameCached) ? nestedMembersByName!
            : this.Synthetic(ref nestedMembersByNameCached, ref nestedMembersByName,
                NameLookupAspect.NamespaceDeclaration_NestedMembersByName);

    public NamespaceSymbolNode(NamespaceSymbol symbol)
    {
        Symbol = symbol;
    }

    private new IFixedList<INamespaceMemberDeclarationNode> GetMembers()
        => ChildList.Attach(this, base.GetMembers().Cast<INamespaceMemberDeclarationNode>());
}
