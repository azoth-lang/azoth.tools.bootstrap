using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;
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
            3001, $"Operator `{@operator.ToSymbolString()}` cannot be applied to operands of type `{leftOperandType.ToNonConstValueType().ToSourceCodeString()}` and `{rightOperandType.ToNonConstValueType().ToSourceCodeString()}`.");
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

    public static Diagnostic CannotImplicitlyConvert(CodeFile file, ICodeSyntax expression, DataType ofType, DataType toType)
    {
        return new(file, expression.Span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            3006, $"Cannot convert expression `{file.Code[expression.Span]}` of type `{ofType.ToNonConstValueType().ToSourceCodeString()}` to type `{toType.ToNonConstValueType().ToSourceCodeString()}`");
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

    public static Diagnostic CannotAssignFieldOfReadOnly(CodeFile file, in TextSpan span, CapabilityType contextType)
    {
        return new(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            3010, $"Cannot assign into a field through a read only `{contextType.ToSourceCodeString()}`");
    }

    public static Diagnostic CannotIdNonReferenceType(CodeFile file, in TextSpan span, DataType type)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Analysis,
            3011, $"Taking the `id` of the type `{type.ToSourceCodeString()}` is not supported, because it is not a reference type");
    }

    public static Diagnostic CannotExplicitlyConvert(CodeFile file, ICodeSyntax expression, DataType ofType, DataType toType)
    {
        return new(file, expression.Span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            3012, $"Cannot explicitly convert expression `{file.Code[expression.Span]}` of type `{ofType.ToNonConstValueType().ToSourceCodeString()}` to type `{toType.ToNonConstValueType().ToSourceCodeString()}`");
    }

    public static Diagnostic CannotApplyCapabilityToConstantType(CodeFile file, ICodeSyntax expression, Capability capability, DeclaredType type)
    {
        return new(file, expression.Span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            3013, $"Cannot use `{capability.ToSourceCodeString()}` on constant type `{type}`");
    }

    public static Diagnostic InvalidConstructorSelfParameterCapability(CodeFile file, ICapabilitySyntax syn)
    {
        var capability = syn.Declared.ToCapability();
        return new(file, syn.Span, DiagnosticLevel.CompilationError, DiagnosticPhase.Analysis,
            3014, $"Constructor self parameter cannot have reference capability `{capability.ToSourceCodeString()}`. Only `mut` and read-only are allowed");
    }

    public static Diagnostic TypeCannotBeLent(CodeFile file, TextSpan span, Pseudotype type)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Analysis,
            3015, $"Cannot apply `lent` to `{type.ToSourceCodeString()}` because it is a fully `const` or `id` type");
    }

    public static Diagnostic OptionalPatternOnNonOptionalType(CodeFile file, IOptionalPatternSyntax pattern, DataType type)
    {
        return new(file, pattern.Span, DiagnosticLevel.CompilationError, DiagnosticPhase.Analysis,
            3016, $"Optional pattern `{pattern}` cannot be applied to value of non-optional type `{type.ToSourceCodeString()}`");
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

    public static Diagnostic CapabilityAppliedToTypeParameter(CodeFile file, ITypeSyntax typeSyntax)
    {
        return new(file, typeSyntax.Span, DiagnosticLevel.CompilationError, DiagnosticPhase.Analysis,
            3019, $"Reference capabilities cannot be applied to type parameters `{typeSyntax.ToString()}`.");
    }

    public static Diagnostic CapabilityViewpointNotAppliedToTypeParameter(CodeFile file, ICapabilityViewpointTypeSyntax typeSyntax)
    {
        return new(file, typeSyntax.Span, DiagnosticLevel.CompilationError, DiagnosticPhase.Analysis,
            3020, $"Reference capabilities viewpoint must be applied to type parameter `{typeSyntax.ToString()}`.");
    }

    public static Diagnostic SelfViewpointNotAppliedToTypeParameter(CodeFile file, ISelfViewpointTypeSyntax typeSyntax)
    {
        return new(file, typeSyntax.Span, DiagnosticLevel.CompilationError, DiagnosticPhase.Analysis,
            3021, $"Self viewpoint must be applied to type parameter `{typeSyntax.ToString()}`.");
    }

    public static Diagnostic SelfViewpointNotAvailable(CodeFile file, ISelfViewpointTypeSyntax typeSyntax)
    {
        return new(file, typeSyntax.Span, DiagnosticLevel.CompilationError, DiagnosticPhase.Analysis,
            3022, $"Self viewpoint not available `{typeSyntax.ToString()}`.");
    }

    public static Diagnostic CannotAccessMutableBindingFieldOfIdentityReference(CodeFile file, IMemberAccessExpressionSyntax exp, Pseudotype contextType)
    {
        return new(file, exp.Span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            3023, $"Cannot access `var` field `{exp.MemberName}` from type `{contextType.ToSourceCodeString()}`.");
    }

    public static Diagnostic CapabilityAppliedToEmptyType(CodeFile file, ICapabilityTypeSyntax typeSyntax)
    {
        return new(file, typeSyntax.Span, DiagnosticLevel.CompilationError, DiagnosticPhase.Analysis,
            3024, $"Reference capabilities cannot be applied to empty types `{typeSyntax.ToString()}`.");
    }

    public static Diagnostic SupertypeMustBeOutputSafe(CodeFile file, IStandardTypeNameSyntax typeSyntax)
    {
        return new(file, typeSyntax.Span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            3025, $"Supertype `{typeSyntax.ToString()}` is not output safe.");
    }

    public static Diagnostic ParameterMustBeInputSafe(CodeFile file, IParameterSyntax parameterSyntax, DataType type)
    {
        return new(file, parameterSyntax.Span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            3026, $"Parameter `{parameterSyntax.ToString()}` with type `{type.ToSourceCodeString()}` is not input safe.");
    }

    public static Diagnostic ReturnTypeMustBeOutputSafe(CodeFile file, ITypeSyntax typeSyntax, DataType type)
    {
        return new(file, typeSyntax.Span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            3027, $"Return type `{type.ToSourceCodeString()}` is not output safe.");
    }

    public static Diagnostic VarFieldMustBeInputAndOutputSafe(CodeFile file, IFieldDefinitionSyntax fieldSyntax, DataType type)
    {
        return new(file, fieldSyntax.Type.Span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            3028, $"The field `{fieldSyntax.Name}` declared with `var` of type `{type.ToSourceCodeString()}` is not input and output safe.");
    }

    public static Diagnostic LetFieldMustBeOutputSafe(CodeFile file, IFieldDefinitionSyntax fieldSyntax, DataType type)
    {
        return new(file, fieldSyntax.Type.Span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            3029, $"The field `{fieldSyntax.Name}` declared with `let` of type `{type.ToSourceCodeString()}` is not output safe.");
    }

    public static Diagnostic FieldMustMaintainIndependence(CodeFile file, IFieldDefinitionSyntax fieldSyntax, DataType type)
    {
        return new(file, fieldSyntax.Type.Span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            3030, $"The field `{fieldSyntax.Name}` of type `{type.ToSourceCodeString()}` does not maintain the independence of the type parameters.");
    }

    public static Diagnostic TypeParameterCannotBeUsedHere(CodeFile file, ITypeNameSyntax typeSyntax)
    {
        return new(file, typeSyntax.Span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            3031, $"Type parameter `{typeSyntax}` cannot be used here.");
    }

    public static Diagnostic SpecialTypeCannotBeUsedHere(CodeFile file, ITypeNameSyntax typeSyntax)
    {
        return new(file, typeSyntax.Span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            3032, $"Special type `{typeSyntax}` cannot be used here.");
    }

    public static Diagnostic CapabilityNotCompatibleWithConstraint(CodeFile file, ICodeSyntax typeSyntax, GenericParameter parameter, DataType arg)
    {
        return new(file, typeSyntax.Span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            3033, $"In `{typeSyntax}` the capability of `{arg.ToSourceCodeString()}` is not compatible with the capability constraint on `{parameter}`.");
    }

    public static Diagnostic SupertypeMustMaintainIndependence(CodeFile file, IStandardTypeNameSyntax typeSyntax)
    {
        return new(file, typeSyntax.Span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            3034, $"Supertype `{typeSyntax.ToString()}` does not maintain independence.");
    }

    public static Diagnostic NoFunctionInGroupMatchesExpectedType(CodeFile file, INameExpressionSyntax nameSyntax, FunctionType functionType)
    {
        return new(file, nameSyntax.Span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            3035, $"No function in group `{nameSyntax.ToString()}` matches the expected type `{functionType.ToSourceCodeString()}`.");
    }

    public static Diagnostic AmbiguousFunctionGroup(CodeFile file, INameExpressionSyntax nameSyntax, DataType functionType)
    {
        return new(file, nameSyntax.Span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            3036, $"Function group `{nameSyntax.ToString()}` has multiple functions that match the expected type `{functionType.ToSourceCodeString()}`.");
    }
}
