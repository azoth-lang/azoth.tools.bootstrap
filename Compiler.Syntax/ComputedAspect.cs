using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;

namespace Azoth.Tools.Bootstrap.Compiler.Syntax;

internal static partial class ComputedAspect
{
    public static partial DiagnosticCollection Package_Diagnostics(IPackageSyntax node)
    {
        var builder = new DiagnosticCollectionBuilder
        {
            node.CompilationUnits.Concat(node.TestingCompilationUnits).Select(cu => cu.Diagnostics)
        };
        return builder.Build();
    }
}
