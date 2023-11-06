using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;

/// <remarks><see cref="ErrorCodeRange"/> for the ranges of various kinds of error codes.</remarks>
public static class NameBindingError
{
    public static Diagnostic CouldNotBindName(CodeFile file, TextSpan span)
    {
        return new(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            5001, $"The name `{file.Code[span]}` is not defined in this scope.");
    }

    public static Diagnostic AmbiguousName(CodeFile file, TextSpan span)
    {
        return new(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            5002, $"The name `{file.Code[span]}` is ambiguous.");
    }

    public static Diagnostic CouldNotBindMember(CodeFile file, TextSpan span)
    {
        return new(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            5003, $"Could not find member `{file.Code[span]}` on object.");
    }

    public static Diagnostic CouldNotBindConstructor(CodeFile file, TextSpan span)
    {
        return new(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            5004, "Type doesn't have a constructor with this name and number of arguments.");
    }

    public static Diagnostic AmbiguousConstructorCall(CodeFile file, TextSpan span)
    {
        return new(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            5005, "Constructor call is ambiguous.");
    }

    public static Diagnostic CouldNotBindFunction(CodeFile file, IInvocationExpressionSyntax invocation)
    {
        return new(file, invocation.Span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            5006, $"Could not find function with the name `{invocation.Expression}` and compatible arguments.");
    }

    public static Diagnostic AmbiguousFunctionCall(CodeFile file, TextSpan span)
    {
        return new(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            5007, "Function call is ambiguous.");
    }

    public static Diagnostic CouldNotBindMethod(CodeFile file, TextSpan span, SimpleName methodName)
    {
        return new(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            5008, $"Could not find method with the name `{methodName}` and compatible arguments.");
    }

    public static Diagnostic AmbiguousMethodCall(CodeFile file, TextSpan span, SimpleName methodName)
    {
        return new(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            5009, $"Method call to `{methodName}` is ambiguous.");
    }

    // TODO add check for this back
    public static Diagnostic UsingNonExistentNamespace(CodeFile file, TextSpan span, NamespaceName ns)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Analysis,
            5010, $"Using directive refers to namespace `{ns}` which does not exist");
    }

    public static Diagnostic CouldNotBindParameterName(CodeFile file, TextSpan span)
    {
        return new(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            5011, $"Could not find parameter with the name `{file.Code[span]}`.");
    }
}
