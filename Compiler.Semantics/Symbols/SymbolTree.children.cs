using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;

[GeneratedCode("AzothCompilerCodeGen", null)]
public static class IDeclarationNodeExtensions
{
    [DebuggerStepThrough]
    public static IEnumerable<IDeclarationNode> Children(this IDeclarationNode node)
    {
        switch (node)
        {
            default:
                throw ExhaustiveMatch.Failed(node);
            case IPackageDeclarationNode n:
                yield return n.MainFacet;
                yield return n.TestingFacet;
                yield break;
            case IPackageFacetDeclarationNode n:
                yield return n.GlobalNamespace;
                yield break;
            case INamespaceDeclarationNode n:
                foreach (var child in n.Members)
                    yield return child;
                foreach (var child in n.NestedMembers)
                    yield return child;
                yield break;
            case IFunctionDeclarationNode n:
                yield break;
            case IClassDeclarationNode n:
                foreach (var child in n.Members)
                    yield return child;
                yield break;
            case IStructDeclarationNode n:
                foreach (var child in n.Members)
                    yield return child;
                yield break;
            case ITraitDeclarationNode n:
                foreach (var child in n.Members)
                    yield return child;
                yield break;
            case IGenericParameterDeclarationNode n:
                yield break;
            case IMethodDeclarationNode n:
                yield break;
            case IConstructorDeclarationNode n:
                yield break;
            case IInitializerDeclarationNode n:
                yield break;
            case IFieldDeclarationNode n:
                yield break;
            case IAssociatedFunctionDeclarationNode n:
                yield break;
        }
    }
}
