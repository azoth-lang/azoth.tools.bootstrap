using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;
using DotNet.Collections.Generic;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;

internal static partial class InheritanceAspect
{
    public static partial IFixedSet<IClassMemberDeclarationNode> ClassDefinition_InclusiveMembers(IClassDefinitionNode node)
        // Explicit type argument required because it is used as a filter and would otherwise be too specific
        => InclusiveMembers<IClassMemberDeclarationNode>(node, node.Members);

    public static partial IFixedSet<IStructMemberDeclarationNode> StructDefinition_InclusiveMembers(IStructDefinitionNode node)
        // Explicit type argument required because it is used as a filter and would otherwise be too specific
        => InclusiveMembers<IStructMemberDeclarationNode>(node, node.Members);

    public static partial IFixedSet<ITraitMemberDeclarationNode> TraitDefinition_InclusiveMembers(ITraitDefinitionNode node)
        // Explicit type argument required because it is used as a filter and would otherwise be too specific
        => InclusiveMembers<ITraitMemberDeclarationNode>(node, node.Members);

    private static IFixedSet<TMemberDeclaration> InclusiveMembers<TMemberDeclaration>(
        ITypeDefinitionNode node,
        IFixedSet<TMemberDeclaration> memberDefinitionNodes)
        where TMemberDeclaration : ITypeMemberDeclarationNode
    {
        var inclusiveMembers = memberDefinitionNodes.Where(m => m.Name is not null)
                                                    .ToMultiMapHashSet(m => m.Name!);
        foreach (var supertype in node.AllSupertypeNames.Select(t => t.ReferencedDeclaration))
            AddInheritedMembers(inclusiveMembers, supertype);
        var anyType = node.ContainingLexicalScope.PackageNames.Lookup(IAntetype.Any);
        AddInheritedMembers(inclusiveMembers, anyType);
        return inclusiveMembers.Values.SelectMany().Concat(memberDefinitionNodes.Where(m => m.Name is null))
                               .ToFixedSet();
    }

    private static void AddInheritedMembers<TMemberDeclaration>(
        MultiMapHashSet<StandardName, TMemberDeclaration> members,
        ITypeDeclarationNode? fromDeclaration)
        where TMemberDeclaration : ITypeMemberDeclarationNode
    {
        if (fromDeclaration is null)
            return;

        foreach (var inheritedMember in fromDeclaration.InclusiveMembers.OfType<TMemberDeclaration>())
        {
            var name = inheritedMember.Name;
            // TODO actually base on signature
            if (name is null || members.ContainsKey(name))
                continue;

            members.TryToAddMapping(name, inheritedMember);
        }
    }
}
