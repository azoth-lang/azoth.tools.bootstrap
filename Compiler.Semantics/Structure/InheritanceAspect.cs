using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Framework;
using DotNet.Collections.Generic;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;

internal static partial class InheritanceAspect
{
    public static partial IFixedSet<ITypeMemberDeclarationNode> ClassDefinition_InclusiveMembers(IClassDefinitionNode node)
        => InclusiveMembers(node, node.Members);

    public static partial IFixedSet<ITypeMemberDeclarationNode> StructDefinition_InclusiveMembers(IStructDefinitionNode node)
        => InclusiveMembers(node, node.Members);

    public static partial IFixedSet<ITypeMemberDeclarationNode> TraitDefinition_InclusiveMembers(ITraitDefinitionNode node)
        => InclusiveMembers(node, node.Members);

    private static IFixedSet<ITypeMemberDeclarationNode> InclusiveMembers(
        ITypeDefinitionNode node,
        IFixedSet<ITypeMemberDeclarationNode> memberDefinitionNodes)
    {
        var inclusiveMembers = memberDefinitionNodes.Where(m => m.Name is not null)
                                                    .ToMultiMapHashSet(m => m.Name!);
        foreach (var supertype in node.AllSupertypeNames.Select(t => t.ReferencedDeclaration))
            AddInheritedMembers(inclusiveMembers, supertype);
        var anyType = node.ContainingLexicalScope.PackageNames.Lookup(BareTypeConstructor.Any);
        AddInheritedMembers(inclusiveMembers, anyType);
        return inclusiveMembers.Values.SelectMany().Concat(memberDefinitionNodes.Where(m => m.Name is null))
                               .ToFixedSet();
    }

    private static void AddInheritedMembers<TMemberDeclaration>(
        MultiMapHashSet<OrdinaryName, TMemberDeclaration> members,
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
