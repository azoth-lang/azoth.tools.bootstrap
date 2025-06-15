using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Syntax;

internal static partial class ComputedAspect
{
    public static partial DiagnosticCollection PackageFacet_Diagnostics(IPackageFacetSyntax node)
    {
        var builder = new DiagnosticCollectionBuilder
        {
            node.CompilationUnits.Select(cu => cu.Diagnostics)
        };
        return builder.Build();
    }

    public static partial IFixedList<IStatementSyntax> ExpressionBody_Statements(IExpressionBodySyntax node)
        => node.ResultStatement.Yield().ToFixedList<IStatementSyntax>();
}
