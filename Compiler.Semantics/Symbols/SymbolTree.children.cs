using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;

[GeneratedCode("AzothCompilerCodeGen", null)]
public static class ISymbolNodeExtensions
{
    [DebuggerStepThrough]
    public static IEnumerable<ISymbolNode> Children(this ISymbolNode node)
    {
        switch (node)
        {
            default:
                throw ExhaustiveMatch.Failed(node);
            case IPackageSymbolNode n:
                yield return n.MainFacet;
                yield return n.TestingFacet;
                yield break;
            case IPackageFacetSymbolNode n:
                yield return n.GlobalNamespace;
                yield break;
            case INamespaceSymbolNode n:
                foreach (var child in n.Members)
                    yield return child;
                foreach (var child in n.NestedMembers)
                    yield return child;
                yield break;
            case IFunctionSymbolNode n:
                yield break;
            case IClassSymbolNode n:
                foreach (var child in n.Members)
                    yield return child;
                yield break;
            case IStructSymbolNode n:
                foreach (var child in n.Members)
                    yield return child;
                yield break;
            case ITraitSymbolNode n:
                foreach (var child in n.Members)
                    yield return child;
                yield break;
            case IGenericParameterSymbolNode n:
                yield break;
            case IMethodSymbolNode n:
                yield break;
            case IConstructorSymbolNode n:
                yield break;
            case IInitializerSymbolNode n:
                yield break;
            case IFieldSymbolNode n:
                yield break;
            case IAssociatedFunctionSymbolNode n:
                yield break;
        }
    }
}
