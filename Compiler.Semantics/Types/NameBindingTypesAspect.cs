using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;
using Azoth.Tools.Bootstrap.Compiler.Types.Flow;
using ExhaustiveMatching;
using Type = Azoth.Tools.Bootstrap.Compiler.Types.Decorated.Type;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types;

internal static partial class NameBindingTypesAspect
{
    public static partial IMaybeNonVoidType VariableDeclarationStatement_BindingType(IVariableDeclarationStatementNode node)
        // TODO report an error for void type
        => node.Type?.NamedType.ToNonVoid() ?? node.Initializer?.InferredDeclarationType(node.Capability) ?? Type.Unknown;

    // TODO move this logic into the types project

    private static IMaybeNonVoidType? InferredDeclarationType(this IExpressionNode initializer, ICapabilityNode? capability)
    {
        var type = initializer.Type;
        if (capability is null && initializer is IMoveExpressionNode)
            // If no capability is specified and it is an explicit move, then take the mutable type.
            return type.ToNonLiteral().ToNonVoid();

        return type.InferredDeclarationType(capability?.DeclaredCapability);
    }

    public static partial IFlowState VariableDeclarationStatement_FlowStateAfter(IVariableDeclarationStatementNode node)
    {
        var flowStateBefore = node.Initializer?.FlowStateAfter ?? node.FlowStateBefore();
        return flowStateBefore.Declare(node, node.Initializer?.ValueId);
    }

    public static partial IMaybeNonVoidType ForeachExpression_BindingType(IForeachExpressionNode node)
        // TODO report an error for void type
        => node.DeclaredType?.NamedType.ToNonVoid() ?? node.IteratedType.ToNonLiteral();

    public static partial IMaybeNonVoidType BindingPattern_BindingType(IBindingPatternNode node)
        => node.ContextBindingType();

    public static partial IFlowState BindingPattern_FlowStateAfter(IBindingPatternNode node)
        // Do not drop the initializer value because it can be used by multiple bindings within a
        // pattern. It is dropped by the pattern match expression when all patterns are complete.
        => node.FlowStateBefore().Declare(node, node.MatchReferentValueId, dropInitializer: false);

    public static partial IMaybeNonVoidType PatternMatchExpression_Pattern_ContextBindingType(IPatternMatchExpressionNode node)
        // TODO report an error for void type
        => node.Referent?.Type.ToNonLiteral().ToNonVoid() ?? Type.Unknown;

    public static partial IMaybeNonVoidType OptionalPattern_Pattern_ContextBindingType(IOptionalPatternNode node)
    {
        var inheritedBindingType = node.ContextBindingType();
        if (inheritedBindingType is OptionalType optionalType)
            return optionalType.Referent;
        return inheritedBindingType;
    }

    public static partial IMaybeNonVoidType BindingContextPattern_Pattern_ContextBindingType(IBindingContextPatternNode node)
        // TODO report an error for void type
        => node.Type?.NamedType.ToNonVoid() ?? node.ContextBindingType();

    public static partial IMaybeNonVoidType NamedParameter_BindingType(INamedParameterNode node)
        // TODO report an error for void type
        => node.TypeNode.NamedType.ToNonVoid();

    public static partial IMaybeParameterType NamedParameter_ParameterType(INamedParameterNode node)
    {
        bool isLent = node.IsLentBinding && node.BindingType.CanBeLent();
        return ParameterType.Create(isLent, node.BindingType);
    }

    public static partial IMaybeNonVoidType MethodSelfParameter_BindingType(IMethodSelfParameterNode node)
    {
        var selfType = node.ContainingSelfTypeConstructor.ConstructWithParameterTypes(node.BindingPlainType);
        var constraintNode = node.Constraint;
        return constraintNode switch
        {
            ICapabilityNode n => selfType.With(n.DeclaredCapability),
            ICapabilitySetNode n => new CapabilitySetSelfType(n.DeclaredCapabilitySet.ToCapabilitySet(), selfType),
            _ => throw ExhaustiveMatch.Failed(constraintNode)
        };
    }

