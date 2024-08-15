using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;

/// <remarks><see cref="ErrorCodeRange"/> for the ranges of various kinds of error codes.</remarks>
public static class OtherSemanticError
{
    public static Diagnostic CantRebindMutableBinding(CodeFile file, TextSpan span)
    {
        return new(file, span, (DiagnosticLevel)DiagnosticLevel.CompilationError, (DiagnosticPhase)DiagnosticPhase.Analysis,
            (int)6001, (string)"Variable binding can't rebind previous mutable variable binding");
    }

    public static Diagnostic CantRebindAsMutableBinding(CodeFile file, TextSpan span)
    {
        return new(file, span, (DiagnosticLevel)DiagnosticLevel.CompilationError, (DiagnosticPhase)DiagnosticPhase.Analysis,
            (int)6002, (string)"Mutable variable binding can't rebind previous variable binding");
    }

    public static Diagnostic CantShadow(CodeFile file, TextSpan bindingSpan, TextSpan useSpan)
    {
        // TODO that use span needs converted to a line and column
        return new(file, bindingSpan, (DiagnosticLevel)DiagnosticLevel.CompilationError, (DiagnosticPhase)DiagnosticPhase.Analysis,
            (int)6003, (string)$"Variable binding can't shadow. Shadowed binding used at {useSpan}");
    }

    public static Diagnostic MayAlreadyBeAssigned(CodeFile file, TextSpan span, IdentifierName name)
    {
        return new(file, span, (DiagnosticLevel)DiagnosticLevel.FatalCompilationError, (DiagnosticPhase)DiagnosticPhase.Analysis,
            (int)6004, (string)$"Variable or field `{name}` declared with `let` may already be assigned");
    }

    public static Diagnostic VariableMayNotHaveBeenAssigned(CodeFile file, TextSpan span, IdentifierName name)
    {
        return new(file, span, (DiagnosticLevel)DiagnosticLevel.FatalCompilationError, (DiagnosticPhase)DiagnosticPhase.Analysis,
            (int)6005, (string)$"Variable `{name}` may not have been assigned before use");
    }

    public static Diagnostic ImplicitSelfOutsideMethod(CodeFile file, TextSpan span)
    {
        return new(file, span, (DiagnosticLevel)DiagnosticLevel.FatalCompilationError, (DiagnosticPhase)DiagnosticPhase.Analysis,
            (int)6007, (string)"Can't use implicit self reference outside of a method or constructor");
    }

    public static Diagnostic SelfOutsideMethod(CodeFile file, TextSpan span)
    {
        return new(file, span, (DiagnosticLevel)DiagnosticLevel.FatalCompilationError, (DiagnosticPhase)DiagnosticPhase.Analysis,
            (int)6008, (string)"Can't use `self` outside of a method or constructor");
    }

    // TODO error reported by parser and statement dropped, perhaps should be a semantic error
    //public static Diagnostic ResultStatementInBody(CodeFile file, TextSpan span)
    //{
    //    return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Analysis,
    //        6009, "Result statement (i.e. `=> ...;`) cannot be used in the body of a function, or method, etc.");
    //}

    public static Diagnostic StatementAfterResult(CodeFile file, TextSpan span)
    {
        return new(file, span, (DiagnosticLevel)DiagnosticLevel.CompilationError, (DiagnosticPhase)DiagnosticPhase.Analysis,
            (int)6010, (string)"Statement after result statement is unreachable");
    }

    public static Diagnostic AbstractMethodNotInAbstractClass(CodeFile file, TextSpan span, IdentifierName name)
    {
        return new(file, span, (DiagnosticLevel)DiagnosticLevel.CompilationError, (DiagnosticPhase)DiagnosticPhase.Analysis,
            (int)6011, (string)$"Abstract method `{name}` declared in a concrete class");
    }

    public static Diagnostic CannotAssignImmutableField(CodeFile file, TextSpan span, IdentifierName name)
    {
        return new(file, span, (DiagnosticLevel)DiagnosticLevel.CompilationError, (DiagnosticPhase)DiagnosticPhase.Analysis,
            (int)6012, (string)$"Field `{name}` declared with `let` cannot be assigned");
    }

