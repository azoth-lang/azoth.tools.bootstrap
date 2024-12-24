using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;
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
        IMaybeType leftOperandType,
        IMaybeType rightOperandType)
    {
        return new(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            3001, $"Operator `{@operator.ToSymbolString()}` cannot be applied to operands of type `{leftOperandType.ToSourceCodeString()}` and `{rightOperandType.ToSourceCodeString()}`.");
    }

    public static Diagnostic OperatorCannotBeAppliedToOperandOfType(
        CodeFile file,
        TextSpan span,
        UnaryOperator @operator,
        IMaybeType operandType)
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

    public static Diagnostic CannotImplicitlyConvert(CodeFile file, ICodeSyntax expression, IMaybeType ofType, Type toType)
    {
        return new(file, expression.Span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            3006, $"Cannot convert expression `{file.Code[expression.Span]}` of type `{ofType.ToSourceCodeString()}` to type `{toType.ToSourceCodeString()}`");
    }

    public static Diagnostic MustBeInvocable(CodeFile file, IExpressionSyntax expression)
    {
        return new(file, expression.Span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            3007, $"Expression must be of callable type to be invoked `{file.Code[expression.Span]}`");
    }

    public static Diagnostic MustReturnCorrectType(CodeFile file, in TextSpan span, IMaybeType returnType)
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

    public static Diagnostic CannotExplicitlyConvert(CodeFile file, ICodeSyntax expression, IMaybeType ofType, IMaybeType toType)
    {
        return new(file, expression.Span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            3012, $"Cannot explicitly convert expression `{file.Code[expression.Span]}` of type `{ofType.ToNonLiteral().ToSourceCodeString()}` to type `{toType.ToNonLiteral().ToSourceCodeString()}`");
    }

    public static Diagnostic CannotApplyCapabilityToConstantType(CodeFile file, ICodeSyntax expression, Capability capability, TypeConstructor type)
    {
        return new(file, expression.Span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            3013, $"Cannot use `{capability.ToSourceCodeString()}` on constant type `{type}`");
    }

    public static Diagnostic InvalidConstructorSelfParameterCapability(CodeFile file, ICapabilitySyntax syn)
    {
        return new(file, syn.Span, DiagnosticLevel.CompilationError, DiagnosticPhase.Analysis,
            3014, $"Constructor self parameter cannot have reference capability `{syn.Capability.ToSourceCodeString()}`. Only `mut` and read-only are allowed");
    }

    public static Diagnostic TypeCannotBeLent(CodeFile file, TextSpan span, IMaybeType type)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Analysis,
            3015, $"Cannot apply `lent` to `{type.ToSourceCodeString()}` because it is a fully `const` or `id` type");
    }

    public static Diagnostic OptionalPatternOnNonOptionalType(CodeFile file, IOptionalPatternSyntax pattern, IMaybeType type)
    {
        return new(file, pattern.Span, DiagnosticLevel.CompilationError, DiagnosticPhase.Analysis,
            3016, $"Optional pattern `{pattern}` cannot be applied to value of non-optional type `{type.ToSourceCodeString()}`");
    }

    public static Diagnostic ConstClassSelfParameterCannotHaveCapability(CodeFile file, ISelfParameterSyntax self)
    {
        return new(file, self.Span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            3017, $"Self parameter `{self}` must be `const` or `id` because it is in a `const` class.");
    }

    public static Diagnostic CannotAwaitType(CodeFile file, TextSpan span, IMaybeType type)
    {
        return new(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            3018, $"Cannot await non-awaitable type `{type.ToSourceCodeString()}`.");
    }

    public static Diagnostic CapabilityAppliedToTypeParameter(CodeFile file, ITypeSyntax typeSyntax)
    {
        return new(file, typeSyntax.Span, DiagnosticLevel.CompilationError, DiagnosticPhase.Analysis,
            3019, $"Reference capabilities cannot be applied to type parameters `{typeSyntax}`.");
    }

    public static Diagnostic CapabilityViewpointNotAppliedToTypeParameter(CodeFile file, ICapabilityViewpointTypeSyntax typeSyntax)
    {
        return new(file, typeSyntax.Span, DiagnosticLevel.CompilationError, DiagnosticPhase.Analysis,
            3020, $"Reference capabilities viewpoint must be applied to type parameter `{typeSyntax}`.");
    }

    public static Diagnostic SelfViewpointNotAppliedToTypeParameter(CodeFile file, ISelfViewpointTypeSyntax typeSyntax)
    {
        return new(file, typeSyntax.Span, DiagnosticLevel.CompilationError, DiagnosticPhase.Analysis,
            3021, $"Self viewpoint must be applied to type parameter `{typeSyntax}`.");
    }

    public static Diagnostic SelfViewpointNotAvailable(CodeFile file, ISelfViewpointTypeSyntax typeSyntax)
    {
        return new(file, typeSyntax.Span, DiagnosticLevel.CompilationError, DiagnosticPhase.Analysis,
            3022, $"Self viewpoint not available `{typeSyntax}`.");
    }

    public static Diagnostic CannotAccessMutableBindingFieldOfIdentityReference(CodeFile file, IMemberAccessExpressionSyntax exp, IMaybeType contextType)
    {
        return new(file, exp.Span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            3023, $"Cannot access `var` field `{exp.MemberName}` from type `{contextType.ToSourceCodeString()}`.");
    }

    public static Diagnostic CapabilityAppliedToEmptyType(CodeFile file, ICapabilityTypeSyntax typeSyntax)
    {
        return new(file, typeSyntax.Span, DiagnosticLevel.CompilationError, DiagnosticPhase.Analysis,
            3024, $"Reference capabilities cannot be applied to empty types `{typeSyntax}`.");
    }

    public static Diagnostic SupertypeMustBeOutputSafe(CodeFile file, IStandardTypeNameSyntax typeSyntax)
    {
        return new(file, typeSyntax.Span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            3025, $"Supertype `{typeSyntax}` is not output safe.");
    }

    public static Diagnostic ParameterMustBeInputSafe(CodeFile file, IParameterSyntax parameterSyntax, IMaybeType type)
    {
        return new(file, parameterSyntax.Span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            3026, $"Parameter `{parameterSyntax}` with type `{type.ToSourceCodeString()}` is not input safe.");
    }

    public static Diagnostic ReturnTypeMustBeOutputSafe(CodeFile file, ITypeSyntax typeSyntax, IMaybeType type)
    {
        return new(file, typeSyntax.Span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            3027, $"Return type `{type.ToSourceCodeString()}` is not output safe.");
    }

    public static Diagnostic VarFieldMustBeInputAndOutputSafe(CodeFile file, IFieldDefinitionSyntax fieldSyntax, IMaybeType type)
    {
        return new(file, fieldSyntax.Type.Span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            3028, $"The field `{fieldSyntax.Name}` declared with `var` of type `{type.ToSourceCodeString()}` is not input and output safe.");
    }

    public static Diagnostic LetFieldMustBeOutputSafe(CodeFile file, IFieldDefinitionSyntax fieldSyntax, IMaybeType type)
    {
        return new(file, fieldSyntax.Type.Span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            3029, $"The field `{fieldSyntax.Name}` declared with `let` of type `{type.ToSourceCodeString()}` is not output safe.");
    }

    public static Diagnostic FieldMustMaintainIndependence(CodeFile file, IFieldDefinitionSyntax fieldSyntax, IMaybeType type)
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

    public static Diagnostic CapabilityNotCompatibleWithConstraint(CodeFile file, ICodeSyntax typeSyntax, TypeConstructorParameter parameter, Type arg)
    {
        return new(file, typeSyntax.Span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            3033, $"In `{typeSyntax}` the capability of `{arg.ToSourceCodeString()}` is not compatible with the capability constraint on `{parameter}`.");
    }

    public static Diagnostic SupertypeMustMaintainIndependence(CodeFile file, IStandardTypeNameSyntax typeSyntax)
    {
        return new(file, typeSyntax.Span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            3034, $"Supertype `{typeSyntax}` does not maintain independence.");
    }

    public static Diagnostic AmbiguousFunctionGroup(CodeFile file, INameExpressionSyntax nameSyntax, IMaybeFunctionType functionType)
    {
        return new(file, nameSyntax.Span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            3035, $"Function group `{nameSyntax}` has multiple functions that match the expected type `{functionType.ToSourceCodeString()}`.");
    }

    public static Diagnostic AmbiguousMethodGroup(CodeFile file, INameExpressionSyntax nameSyntax, IMaybeFunctionType functionType)
    {
        return new(file, nameSyntax.Span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis,
            3036, $"Method group `{nameSyntax}` has multiple methods that match the expected type `{functionType.ToSourceCodeString()}`.");
    }
}
