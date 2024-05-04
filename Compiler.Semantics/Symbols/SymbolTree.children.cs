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
                yield return n.Facet;
                foreach (var child in n.Members)
                    yield return child;
                yield break;
            case IClassSymbolNode n:
                yield return n.Facet;
                foreach (var child in n.Members)
                    yield return child;
                yield break;
            case IStructSymbolNode n:
                yield return n.Facet;
                foreach (var child in n.Members)
                    yield return child;
                yield break;
            case ITraitSymbolNode n:
                yield return n.Facet;
                foreach (var child in n.Members)
                    yield return child;
                yield break;
            case IFunctionSymbolNode n:
                yield return n.Facet;
                yield break;
        }
    }
}
