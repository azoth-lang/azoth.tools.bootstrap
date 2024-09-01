using System;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Parameters;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types;

internal static partial class NameBindingTypesAspect
{
    public static partial DataType VariableDeclarationStatement_BindingType(IVariableDeclarationStatementNode node)
        => node.Type?.NamedType ?? InferDeclarationType(node, node.Capability) ?? DataType.Unknown;

    private static DataType? InferDeclarationType(
        IVariableDeclarationStatementNode node,
        ICapabilityNode? capability)
    {
        if (node.Initializer?.Type.ToNonConstValueType() is not NonEmptyType type)
            return null;

        if (capability is null)
        {
            if (node.Initializer is IAmbiguousMoveExpressionNode)
                // If no capability is specified and it is an explicit move, then take the mutable type.
                return type;

            // Assume read only on variables unless explicitly stated
            return type.WithoutWrite();
        }

        if (type is not CapabilityType capabilityType)
            throw new NotImplementedException("Compile error: can't infer mutability for non-capability type.");

        return capabilityType.With(capability.Capability);
    }

    public static IFlowState VariableDeclarationStatement_FlowStateAfter(IVariableDeclarationStatementNode node)
    {
        var flowStateBefore = node.Initializer?.FlowStateAfter ?? node.FlowStateBefore();
        return flowStateBefore.Declare(node, node.Initializer?.ValueId);
    }

    public static partial DataType ForeachExpression_BindingType(IForeachExpressionNode node)
        => node.DeclaredType?.NamedType ?? node.IteratedType;

    public static partial DataType BindingPattern_BindingType(IBindingPatternNode node)
        => node.ContextBindingType();

    public static IFlowState BindingPattern_FlowStateAfter(IBindingPatternNode node)
        // TODO the match referent value id could be used multiple times and perhaps shouldn't be removed here
        => node.FlowStateBefore().Declare(node, node.MatchReferentValueId);

    public static DataType PatternMatchExpression_Pattern_ContextBindingType(IPatternMatchExpressionNode node)
        => node.Referent?.Type.ToNonConstValueType() ?? DataType.Unknown;

    public static DataType OptionalPattern_Pattern_ContextBindingType(IOptionalPatternNode node)
    {
        var inheritedBindingType = node.ContextBindingType();
        if (inheritedBindingType is OptionalType optionalType)
            return optionalType.Referent;
        return inheritedBindingType;
    }

    public static DataType BindingContextPattern_Pattern_ContextBindingType(IBindingContextPatternNode node)
        => node.Type?.NamedType ?? node.ContextBindingType();

    public static partial DataType NamedParameter_BindingType(INamedParameterNode node)
        => node.TypeNode.NamedType;

    public static ParameterType NamedParameter_ParameterType(INamedParameterNode node)
    {
        bool isLent = node.IsLentBinding && node.BindingType.CanBeLent();
        return new(isLent, node.BindingType);
    }

    public static partial Pseudotype MethodSelfParameter_BindingType(IMethodSelfParameterNode node)
    {
        var declaredType = node.ContainingDeclaredType;
        var genericParameterTypes = declaredType.GenericParameterTypes;
        var capability = node.Capability;
        // TODO shouldn't their be an overload of .With() that takes an ICapabilityConstraint (e.g. `capability.Constraint`)
        return capability switch
        {
            ICapabilityNode n => declaredType.With(n.Capability, genericParameterTypes),
            ICapabilitySetNode n => declaredType.With(n.Constraint, genericParameterTypes),
            _ => throw ExhaustiveMatch.Failed(capability)
        };
    }

    private static void CheckTypeCannotBeLent(IMethodSelfParameterNode node, DiagnosticCollectionBuilder diagnostics)
    {
        var isLent = node.IsLentBinding;
        var selfType = ((IParameterNode)node).BindingType;
        if (isLent && !selfType.CanBeLent())
            diagnostics.Add(TypeError.TypeCannotBeLent(node.File, node.Syntax.Span, selfType));
    }

    public static partial CapabilityType ConstructorSelfParameter_BindingType(IConstructorSelfParameterNode node)
    {
        var declaredType = node.ContainingDeclaredType;
        var capability = node.Syntax.Capability.Declared.ToSelfParameterCapability();
        return declaredType.With(capability, declaredType.GenericParameterTypes);
    }

