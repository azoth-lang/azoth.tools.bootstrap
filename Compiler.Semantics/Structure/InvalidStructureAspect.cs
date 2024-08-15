using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;

internal class InvalidStructureAspect
{
    public static void BlockExpression_ContributeDiagnostics(
        IBlockExpressionNode node,
        DiagnosticCollectionBuilder diagnostics)
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

    public static void ReturnExpression_ContributeDiagnostics(IReturnExpressionNode node, DiagnosticCollectionBuilder diagnostics)
    {
        var expectedReturnType = node.ExpectedReturnType;
        switch (expectedReturnType)
        {
            case null:
                diagnostics.Add(OtherSemanticError.CannotReturnFromFieldInitializer(node.File, node.Syntax));
                break;
            case NeverType:
                diagnostics.Add(TypeError.CannotReturnFromNeverFunction(node.File, node.Syntax.Span));
                break;
            case VoidType when node.IntermediateValue is not null:
                diagnostics.Add(TypeError.MustReturnCorrectType(node.File, node.Syntax.Span, DataType.Void));
                break;
        }
    }
}
