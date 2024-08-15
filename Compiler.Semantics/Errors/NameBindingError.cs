using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;

/// <remarks><see cref="ErrorCodeRange"/> for the ranges of various kinds of error codes.</remarks>
public static class NameBindingError
{
    public static Diagnostic CouldNotBindName(CodeFile file, TextSpan span)
    {
        return new(file, span, (DiagnosticLevel)DiagnosticLevel.FatalCompilationError, (DiagnosticPhase)DiagnosticPhase.Analysis,
            (int)5001, (string)$"The name `{file.Code[span]}` is not defined in this scope.");
    }

    public static Diagnostic AmbiguousName(CodeFile file, TextSpan span)
    {
        return new(file, span, (DiagnosticLevel)DiagnosticLevel.FatalCompilationError, (DiagnosticPhase)DiagnosticPhase.Analysis,
            (int)5002, (string)$"The name `{file.Code[span]}` is ambiguous.");
    }

    public static Diagnostic CouldNotBindMember(CodeFile file, TextSpan span)
    {
        return new(file, span, (DiagnosticLevel)DiagnosticLevel.FatalCompilationError, (DiagnosticPhase)DiagnosticPhase.Analysis,
            (int)5003, (string)$"Could not find member `{file.Code[span]}` on object.");
    }

    public static Diagnostic CouldNotBindConstructor(CodeFile file, TextSpan span)
    {
        return new(file, span, (DiagnosticLevel)DiagnosticLevel.FatalCompilationError, (DiagnosticPhase)DiagnosticPhase.Analysis,
            (int)5004, (string)"Type doesn't have a constructor with this name and matching arguments.");
    }

    public static Diagnostic AmbiguousConstructorCall(CodeFile file, TextSpan span)
    {
        return new(file, span, (DiagnosticLevel)DiagnosticLevel.FatalCompilationError, (DiagnosticPhase)DiagnosticPhase.Analysis,
            (int)5005, (string)"Constructor call is ambiguous.");
    }

    public static Diagnostic CouldNotBindFunction(CodeFile file, IInvocationExpressionSyntax invocation)
    {
        return new(file, invocation.Span, (DiagnosticLevel)DiagnosticLevel.FatalCompilationError, (DiagnosticPhase)DiagnosticPhase.Analysis,
            (int)5006, (string)$"Could not find function with the name `{invocation.Expression}` and compatible arguments.");
    }

    public static Diagnostic AmbiguousFunctionCall(CodeFile file, IInvocationExpressionSyntax invocation)
    {
        return new(file, invocation.Span, (DiagnosticLevel)DiagnosticLevel.FatalCompilationError, (DiagnosticPhase)DiagnosticPhase.Analysis,
            (int)5007, (string)$"Function call `{invocation}` is ambiguous.");
    }

    public static Diagnostic CouldNotBindMethod(CodeFile file, IInvocationExpressionSyntax invocation)
    {
        return new(file, invocation.Span, (DiagnosticLevel)DiagnosticLevel.FatalCompilationError, (DiagnosticPhase)DiagnosticPhase.Analysis,
            (int)5008, (string)$"Could not find method with the name `{invocation.Expression}` and compatible arguments.");
    }

    public static Diagnostic AmbiguousMethodCall(CodeFile file, IInvocationExpressionSyntax invocation)
    {
        return new(file, invocation.Span, (DiagnosticLevel)DiagnosticLevel.FatalCompilationError, (DiagnosticPhase)DiagnosticPhase.Analysis,
            (int)5009, (string)$"Method call `{invocation}` is ambiguous.");
    }

    // TODO add check for this back
    public static Diagnostic UsingNonExistentNamespace(CodeFile file, TextSpan span, NamespaceName ns)
    {
        return new(file, span, (DiagnosticLevel)DiagnosticLevel.CompilationError, (DiagnosticPhase)DiagnosticPhase.Analysis,
            (int)5010, (string)$"Using directive refers to namespace `{ns}` which does not exist");
    }

    public static Diagnostic CouldNotBindParameterName(CodeFile file, TextSpan span)
    {
        return new(file, span, (DiagnosticLevel)DiagnosticLevel.FatalCompilationError, (DiagnosticPhase)DiagnosticPhase.Analysis,
            (int)5011, (string)$"Could not find parameter with the name `{file.Code[span]}`.");
    }
}
