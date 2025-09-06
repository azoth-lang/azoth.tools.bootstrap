using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Framework;
using DotNet.Collections.Generic;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;

internal static partial class InheritanceAspect
{
    #region Type Definitions
    public static partial IFixedSet<ITypeMemberDeclarationNode> TypeDefinition_InclusiveMembers(ITypeDefinitionNode node)
    {
        var inclusiveMembers = node.Members.Where<ITypeMemberDeclarationNode>(m => m.Name is not null)
                                   .ToMultiMapHashSet(m => m.Name!);
        foreach (var supertype in node.AllSupertypeNames.Select(t => t.ReferencedDeclaration))
            AddInheritedMembers(inclusiveMembers, supertype);
        var anyType = node.ContainingLexicalScope.PackageNames.Lookup(BareTypeConstructor.Any);
        AddInheritedMembers(inclusiveMembers, anyType);
        return inclusiveMembers.Values.SelectMany().Concat(node.Members.Where(m => m.Name is null))
                               .ToFixedSet();
    }

    private static void AddInheritedMembers(
        MultiMapHashSet<OrdinaryName, ITypeMemberDeclarationNode> members,
        ITypeDeclarationNode? fromDeclaration)
    {
        if (fromDeclaration is null)
            return;

        var fromDeclarationInclusiveMembers = fromDeclaration.InclusiveMembers;
        foreach (var inheritedMember in fromDeclarationInclusiveMembers)
        {
            var name = inheritedMember.Name;
            // TODO actually base on signature
            if (name is null || members.ContainsKey(name))
                continue;

            members.TryToAddMapping(name, inheritedMember);
        }
    }
    #endregion
}