    public static partial DataType FieldParameter_BindingType(IFieldParameterNode node)
        => node.ReferencedField?.BindingType ?? DataType.Unknown;

    public static partial CapabilityType InitializerSelfParameter_BindingType(IInitializerSelfParameterNode node)
    {
        var declaredType = node.ContainingDeclaredType;
        var capability = node.Syntax.Capability.Declared.ToSelfParameterCapability();
        return declaredType.With(capability, declaredType.GenericParameterTypes);
    }

    private static void CheckInvalidConstructorSelfParameterCapability(
        ICapabilitySyntax capabilitySyntax,
        CodeFile file,
        DiagnosticCollectionBuilder diagnostics)
    {
        var declaredCapability = capabilitySyntax.Declared;
        switch (declaredCapability)
        {
            case DeclaredCapability.Read:
            case DeclaredCapability.Mutable:
                break;
            case DeclaredCapability.Isolated:
            case DeclaredCapability.TemporarilyIsolated:
            case DeclaredCapability.Constant:
            case DeclaredCapability.TemporarilyConstant:
            case DeclaredCapability.Identity:

                diagnostics.Add(TypeError.InvalidConstructorSelfParameterCapability(file, capabilitySyntax));
                break;
            default:
                throw ExhaustiveMatch.Failed(declaredCapability);
        }
    }

    public static SelfParameterType SelfParameter_ParameterType(ISelfParameterNode node)
    {
        bool isLent = node.IsLentBinding && node.BindingType.CanBeLent();
        return new(isLent, node.BindingType);
    }

    public static void MethodSelfParameter_ContributeDiagnostics(IMethodSelfParameterNode node, DiagnosticCollectionBuilder diagnostics)
    {
        CheckTypeCannotBeLent(node, diagnostics);

        CheckConstClassSelfParameterCannotHaveCapability(node, diagnostics);
    }

    private static void CheckConstClassSelfParameterCannotHaveCapability(
        IMethodSelfParameterNode node,
        DiagnosticCollectionBuilder diagnostics)
    {
        var inConstClass = node.ContainingDeclaredType.IsDeclaredConst;
        var selfParameterType = node.ParameterType;
        var selfType = selfParameterType.Type;
        if (inConstClass
            && ((selfType is CapabilityType { Capability: var selfCapability }
                 && selfCapability != Capability.Constant
                 && selfCapability != Capability.Identity)
                || selfType is CapabilityTypeConstraint))
            diagnostics.Add(TypeError.ConstClassSelfParameterCannotHaveCapability(node.File, node.Syntax));
    }

    public static void ConstructorSelfParameter_ContributeDiagnostics(
        IConstructorSelfParameterNode node,
        DiagnosticCollectionBuilder diagnostics)
    {
        if (node.IsLentBinding)
            diagnostics.Add(OtherSemanticError.LentConstructorOrInitializerSelf(node.File, node.Syntax));

        CheckInvalidConstructorSelfParameterCapability(node.Capability.Syntax, node.File, diagnostics);
    }

    public static ParameterType FieldParameter_ParameterType(IFieldParameterNode node)
        => new ParameterType(false, node.BindingType);

    public static void InitializerSelfParameter_ContributeDiagnostics(
        IInitializerSelfParameterNode node,
        DiagnosticCollectionBuilder diagnostics)
    {
        if (node.IsLentBinding)
            diagnostics.Add(OtherSemanticError.LentConstructorOrInitializerSelf(node.File, node.Syntax));

        CheckInvalidConstructorSelfParameterCapability(node.Capability.Syntax, node.File, diagnostics);
    }

    public static void NamedParameter_ContributeDiagnostics(INamedParameterNode node, DiagnosticCollectionBuilder diagnostics)
    {
        var type = node.BindingType;
        if (node.IsLentBinding && !type.CanBeLent())
            diagnostics.Add(TypeError.TypeCannotBeLent(node.File, node.Syntax.Span, type));
    }

    public static partial DataType FieldDefinition_BindingType(IFieldDefinitionNode node) => node.TypeNode.NamedType;
}
