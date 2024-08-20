using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;

internal static class DiagnosticsAspect
{
    public static DiagnosticCollection Package(IPackageNode node)
    {
        var diagnostics = new DiagnosticCollectionBuilder
        {
            CollectForFacet(node.MainFacet),
            CollectForFacet(node.TestingFacet)
        };
        return diagnostics.Build();
    }

    private static IEnumerable<Diagnostic> CollectForFacet(IPackageFacetNode node)
        => node.CompilationUnits.SelectMany(cu => cu.Diagnostics);

    public static void CompilationUnit_ContributeDiagnostics(ICompilationUnitNode node, DiagnosticCollectionBuilder diagnostics)
        => diagnostics.Add(node.Syntax.Diagnostics);
}
