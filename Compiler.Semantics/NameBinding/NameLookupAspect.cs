using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.NameBinding;

internal static partial class NameLookupAspect
{
    public static partial FixedDictionary<OrdinaryName, IFixedSet<INamespaceMemberDeclarationNode>>
        NamespaceDeclaration_MembersByName(INamespaceDeclarationNode node)
        => node.Members.ToNameLookup();

    public static partial FixedDictionary<OrdinaryName, IFixedSet<INamespaceMemberDeclarationNode>>
        NamespaceDeclaration_NestedMembersByName(INamespaceDeclarationNode node)
        => node.NestedMembers.ToNameLookup();

    public static partial FixedDictionary<OrdinaryName, IFixedSet<IInstanceMemberDeclarationNode>>
        BuiltInTypeDeclaration_InclusiveInstanceMembersByName(IBuiltInTypeDeclarationNode node)
        => node.InclusiveMembers.OfType<IInstanceMemberDeclarationNode>().ToNameLookup();

    public static partial FixedDictionary<OrdinaryName, IFixedSet<IAssociatedMemberDeclarationNode>>
        BuiltInTypeDeclaration_AssociatedMembersByName(IBuiltInTypeDeclarationNode node)
        => node.Members.OfType<IAssociatedMemberDeclarationNode>().ToNameLookup();

    public static partial FixedDictionary<OrdinaryName, IFixedSet<IInstanceMemberDeclarationNode>>
        OrdinaryTypeDeclaration_InclusiveInstanceMembersByName(IOrdinaryTypeDeclarationNode node)
        => node.InclusiveMembers.OfType<IInstanceMemberDeclarationNode>().ToNameLookup();

    public static partial FixedDictionary<OrdinaryName, IFixedSet<IAssociatedMemberDeclarationNode>>
        OrdinaryTypeDeclaration_AssociatedMembersByName(IOrdinaryTypeDeclarationNode node)
        => node.Members.OfType<IAssociatedMemberDeclarationNode>().ToNameLookup();

    private static FixedDictionary<OrdinaryName, IFixedSet<T>> ToNameLookup<T>(this IEnumerable<T> members)
        where T : IPackageFacetChildDeclarationNode
        => members.GroupBy(m => m.Name).Where(g => g.Key is not null)
                  .ToFixedDictionary(g => g.Key!, g => g.ToFixedSet());
}
