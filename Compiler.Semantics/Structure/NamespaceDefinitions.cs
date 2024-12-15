using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;

internal partial class NamespaceDefinitions
{
    public static partial INamespaceDefinitionNode PackageFacet_GlobalNamespace(IPackageFacetNode node)
    {
        var packageSymbol = node.PackageSymbol;
        var builder = new NamespaceDefinitionNodeBuilder(packageSymbol);
        foreach (var cu in node.CompilationUnits)
            BuildNamespace(packageSymbol, cu.ImplicitNamespaceName, cu.Definitions);
        return Child.Attach(node, builder.Build());

        void BuildMember(NamespaceSymbol namespaceSymbol, INamespaceBlockMemberDefinitionNode definition)
        {
            switch (definition)
            {
                default:
                    throw ExhaustiveMatch.Failed(definition);
                case INamespaceBlockDefinitionNode n:
                    var containingNamespace = n.IsGlobalQualified ? packageSymbol : namespaceSymbol;
                    BuildNamespace(containingNamespace, n.DeclaredNames, n.Members);
                    break;
                case IFunctionDefinitionNode n:
                    builder.Add(namespaceSymbol, n);
                    break;
                case ITypeDefinitionNode n:
                    builder.Add(namespaceSymbol, n);
                    break;
            }
        }

        void BuildNamespace(
            NamespaceSymbol containingNamespace,
            NamespaceName name,
            IEnumerable<INamespaceBlockMemberDefinitionNode> declarations)
        {
            var namespaceSymbol = builder.AddNamespace(containingNamespace, name);
            foreach (var declaration in declarations) BuildMember(namespaceSymbol, declaration);
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
