using System;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types;

internal static partial class NameBindingTypesAspect
{
    public static DataType VariableDeclarationStatement_BindingType(IVariableDeclarationStatementNode node)
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

    public static DataType ForeachExpression_BindingType(IForeachExpressionNode node)
        => node.DeclaredType?.NamedType ?? node.IteratedType;

    public static DataType BindingPattern_BindingType(IBindingPatternNode node)
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
}
