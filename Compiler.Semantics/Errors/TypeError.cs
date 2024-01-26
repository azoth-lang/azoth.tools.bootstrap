using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using UnaryOperator = Azoth.Tools.Bootstrap.Compiler.Core.Operators.UnaryOperator;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;

/// <remarks><see cref="ErrorCodeRange"/> for the ranges of various kinds of error codes.</remarks>
public static class TypeError
{
    public static Diagnostic NotImplemented(CodeFile file, TextSpan span, string message)
    {
        return new(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            3000, message);
    }

    public static Diagnostic OperatorCannotBeAppliedToOperandsOfType(
        CodeFile file,
        TextSpan span,
        BinaryOperator @operator,
        DataType leftOperandType,
        DataType rightOperandType)
    {
        return new(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            3001, $"Operator `{@operator.ToSymbolString()}` cannot be applied to operands of type `{leftOperandType.ToNonConstantType().ToSourceCodeString()}` and `{rightOperandType.ToNonConstantType().ToSourceCodeString()}`.");
    }

    public static Diagnostic OperatorCannotBeAppliedToOperandOfType(
        CodeFile file,
        TextSpan span,
        UnaryOperator @operator,
        DataType operandType)
    {
        return new(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            3002, $"Operator `{@operator}` cannot be applied to operand of type `{operandType.ToSourceCodeString()}`.");
    }

    public static Diagnostic MustBeATypeExpression(CodeFile file, TextSpan span)
    {
        return new(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            3003, "Expression must be of type `type` (i.e. it must evaluate to a type)");
    }

    public static Diagnostic NameRefersToFunctionNotType(CodeFile file, TextSpan span, string name)
    {
        return new(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            3004, $"The name `{name}` refers to a function not a type.");
    }

    public static Diagnostic MustBeABoolExpression(CodeFile file, TextSpan span)
    {
        return new(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            3005, "Expression must be of type `bool`");
    }

    public static Diagnostic CannotImplicitlyConvert(CodeFile file, ISyntax expression, DataType ofType, DataType toType)
    {
        return new(file, expression.Span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            3006, $"Cannot convert expression `{file.Code[expression.Span]}` of type `{ofType.ToNonConstantType().ToSourceCodeString()}` to type `{toType.ToNonConstantType().ToSourceCodeString()}`");
    }

    public static Diagnostic MustBeInvocable(CodeFile file, IExpressionSyntax expression)
    {
        return new(file, expression.Span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            3007, $"Expression must be of callable type to be invoked `{file.Code[expression.Span]}`");
    }

    public static Diagnostic MustReturnCorrectType(CodeFile file, in TextSpan span, DataType returnType)
    {
        return new(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            3008, $"The return expression must return a value of type `{returnType}`");
    }

    public static Diagnostic CannotReturnFromNeverFunction(CodeFile file, in TextSpan span)
    {
        return new(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            3009, "Functions that never return can't contain return statements");
    }

    public static Diagnostic CannotAssignFieldOfReadOnly(CodeFile file, in TextSpan span, ReferenceType referenceType)
    {
        return new(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            3010, $"Cannot assign into a field through a read only reference of type `{referenceType.ToSourceCodeString()}`");
    }

    public static Diagnostic CannotIdNonReferenceType(CodeFile file, in TextSpan span, DataType type)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Analysis,
            3011, $"Taking the `id` of the type `{type.ToSourceCodeString()}` is not supported, because it is not a reference type");
    }

    public static Diagnostic CannotExplicitlyConvert(CodeFile file, ISyntax expression, DataType ofType, DataType toType)
    {
        return new(file, expression.Span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            3012, $"Cannot explicitly convert expression `{file.Code[expression.Span]}` of type `{ofType.ToNonConstantType().ToSourceCodeString()}` to type `{toType.ToNonConstantType().ToSourceCodeString()}`");
    }

    public static Diagnostic CannotApplyCapabilityToConstantType(CodeFile file, ISyntax expression, ReferenceCapability capability, DeclaredObjectType type)
    {
        return new(file, expression.Span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            3013, $"Cannot use `{capability.ToSourceString()}` on constant type `{type}`");
    }

    public static Diagnostic InvalidConstructorSelfParameterCapability(CodeFile file, IReferenceCapabilitySyntax syn)
    {
        var capability = syn.Declared.ToReferenceCapability();
        return new(file, syn.Span, DiagnosticLevel.CompilationError, DiagnosticPhase.Analysis,
            3014, $"Constructor self parameter cannot have reference capability `{capability.ToSourceString()}`. Only `mut` and read-only are allowed");
    }

    public static Diagnostic LentIdentity(CodeFile file, TextSpan span)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Analysis,
            3015, "Cannot have `lent id`");
    }

    public static Diagnostic OptionalPatternOnNonOptionalType(CodeFile file, IOptionalPatternSyntax pattern, DataType type)
    {
        return new(file, pattern.Span, DiagnosticLevel.CompilationError, DiagnosticPhase.Analysis,
            3016, $"Optional pattern `{pattern}` cannot be applied to value of non-optional type {type.ToSourceCodeString()}");
    }

    public static Diagnostic ConstClassSelfParameterCannotHaveCapability(CodeFile file, ISelfParameterSyntax self)
    {
        return new(file, self.Span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            3017, $"Self parameter `{self}` must be `const` or `id` because it is in a `const` class.");
    }

    public static Diagnostic CannotAwaitType(CodeFile file, TextSpan span, DataType type)
    {
        return new(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            3018, $"Cannot await non-awaitable type `{type.ToSourceCodeString()}`.");
    }

    public static Diagnostic CapabilityAppliedToTypeParameter(CodeFile file, ITypeNameSyntax typeSyntax)
    {
        return new(file, typeSyntax.Span, DiagnosticLevel.CompilationError, DiagnosticPhase.Analysis,
            3019, $"Reference capabilities cannot be applied to type parameters `{typeSyntax.ToString()}`.");
    }
}
