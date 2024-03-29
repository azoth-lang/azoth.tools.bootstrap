using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;

/// <remarks><see cref="ErrorCodeRange"/> for the ranges of various kinds of error codes.</remarks>
public static class FlowTypingError
{
    public static Diagnostic CannotMoveValue(CodeFile file, IMoveExpressionSyntax expression)
    {
        return new(file, expression.Span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            4001, $"Cannot move value `{file.Code[expression.Referent.Span]}`");
    }

    public static Diagnostic CannotFreezeValue(CodeFile file, IFreezeExpressionSyntax expression)
    {
        return new(file, expression.Span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            4002, $"Cannot freeze the value `{file.Code[expression.Referent.Span]}`");
    }

    public static Diagnostic UseOfPossiblyMovedValue(CodeFile file, TextSpan span)
    {
        return new(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            4003, "Use of possibly moved value");
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