    private static void CheckTypeCannotBeLent(IMethodSelfParameterNode node, DiagnosticCollectionBuilder diagnostics)
    {
        var isLent = node.IsLentBinding;
        var selfType = ((IParameterNode)node).BindingType;
        if (isLent && !selfType.CanBeLent())
            diagnostics.Add(TypeError.TypeCannotBeLent(node.File, node.Syntax.Span, selfType));
    }

    // TODO this is strange because a FieldParameter isn't a binding
    public static partial IMaybeNonVoidType FieldParameter_BindingType(IFieldParameterNode node)
        => node.ReferencedField?.BindingType ?? Type.Unknown;

    public static partial CapabilityType InitializerSelfParameter_BindingType(IInitializerSelfParameterNode node)
    {
        var bareType = node.ContainingSelfTypeConstructor.ConstructWithParameterTypes(node.BindingPlainType);
        var capability = node.Capability.DeclaredCapability.ToSelfParameterCapability();
        return bareType.With(capability);
    }

    private static void CheckInvalidInitializerSelfParameterCapability(
        ICapabilitySyntax capabilitySyntax,
        CodeFile file,
        DiagnosticCollectionBuilder diagnostics)
    {
        var declaredCapability = capabilitySyntax.Capability;
        switch (declaredCapability)
        {
            case DeclaredCapability.Default:
            case DeclaredCapability.Mutable:
                break;
            case DeclaredCapability.Read:
            case DeclaredCapability.Isolated:
            case DeclaredCapability.TemporarilyIsolated:
            case DeclaredCapability.Constant:
            case DeclaredCapability.TemporarilyConstant:
            case DeclaredCapability.Identity:
                diagnostics.Add(TypeError.InvalidInitializerSelfParameterCapability(file, capabilitySyntax));
                break;
            default:
                throw ExhaustiveMatch.Failed(declaredCapability);
        }
    }

    public static partial IMaybeNonVoidType SelfParameter_ParameterType(ISelfParameterNode node)
        => node.BindingType.ToNonVoid();

    public static partial void MethodSelfParameter_Contribute_Diagnostics(IMethodSelfParameterNode node, DiagnosticCollectionBuilder diagnostics)
    {
        CheckTypeCannotBeLent(node, diagnostics);

        CheckConstClassSelfParameterCannotHaveCapability(node, diagnostics);
    }

    private static void CheckConstClassSelfParameterCannotHaveCapability(
        IMethodSelfParameterNode node,
        DiagnosticCollectionBuilder diagnostics)
    {
        var inConstClass = node.ContainingTypeConstructor.IsDeclaredConst;
        var selfType = node.ParameterType;
        if (inConstClass
            && ((selfType is CapabilityType { Capability: var selfCapability }
                 && selfCapability != Capability.Constant
                 && selfCapability != Capability.Identity)
                || selfType is CapabilitySetSelfType))
            diagnostics.Add(TypeError.ConstClassSelfParameterCannotHaveCapability(node.File, node.Syntax));
    }

    public static partial IMaybeParameterType FieldParameter_ParameterType(IFieldParameterNode node)
        => ParameterType.Create(false, node.BindingType);

    public static partial void InitializerSelfParameter_Contribute_Diagnostics(
        IInitializerSelfParameterNode node,
        DiagnosticCollectionBuilder diagnostics)
    {
        if (node.IsLentBinding)
            diagnostics.Add(OtherSemanticError.LentInitializerSelf(node.File, node.Syntax));

        CheckInvalidInitializerSelfParameterCapability(node.Capability.Syntax, node.File, diagnostics);
    }

    public static partial void NamedParameter_Contribute_Diagnostics(INamedParameterNode node, DiagnosticCollectionBuilder diagnostics)
    {
        var type = node.BindingType;
        if (node.IsLentBinding && !type.CanBeLent())
            diagnostics.Add(TypeError.TypeCannotBeLent(node.File, node.Syntax.Span, type));
    }

    public static partial IMaybeNonVoidType FieldDefinition_BindingType(IFieldDefinitionNode node)
        // TODO report an error for void type
        => node.TypeNode.NamedType.ToNonVoid();
}
