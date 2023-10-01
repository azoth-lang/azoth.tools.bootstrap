using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;

/// <summary>
/// Error Code Ranges:
/// 1001-1999: Lexical Errors
/// 2001-2999: Parsing Errors
/// 3001-3999: Type Errors
/// 4001-4999: Borrow Checking Errors
/// 5001-5999: Name Binding Errors
/// 6001-6999: Other Semantic Errors
/// </summary>
public static class SemanticError
{
    public static Diagnostic CantRebindMutableBinding(CodeFile file, TextSpan span)
    {
        return new(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            6001, "Variable binding can't rebind previous mutable variable binding");
    }

    public static Diagnostic CantRebindAsMutableBinding(CodeFile file, TextSpan span)
    {
        return new(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            6002, "Mutable variable binding can't rebind previous variable binding");
    }

    public static Diagnostic CantShadow(CodeFile file, TextSpan bindingSpan, TextSpan useSpan)
    {
        // TODO that use span needs converted to a line and column
        return new(file, bindingSpan, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            6003, $"Variable binding can't shadow. Shadowed binding used at {useSpan}");
    }

    public static Diagnostic MayAlreadyBeAssigned(CodeFile file, TextSpan span, Name name)
    {
        return new(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            6004, $"Variable or field `{name}` declared with `let` may already be assigned");
    }

    public static Diagnostic VariableMayNotHaveBeenAssigned(CodeFile file, TextSpan span, Name name)
    {
        return new(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            6005, $"Variable `{name}` may not have been assigned before use");
    }

    public static Diagnostic UseOfPossiblyMovedValue(CodeFile file, TextSpan span)
    {
        return new(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            6006, "Use of possibly moved value.");
    }

    public static Diagnostic ImplicitSelfOutsideMethod(CodeFile file, TextSpan span)
    {
        return new(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            6007, "Can't use implicit self reference outside of a method or constructor");
    }

    public static Diagnostic SelfOutsideMethod(CodeFile file, TextSpan span)
    {
        return new(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            6008, "Can't use `self` outside of a method or constructor");
    }

    public static Diagnostic NoStringTypeDefined(CodeFile file)
    {
        return new(file, new TextSpan(0, 0), DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            6009, "Could not find a `String` type. A `String` type must be defined in the global namespace");
    }

    public static Diagnostic ResultStatementInBody(CodeFile file, TextSpan span)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Analysis,
            6010, "Result statement (i.e. `=> ...;`) cannot be used in the body of a function, or method, etc.");
    }

    public static Diagnostic StatementAfterResult(CodeFile file, TextSpan span)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Analysis,
            6011, "Statement after result statement is unreachable");
    }

    public static Diagnostic AbstractMethodNotInAbstractClass(CodeFile file, TextSpan span, Name name)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Analysis,
            6012, $"Abstract method `{name}` declared in a concrete class");
    }

    public static Diagnostic CannotAssignImmutableField(CodeFile file, TextSpan span, Name name)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Analysis,
            6013, $"Field `{name}` declared with `let` cannot be assigned");
    }
}
