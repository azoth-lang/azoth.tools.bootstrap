using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics;

[GeneratedCode("AzothCompilerCodeGen", null)]
public static class ISemanticNodeExtensions
{
    [DebuggerStepThrough]
    public static IEnumerable<ISemanticNode> Children(this ISemanticNode node)
    {
        switch (node)
        {
            default:
                throw ExhaustiveMatch.Failed(node);
            case IPackageNode n:
                foreach (var child in n.References)
                    yield return child;
                yield return n.MainFacet;
                yield return n.TestingFacet;
                yield break;
            case IPackageReferenceNode n:
                yield break;
            case IPackageFacetNode n:
                foreach (var child in n.CompilationUnits)
                    yield return child;
                yield break;
            case ICompilationUnitNode n:
                foreach (var child in n.UsingDirectives)
                    yield return child;
                foreach (var child in n.Declarations)
                    yield return child;
                yield break;
            case IUsingDirectiveNode n:
                yield break;
            case INamespaceDeclarationNode n:
                foreach (var child in n.UsingDirectives)
                    yield return child;
                foreach (var child in n.Declarations)
                    yield return child;
                yield break;
            case IClassDeclarationNode n:
                foreach (var child in n.GenericParameters)
                    yield return child;
                if (n.BaseTypeName is not null)
                    yield return n.BaseTypeName;
                foreach (var child in n.SupertypeNames)
                    yield return child;
                foreach (var child in n.Members)
                    yield return child;
                yield break;
            case IStructDeclarationNode n:
                foreach (var child in n.GenericParameters)
                    yield return child;
                foreach (var child in n.SupertypeNames)
                    yield return child;
                foreach (var child in n.Members)
                    yield return child;
                yield break;
            case ITraitDeclarationNode n:
                foreach (var child in n.GenericParameters)
                    yield return child;
                foreach (var child in n.SupertypeNames)
                    yield return child;
                foreach (var child in n.Members)
                    yield return child;
                yield break;
            case IGenericParameterNode n:
                yield return n.Constraint;
                yield break;
            case ISupertypeNameNode n:
                yield break;
            case IFunctionDeclarationNode n:
                yield break;
            case ICapabilitySetNode n:
                yield break;
            case ICapabilityNode n:
                yield break;
        }
    }
}