    public static Diagnostic LentConstructorOrInitializerSelf(CodeFile file, ISelfParameterSyntax self)
    {
        return new(file, self.Span, (DiagnosticLevel)DiagnosticLevel.CompilationError, (DiagnosticPhase)DiagnosticPhase.Analysis,
            (int)6013, (string)"Constructor or initializer `self` parameter cannot be `lent`");
    }

    public static Diagnostic CircularDefinition(CodeFile file, TextSpan span, ITypeDefinitionSyntax type)
    {
        return new(file, span, (DiagnosticLevel)DiagnosticLevel.FatalCompilationError, (DiagnosticPhase)DiagnosticPhase.Analysis,
            (int)6014, (string)$"Declaration of type `{type.ContainingNamespaceName}.{type.Name}` is part of a circular definition");
    }

    public static Diagnostic BaseTypeMustBeClass(CodeFile file, StandardName className, IStandardTypeNameSyntax baseTypeName)
    {
        return new(file, baseTypeName.Span, (DiagnosticLevel)DiagnosticLevel.FatalCompilationError, (DiagnosticPhase)DiagnosticPhase.Analysis,
            (int)6015, (string)$"Class `{className}` cannot have base type `{baseTypeName}` because it is not a class");
    }

    public static Diagnostic SupertypeMustBeClassOrTrait(CodeFile file, StandardName typeName, IStandardTypeNameSyntax superTypeName)
    {
        return new(file, superTypeName.Span, (DiagnosticLevel)DiagnosticLevel.FatalCompilationError, (DiagnosticPhase)DiagnosticPhase.Analysis,
            (int)6016, (string)$"Type `{typeName}` cannot have super type `{superTypeName}` because it is not a trait or class");
    }

    public static Diagnostic CannotConstructAbstractType(CodeFile file, ITypeNameSyntax typeName)
    {
        return new(file, typeName.Span, (DiagnosticLevel)DiagnosticLevel.FatalCompilationError, (DiagnosticPhase)DiagnosticPhase.Analysis,
            (int)6017, (string)$"Type `{typeName}` cannot be constructed because it is abstract");
    }

    public static Diagnostic ForeachNoIterateOrNextMethod(CodeFile file, IExpressionSyntax inExpression, DataType type)
    {
        return new(file, inExpression.Span, (DiagnosticLevel)DiagnosticLevel.FatalCompilationError, (DiagnosticPhase)DiagnosticPhase.Analysis,
            (int)6018, (string)$"`foreach` cannot operate on value of type `{type.ToILString()}` because it does not have an `iterate()` nor `next()` method.");
    }

    public static Diagnostic ForeachNoNextMethod(CodeFile file, IExpressionSyntax inExpression, DataType type)
    {
        return new(file, inExpression.Span, (DiagnosticLevel)DiagnosticLevel.FatalCompilationError, (DiagnosticPhase)DiagnosticPhase.Analysis,
            (int)6018, (string)$"`foreach` cannot operate on value of type `{type.ToILString()}` because its iterator does not have a `next()` method.");
    }

    public static Diagnostic CircularDefinitionInSupertype(CodeFile file, IStandardTypeNameSyntax supertypeName)
    {
        return new(file, supertypeName.Span, (DiagnosticLevel)DiagnosticLevel.FatalCompilationError, (DiagnosticPhase)DiagnosticPhase.Analysis,
            (int)6019, (string)$"Circular definition found when trying to evaluate supertype `{supertypeName}`.");
    }

    public static Diagnostic CannotReturnFromFieldInitializer(CodeFile file, IReturnExpressionSyntax returnExpression)
    {
        return new(file, returnExpression.Span, (DiagnosticLevel)DiagnosticLevel.FatalCompilationError, (DiagnosticPhase)DiagnosticPhase.Analysis,
            (int)6020, (string)"Cannot return from a field initializer.");
    }
}
