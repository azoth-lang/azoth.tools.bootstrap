using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;

internal static class DiagnosticsAspect
{
    public static IFixedList<Diagnostic> Package(IPackageNode node)
        => CollectForFacet(node.MainFacet).Concat(CollectForFacet(node.TestingFacet))
                                          .OrderBy(d => d.StartPosition).ThenBy(d => d.EndPosition)
                                          .ToFixedList();

    private static IEnumerable<Diagnostic> CollectForFacet(IPackageFacetNode node)
        => node.CompilationUnits.SelectMany(cu => cu.Diagnostics);

    public static void CompilationUnit_ContributeDiagnostics(ICompilationUnitNode node, DiagnosticsBuilder diagnostics)
        => diagnostics.Add(node.Syntax.Diagnostics);
}
