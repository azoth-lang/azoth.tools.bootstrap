using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;

internal static class DiagnosticsAspect
{
    public static void CompilationUnit_Contribute_This_Diagnostics(ICompilationUnitNode node, DiagnosticCollectionBuilder diagnostics)
        => diagnostics.Add(node.Syntax.Diagnostics);
}
