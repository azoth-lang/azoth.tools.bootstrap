using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;

/// <remarks><see cref="ErrorCodeRange"/> for the ranges of various kinds of error codes.</remarks>
public static class FlowTypingError
{
    public static Diagnostic CannotMoveValue(CodeFile file, IExpressionSyntax moveExpression, IExpressionSyntax referent)
    {
        return new(file, moveExpression.Span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            4001, $"Cannot move value `{file.Code[referent.Span]}`");
    }

    public static Diagnostic CannotFreezeValue(CodeFile file, IExpressionSyntax expression, IExpressionSyntax referent)
    {
        return new(file, expression.Span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            4002, $"Cannot freeze the value `{file.Code[referent.Span]}`");
    }

    public static Diagnostic CannotUnion(CodeFile file, TextSpan span)
    {
        return new(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            4004, "Cannot combine lent values");
    }

    public static Diagnostic CannotReturnLent(CodeFile file, IReturnExpressionSyntax expression)
    {
        return new(file, expression.Span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            4005, "Cannot return lent value from function");
    }
}
