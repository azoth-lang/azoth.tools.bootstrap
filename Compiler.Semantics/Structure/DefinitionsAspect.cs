using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core.Types;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;
using MoreLinq;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;

internal static partial class DefinitionsAspect
{
    public static partial IFunctionDefinitionNode? Package_EntryPoint(IPackageNode node)
        // TODO warn on and remove main functions that don't have correct parameters or types
        // TODO compiler error on multiple main functions
        => node.MainFacet.Definitions.OfType<IFunctionDefinitionNode>().SingleOrDefault(f => f.Name == "main");

    public static partial IFixedSet<IFacetMemberDefinitionNode> PackageFacet_Definitions(IPackageFacetNode node)
    {
        return node.CompilationUnits.SelectMany(n => n.Definitions)
                   .SelectMany(n => MoreEnumerable.TraverseDepthFirst(n, NamespaceChildren))
                   .OfType<IFacetMemberDefinitionNode>().ToFixedSet();

        static IEnumerable<INamespaceBlockMemberDefinitionNode> NamespaceChildren(INamespaceBlockMemberDefinitionNode m)
            => (m as INamespaceBlockDefinitionNode)?.Members ?? Enumerable.Empty<INamespaceBlockMemberDefinitionNode>();
    }

    public static partial void CompilationUnit_Contribute_Diagnostics(ICompilationUnitNode node, DiagnosticCollectionBuilder diagnostics)
        => diagnostics.Add(node.Syntax.Diagnostics);

    public static partial IFixedList<INamespaceMemberDefinitionNode> NamespaceDefinition_Members(INamespaceDefinitionNode node)
        => node.MemberNamespaces.Concat<INamespaceMemberDefinitionNode>(node.PackageMembers).ToFixedList();

    #region Member Definitions
    public static partial TypeVariance AssociatedTypeDefinition_Variance(IAssociatedTypeDefinitionNode node)
        => node.Syntax.Variance switch
        {
            null => TypeVariance.Invariant,
            IInKeywordToken _ => TypeVariance.Contravariant,
            IOutKeywordToken _ => TypeVariance.Covariant,
            _ => throw ExhaustiveMatch.Failed(node.Syntax.Variance),
        };
    #endregion
}
