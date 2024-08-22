using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.NameBinding;

internal static partial class NameLookupAspect
{
    public static partial FixedDictionary<StandardName, IFixedSet<INamespaceMemberDeclarationNode>>
        NamespaceDeclaration_MembersByName(INamespaceDeclarationNode node)
        => node.Members.ToNameLookup();

    public static partial FixedDictionary<StandardName, IFixedSet<INamespaceMemberDeclarationNode>>
        NamespaceDeclaration_NestedMembersByName(INamespaceDeclarationNode node)
        => node.NestedMembers.ToNameLookup();

    public static partial FixedDictionary<StandardName, IFixedSet<IInstanceMemberDeclarationNode>>
        PrimitiveTypeDeclaration_InclusiveInstanceMembersByName(IPrimitiveTypeDeclarationNode node)
        => node.InclusiveMembers.OfType<IInstanceMemberDeclarationNode>().ToNameLookup();

    public static partial FixedDictionary<StandardName, IFixedSet<IAssociatedMemberDeclarationNode>>
        PrimitiveTypeDeclaration_AssociatedMembersByName(IPrimitiveTypeDeclarationNode node)
        => node.Members.OfType<IAssociatedMemberDeclarationNode>().ToNameLookup();

    public static partial FixedDictionary<StandardName, IFixedSet<IInstanceMemberDeclarationNode>>
        UserTypeDeclaration_InclusiveInstanceMembersByName(IUserTypeDeclarationNode node)
        => node.InclusiveMembers.OfType<IInstanceMemberDeclarationNode>().ToNameLookup();

    public static partial FixedDictionary<StandardName, IFixedSet<IAssociatedMemberDeclarationNode>>
        UserTypeDeclaration_AssociatedMembersByName(IUserTypeDeclarationNode node)
        => node.Members.OfType<IAssociatedMemberDeclarationNode>().ToNameLookup();

    private static FixedDictionary<StandardName, IFixedSet<T>> ToNameLookup<T>(this IEnumerable<T> members)
        where T : IPackageFacetChildDeclarationNode
        => members.GroupBy(m => m.Name).Where(g => g.Key is not null)
                  .ToFixedDictionary(g => g.Key!, g => g.ToFixedSet());
}
