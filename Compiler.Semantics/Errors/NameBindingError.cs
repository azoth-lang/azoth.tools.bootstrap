using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Syntax;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;

/// <remarks><see cref="ErrorCodeRange"/> for the ranges of various kinds of error codes.</remarks>
public static class NameBindingError
{
    public static Diagnostic NotImplemented(CodeFile file, TextSpan span, string message)
    {
        return new(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            5000, message);
    }

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

    public static Diagnostic CouldNotBindInitializer(CodeFile file, TextSpan span)
    {
        return new(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            5004, "Type doesn't have a initializer with this name and matching arguments.");
    }

    public static Diagnostic AmbiguousInitializerCall(CodeFile file, TextSpan span)
    {
        return new(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            5005, "Initializer call is ambiguous.");
    }

    public static Diagnostic CouldNotBindFunction(CodeFile file, IInvocationExpressionSyntax invocation)
    {
        return new(file, invocation.Span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            5006, $"Could not find function with the name `{invocation.Expression}` and compatible arguments.");
    }

    public static Diagnostic AmbiguousFunctionCall(CodeFile file, IInvocationExpressionSyntax invocation)
    {
        return new(file, invocation.Span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            5007, $"Function call `{invocation}` is ambiguous.");
    }

    // TODO add check for this back
    public static Diagnostic ImportNonExistentNamespace(CodeFile file, TextSpan span, NamespaceName ns)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Analysis,
            5010, $"Import directive refers to namespace `{ns}` which does not exist");
    }

    public static Diagnostic CouldNotBindFunctionName(CodeFile file, INameExpressionSyntax groupName)
    {
        return new(file, groupName.Span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            5011, $"Could not find function with the name `{groupName}` and compatible arguments.");
    }

    public static Diagnostic AmbiguousFunctionName(CodeFile file, INameExpressionSyntax groupName)
    {
        return new(file, groupName.Span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            5012, $"Function name `{groupName}` is ambiguous.");
    }

    public static Diagnostic CouldNotBindMethodName(CodeFile file, IMemberAccessExpressionSyntax groupName)
    {
        return new(file, groupName.Span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            5013, $"Could not find method with the name `{groupName}` and compatible arguments.");
    }

    public static Diagnostic AmbiguousMethodName(CodeFile file, IMemberAccessExpressionSyntax groupName)
    {
        return new(file, groupName.Span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            5014, $"Method name `{groupName}` is ambiguous.");
    }
}
