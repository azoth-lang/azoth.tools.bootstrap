using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;

internal class UnreachableCodeAspect
{
    public static void BlockExpression_ContributeDiagnostics(
        IBlockExpressionNode node,
        Diagnostics diagnostics)
    {
        // Check if there is a statement after a result statement
        IResultStatementNode? resultStatement = null;
        foreach (var statement in node.Statements)
            if (resultStatement is not null)
            {
                diagnostics.Add(OtherSemanticError.StatementAfterResult(statement.File, statement.Syntax.Span));
                break;
            }
            else if (statement is IResultStatementNode result)
                resultStatement = result;
    }
}
