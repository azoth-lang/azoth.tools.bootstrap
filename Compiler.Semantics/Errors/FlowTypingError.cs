using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;

/// <remarks><see cref="ErrorCodeRange"/> for the ranges of various kinds of error codes.</remarks>
public static class FlowTypingError
{
    public static Diagnostic CannotMoveValue(CodeFile file, IExpressionSyntax moveExpression, IExpressionSyntax referent)
    {
        return new(file, moveExpression.Span, (DiagnosticLevel)DiagnosticLevel.FatalCompilationError, (DiagnosticPhase)DiagnosticPhase.Analysis,
            (int)4001, (string)$"Cannot move value `{file.Code[referent.Span]}`");
    }

    public static Diagnostic CannotFreezeValue(CodeFile file, IExpressionSyntax expression, IExpressionSyntax referent)
    {
        return new(file, expression.Span, (DiagnosticLevel)DiagnosticLevel.FatalCompilationError, (DiagnosticPhase)DiagnosticPhase.Analysis,
            (int)4002, (string)$"Cannot freeze the value `{file.Code[referent.Span]}`");
    }

    public static Diagnostic CannotUnion(CodeFile file, TextSpan span)
    {
        return new(file, span, (DiagnosticLevel)DiagnosticLevel.FatalCompilationError, (DiagnosticPhase)DiagnosticPhase.Analysis,
            (int)4004, (string)"Cannot combine lent values");
    }

    public static Diagnostic CannotReturnLent(CodeFile file, IReturnExpressionSyntax expression)
    {
        return new(file, expression.Span, (DiagnosticLevel)DiagnosticLevel.FatalCompilationError, (DiagnosticPhase)DiagnosticPhase.Analysis,
            (int)4005, (string)"Cannot return lent value from function");
    }
}
