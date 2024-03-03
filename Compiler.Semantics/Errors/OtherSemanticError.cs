using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;

/// <remarks><see cref="ErrorCodeRange"/> for the ranges of various kinds of error codes.</remarks>
public static class OtherSemanticError
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

    public static Diagnostic MayAlreadyBeAssigned(CodeFile file, TextSpan span, SimpleName name)
    {
        return new(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            6004, $"Variable or field `{name}` declared with `let` may already be assigned");
    }

    public static Diagnostic VariableMayNotHaveBeenAssigned(CodeFile file, TextSpan span, SimpleName name)
    {
        return new(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            6005, $"Variable `{name}` may not have been assigned before use");
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

    public static Diagnostic ResultStatementInBody(CodeFile file, TextSpan span)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Analysis,
            6009, "Result statement (i.e. `=> ...;`) cannot be used in the body of a function, or method, etc.");
    }

    public static Diagnostic StatementAfterResult(CodeFile file, TextSpan span)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Analysis,
            6010, "Statement after result statement is unreachable");
    }

    public static Diagnostic AbstractMethodNotInAbstractClass(CodeFile file, TextSpan span, SimpleName name)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Analysis,
            6011, $"Abstract method `{name}` declared in a concrete class");
    }

    public static Diagnostic CannotAssignImmutableField(CodeFile file, TextSpan span, SimpleName name)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Analysis,
            6012, $"Field `{name}` declared with `let` cannot be assigned");
    }

    public static Diagnostic LentConstructorOrInitializerSelf(CodeFile file, ISelfParameterSyntax self)
    {
        return new(file, self.Span, DiagnosticLevel.CompilationError, DiagnosticPhase.Analysis,
            6013, "Constructor or initializer `self` parameter cannot be `lent`");
    }

    public static Diagnostic CircularDefinition(CodeFile file, TextSpan span, ITypeDeclarationSyntax type)
    {
        return new(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            6014, $"Declaration of type `{type.ContainingNamespaceName}.{type.Name}` is part of a circular definition");
    }

    public static Diagnostic BaseTypeMustBeClass(CodeFile file, StandardTypeName className, ITypeNameSyntax baseTypeName)
    {
        return new(file, baseTypeName.Span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            6015, $"Class `{className}` cannot have base type `{baseTypeName}` because it is not a class");
    }

    public static Diagnostic SupertypeMustBeClassOrTrait(CodeFile file, StandardTypeName typeName, ITypeNameSyntax superTypeName)
    {
        return new(file, superTypeName.Span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            6016, $"Type `{typeName}` cannot have super type `{superTypeName}` because it is not a trait or class");
    }

    public static Diagnostic CannotConstructAbstractType(CodeFile file, ITypeNameSyntax typeName)
    {
        return new(file, typeName.Span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            6017, $"Type `{typeName}` cannot be constructed because it is abstract");
    }

    public static Diagnostic ForeachNoIterateOrNextMethod(CodeFile file, IExpressionSyntax inExpression, DataType type)
    {
        return new(file, inExpression.Span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            6018, $"`foreach` cannot operate on value of type `{type.ToILString()}` because it does not have an `iterate()` nor `next()` method.");
    }

    public static Diagnostic ForeachNoNextMethod(CodeFile file, IExpressionSyntax inExpression, DataType type)
    {
        return new(file, inExpression.Span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis, 6018,
            $"`foreach` cannot operate on value of type `{type.ToILString()}` because its iterator does not have a `next()` method.");
    }
}
