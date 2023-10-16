using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types;
using UnaryOperator = Azoth.Tools.Bootstrap.Compiler.Core.Operators.UnaryOperator;

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

    public static Diagnostic CircularDefinition(CodeFile file, TextSpan span, ITypeDeclarationSyntax type)
    {
        return new(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            3006, $"Declaration of type `{type.ContainingNamespaceName}.{type.Name}` is part of a circular definition");
    }

    public static Diagnostic CannotImplicitlyConvert(CodeFile file, ISyntax expression, DataType ofType, DataType toType)
    {
        return new(file, expression.Span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            3007, $"Cannot convert expression `{file.Code[expression.Span]}` of type `{ofType.ToNonConstantType().ToSourceCodeString()}` to type `{toType.ToNonConstantType().ToSourceCodeString()}`");
    }

    public static Diagnostic MustBeInvocable(CodeFile file, IExpressionSyntax expression)
    {
        return new(file, expression.Span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            3008, $"Expression must be of callable type to be invoked `{file.Code[expression.Span]}`");
    }

    public static Diagnostic CannotMoveValue(CodeFile file, IMoveExpressionSyntax expression)
    {
        return new(file, expression.Span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            3009, $"Cannot move value `{file.Code[expression.Referent.Span]}`");
    }

    public static Diagnostic TypeDeclaredImmutable(CodeFile file, IExpressionSyntax expression)
    {
        return new(file, expression.Span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            3010, $"Type can't be made mutable because it was declared immutable `{file.Code[expression.Span]}`");
    }

    public static Diagnostic ReturnExpressionMustHaveValue(CodeFile file, in TextSpan span, DataType returnType)
    {
        return new(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            3012, $"The return expression must return a value of type `{returnType}`");
    }

    public static Diagnostic CannotReturnFromNeverFunction(CodeFile file, in TextSpan span)
    {
        return new(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            3013, "Functions that never return can't contain return statements");
    }

    public static Diagnostic CannotAssignFieldOfReadOnly(CodeFile file, in TextSpan span, ReferenceType referenceType)
    {
        return new(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            3014, $"Cannot assign into a field through a read only reference of type `{referenceType.ToSourceCodeString()}`");
    }

    public static Diagnostic CannotIdNonReferenceType(CodeFile file, in TextSpan span, DataType type)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Analysis,
            3015, $"Taking the `id` of the type `{type.ToSourceCodeString()}` is not supported, because it is not a reference type");
    }

    public static Diagnostic CannotFreezeValue(CodeFile file, IFreezeExpressionSyntax expression)
    {
        return new(file, expression.Span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            3016, $"Cannot freeze the value `{file.Code[expression.Referent.Span]}`");
    }

    public static Diagnostic CannotExplicitlyConvert(CodeFile file, ISyntax expression, DataType ofType, DataType toType)
    {
        return new(file, expression.Span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            3017, $"Cannot explicitly convert expression `{file.Code[expression.Span]}` of type `{ofType.ToNonConstantType().ToSourceCodeString()}` to type `{toType.ToNonConstantType().ToSourceCodeString()}`");
    }

    public static Diagnostic CannotApplyCapabilityToConstantType(CodeFile file, ISyntax expression, ReferenceCapability capability, DeclaredObjectType type)
    {
        return new(file, expression.Span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            3018, $"Cannot use `{capability.ToSourceString()}` on constant type `{type}`");
    }

    public static Diagnostic BaseTypeMustBeClass(CodeFile file, StandardTypeName className, ITypeNameSyntax typeName)
    {
        return new(file, typeName.Span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            3019, $"Class `{className}` cannot have base class `{typeName}` because it is not a class");
    }

    public static Diagnostic InvalidConstructorSelfParameterCapability(CodeFile file, IReferenceCapabilitySyntax syn)
    {
        var capability = syn.Declared.ToReferenceCapability();
        return new(file, syn.Span, DiagnosticLevel.CompilationError, DiagnosticPhase.Analysis,
            3020, $"Constructor self parameter cannot have reference capability `{capability.ToSourceString()}`. Only `mut` and read-only are allowed");
    }

    public static Diagnostic LentIdentity(CodeFile file, TextSpan span)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Analysis,
            3021, "Cannot have `lent` `id`");
    }

    public static Diagnostic SuperTypeMustBeClassOrTrait(CodeFile file, SimpleName typeName, ITypeNameSyntax superTypeName)
    {
        return new(file, superTypeName.Span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            3022, $"Type `{typeName}` cannot have super type `{superTypeName}` because it is not a trait or class");
    }
}
