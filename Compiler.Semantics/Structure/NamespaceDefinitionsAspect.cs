using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Names;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;

internal partial class NamespaceDefinitionsAspect
{
    public static partial INamespaceDefinitionNode PackageFacet_GlobalNamespace(IPackageFacetNode node)
    {
        var builder = new NamespaceDefinitionNodeBuilder();
        foreach (var cu in node.CompilationUnits)
            BuildNamespace(NamespaceName.Global, cu.ImplicitNamespaceName, cu.Definitions);
        return Child.Attach(node, builder.Build());

        void BuildMember(NamespaceName ns, INamespaceBlockMemberDefinitionNode definition)
        {
            switch (definition)
            {
                default:
                    throw ExhaustiveMatch.Failed(definition);
                case INamespaceBlockDefinitionNode n:
                    var containingNamespace = n.IsGlobalQualified ? NamespaceName.Global : ns;
                    BuildNamespace(containingNamespace, n.DeclaredNames, n.Members);
                    break;
                case IFunctionDefinitionNode n:
                    builder.Add(ns, n);
                    break;
                case ITypeDefinitionNode n:
                    builder.Add(ns, n);
                    break;
            }
        }

        void BuildNamespace(
            NamespaceName containingNamespace,
            NamespaceName declaredNamespaceNames,
            IEnumerable<INamespaceBlockMemberDefinitionNode> definitions)
        {
            var ns = builder.AddNamespace(containingNamespace, declaredNamespaceNames);
            foreach (var declaration in definitions)
                BuildMember(ns, declaration);
        }
    }

    public static partial INamespaceDefinitionNode CompilationUnit_ImplicitNamespace(ICompilationUnitNode node)
        => FindNamespace(node.ContainingDeclaration.GlobalNamespace, node.ImplicitNamespaceName);

    private static INamespaceDefinitionNode FindNamespace(INamespaceDefinitionNode containingDeclarationNode, NamespaceName ns)
    {
        // TODO rework
        var current = containingDeclarationNode;
        foreach (var name in ns.Segments)
            current = current.MembersNamed(name).OfType<INamespaceDefinitionNode>().Single();
        return current;
    }

    public static partial INamespaceDefinitionNode NamespaceBlockDefinition_ContainingNamespace(INamespaceBlockDefinitionNode node)
        => node.IsGlobalQualified ? node.Facet.GlobalNamespace : node.ContainingDeclaration;

    public static partial INamespaceDefinitionNode NamespaceBlockDefinition_Definition(INamespaceBlockDefinitionNode node)
        => FindNamespace(node.ContainingNamespace, node.DeclaredNames);
}
