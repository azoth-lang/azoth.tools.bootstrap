using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;

internal class UnreachableCodeAspect
{
    public static void BlockExpression_StatementAfterResultStatement(
        IBlockExpressionNode node,
        Diagnostics diagnostics)
    {
        IResultStatementNode? resultStatement = null;
        foreach (var statement in node.Statements)
            if (resultStatement is not null)
            {
                diagnostics.Add(OtherSemanticError.StatementAfterResult(statement.File, statement.Syntax.Span));
                return;
            }
            else if (statement is IResultStatementNode result)
                resultStatement = result;
    }
}
