using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Framework;
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

    #region Type Definitions
    public static partial void StructDefinition_Contribute_Diagnostics(IStructDefinitionNode node, DiagnosticCollectionBuilder diagnostics)
    {
        if (node.Syntax.StructKindModifier is null)
            diagnostics.Add(OtherSemanticError.StructKindRequired(node.Syntax.File, node.Syntax));
    }
    #endregion
}
