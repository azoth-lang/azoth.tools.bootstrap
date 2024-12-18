using System;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;
using ExhaustiveMatching;
using Type = Azoth.Tools.Bootstrap.Compiler.Types.Decorated.Type;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types;

internal static partial class NameBindingTypesAspect
{
    public static partial IMaybeNonVoidType VariableDeclarationStatement_BindingType(IVariableDeclarationStatementNode node)
        // TODO report an error for void type
        => node.Type?.NamedType.ToNonVoidType() ?? InferDeclarationType(node, node.Capability) ?? Type.Unknown;

    private static IMaybeNonVoidType? InferDeclarationType(
        IVariableDeclarationStatementNode node,
        ICapabilityNode? capability)
    {
        if (node.Initializer?.Type.ToNonLiteral() is not NonVoidType type)
            return null;

        if (capability is null)
        {
            if (node.Initializer is IMoveExpressionNode)
                // If no capability is specified and it is an explicit move, then take the mutable type.
                return type;

            // Assume read only on variables unless explicitly stated
            return type.WithoutWrite();
        }

        if (type is not CapabilityType capabilityType)
            throw new NotImplementedException("Compile error: can't infer mutability for non-capability type.");

        return capabilityType.With(capability.Capability);
    }

    public static partial IFlowState VariableDeclarationStatement_FlowStateAfter(IVariableDeclarationStatementNode node)
    {
        var flowStateBefore = node.Initializer?.FlowStateAfter ?? node.FlowStateBefore();
        return flowStateBefore.Declare(node, node.Initializer?.ValueId);
    }

    public static partial IMaybeNonVoidType ForeachExpression_BindingType(IForeachExpressionNode node)
        // TODO report an error for void type
        => node.DeclaredType?.NamedType.ToNonVoidType() ?? node.IteratedType;

    public static partial IMaybeNonVoidType BindingPattern_BindingType(IBindingPatternNode node)
        => node.ContextBindingType();

    public static partial IFlowState BindingPattern_FlowStateAfter(IBindingPatternNode node)
        // TODO the match referent value id could be used multiple times and perhaps shouldn't be removed here
        => node.FlowStateBefore().Declare(node, node.MatchReferentValueId);

    public static partial IMaybeNonVoidType PatternMatchExpression_Pattern_ContextBindingType(IPatternMatchExpressionNode node)
        // TODO report an error for void type
        => node.Referent?.Type.ToNonLiteral().ToNonVoidType() ?? Type.Unknown;

    public static partial IMaybeNonVoidType OptionalPattern_Pattern_ContextBindingType(IOptionalPatternNode node)
    {
        var inheritedBindingType = node.ContextBindingType();
        if (inheritedBindingType is OptionalType optionalType)
            return optionalType.Referent;
        return inheritedBindingType;
    }

    public static partial IMaybeNonVoidType BindingContextPattern_Pattern_ContextBindingType(IBindingContextPatternNode node)
        // TODO report an error for void type
        => node.Type?.NamedType.ToNonVoidType() ?? node.ContextBindingType();

    public static partial IMaybeNonVoidType NamedParameter_BindingType(INamedParameterNode node)
        // TODO report an error for void type
        => node.TypeNode.NamedType.ToNonVoidType();

    public static partial IMaybeParameterType NamedParameter_ParameterType(INamedParameterNode node)
    {
        bool isLent = node.IsLentBinding && node.BindingType.CanBeLent();
        return ParameterType.Create(isLent, node.BindingType);
    }

    public static partial IMaybeType MethodSelfParameter_BindingType(IMethodSelfParameterNode node)
    {
        var selfType = node.ContainingSelfTypeConstructor.ConstructWithParameterTypes();
        var constraintNode = node.Constraint;
        // TODO simplify this. It ought to be possible to do something like selfType.With(constraintNode.ConstraintFor(selfType))
        return constraintNode switch
        {
            ICapabilityNode n
                => n.Capability == Capability.Read
                    ? selfType.WithDefaultCapability()
                    : selfType.With(n.Capability),
            ICapabilitySetNode n => new CapabilitySetSelfType(n.Constraint, selfType),
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

    public static partial CapabilityType ConstructorSelfParameter_BindingType(IConstructorSelfParameterNode node)
    {
        var bareType = node.ContainingTypeConstructor.ConstructWithParameterTypes();
        var capability = node.Syntax.Constraint.Declared.ToSelfParameterCapability();
        return bareType.With(capability);
    }

    // TODO this is strange because a FieldParameter isn't a binding
    public static partial IMaybeNonVoidType FieldParameter_BindingType(IFieldParameterNode node)
        => node.ReferencedField?.BindingType ?? Type.Unknown;

    public static partial CapabilityType InitializerSelfParameter_BindingType(IInitializerSelfParameterNode node)
    {
        var bareType = node.ContainingTypeConstructor.ConstructWithParameterTypes();
        var capability = node.Syntax.Constraint.Declared.ToSelfParameterCapability();
        return bareType.With(capability);
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

    public static partial IMaybeNonVoidType SelfParameter_ParameterType(ISelfParameterNode node)
        => node.BindingType.ToNonVoidType();

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

    public static partial void ConstructorSelfParameter_Contribute_Diagnostics(
        IConstructorSelfParameterNode node,
        DiagnosticCollectionBuilder diagnostics)
    {
        if (node.IsLentBinding)
            diagnostics.Add(OtherSemanticError.LentConstructorOrInitializerSelf(node.File, node.Syntax));

        CheckInvalidConstructorSelfParameterCapability(node.Capability.Syntax, node.File, diagnostics);
    }

    public static partial IMaybeParameterType FieldParameter_ParameterType(IFieldParameterNode node)
        => ParameterType.Create(false, node.BindingType);

    public static partial void InitializerSelfParameter_Contribute_Diagnostics(
        IInitializerSelfParameterNode node,
        DiagnosticCollectionBuilder diagnostics)
    {
        if (node.IsLentBinding)
            diagnostics.Add(OtherSemanticError.LentConstructorOrInitializerSelf(node.File, node.Syntax));

        CheckInvalidConstructorSelfParameterCapability(node.Capability.Syntax, node.File, diagnostics);
    }

    public static partial void NamedParameter_Contribute_Diagnostics(INamedParameterNode node, DiagnosticCollectionBuilder diagnostics)
    {
        var type = node.BindingType;
        if (node.IsLentBinding && !type.CanBeLent())
            diagnostics.Add(TypeError.TypeCannotBeLent(node.File, node.Syntax.Span, type));
    }

    public static partial IMaybeNonVoidType FieldDefinition_BindingType(IFieldDefinitionNode node)
        // TODO report an error for void type
        => node.TypeNode.NamedType.ToNonVoidType();
}
