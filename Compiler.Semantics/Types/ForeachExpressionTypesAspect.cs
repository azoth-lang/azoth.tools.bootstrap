using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;
using Azoth.Tools.Bootstrap.Compiler.Types.Flow;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types;

internal static partial class ForeachExpressionTypesAspect
{
    public static partial IMaybeNonVoidType ForeachExpression_IterableType(IForeachExpressionNode node)
        => node.InExpression?.Type.ToNonLiteral().ToNonVoid() ?? Type.Unknown;

    public static partial ContextualizedCall? ForeachExpression_IterateContextualizedCall(IForeachExpressionNode node)
        => node.ReferencedIterateMethod is { } iterateMethod
            ? ContextualizedCall.Create(node.IterableType, iterateMethod) : null;

    public static partial IMaybeNonVoidType ForeachExpression_IteratorType(IForeachExpressionNode node)
        // TODO report an error for void type
        => node.IterateContextualizedCall?.ReturnType.ToNonVoid() ?? node.IterableType;

    public static partial IMaybeNonVoidType ForeachExpression_IteratedType(IForeachExpressionNode node)
    {
        // TODO add and use a contextualized call for the next method
        var nextMethodReturnType = node.ReferencedNextMethod?.MethodGroupType.Return;
        if (nextMethodReturnType is not OptionalType { Referent: var iteratedType })
            return Type.Unknown;

        if (node.IteratorType is not NonVoidType nonVoidIteratorType)
            return iteratedType;

        // TODO report an error for void type
        return nonVoidIteratorType.TypeReplacements.ApplyTo(iteratedType).ToNonVoid();
    }

    public static partial IMaybeType ForeachExpression_Type(IForeachExpressionNode node)
        // TODO assign correct type to the expression
        => Type.Void;

    public static partial IFlowState ForeachExpression_FlowStateBeforeBlock(IForeachExpressionNode node)
    {
        var flowStateBefore = node.InExpression?.FlowStateAfter ?? IFlowState.Empty;

        // Apply any flow state changes as if .iterate() was called
        if (node is { IterateContextualizedCall: { } iterateCall, InExpression: { } inExpression })
        {
            // TODO this duplicates logic in other nodes, eliminate that duplication somehow
            var contextType = node.IterableType;
            // Apply any implicit deref needed
            // TODO do derefs need to affect the flow state?
            while (contextType is RefType refType) contextType = refType.Referent;

            var previousValueId = inExpression.ValueId;

            // TODO handle the possibility that an implicit move is needed

            // Check for and apply an implicit freeze
            var expectedType = iterateCall.SelfParameterType;
            if (expectedType is CapabilityType { Capability: var expectedCapability }
                && (expectedCapability == Capability.Constant || expectedCapability == Capability.TemporarilyConstant)
                && contextType.ToNonLiteral() is CapabilityType { Capability: var capability } capabilityType
                && capability != expectedCapability)
            {
                var isTemporary = expectedCapability == Capability.TemporarilyConstant;
                var implicitRecoveryValueId = node.ImplicitRecoveryValueId;
                if (isTemporary)
                    flowStateBefore = flowStateBefore.TempFreeze(previousValueId, implicitRecoveryValueId);
                else if (inExpression is IVariableNameExpressionNode variableExpression)
                    flowStateBefore = flowStateBefore.FreezeVariable(variableExpression.ReferencedDefinition,
                        previousValueId, implicitRecoveryValueId);
                else
                    flowStateBefore = flowStateBefore.FreezeValue(inExpression.ValueId, implicitRecoveryValueId);

                previousValueId = implicitRecoveryValueId;
            }

            // Now apply the actual .iterate() call
            // TODO this, like ArgumentValueIds assumes the self parameter is not lent, but it can be!
            var selfArgumentValueId = new ArgumentValueId(false, previousValueId);
            flowStateBefore = flowStateBefore.CombineArguments(selfArgumentValueId.Yield(), node.IteratorValueId, node.IteratorType);

            // Hold the iterator in the node.IteratorValueId temporary value for the duration of the loop
        }

        // TODO apply flow state changes as if .next() was called and use the output as the initializer for the loop variable

        // TODO this declares the variable outside the block which could cause the issue where closures
        // capture a variable that lives too long.
        // TODO null is wrong here, it is initialized with the result of .next()
        // This uses the node.BindingValueId so it doesn't conflict with the `foreach` expression result
        return flowStateBefore.Declare(node, null);
    }

    public static partial IFlowState ForeachExpression_FlowStateAfterBlock(IForeachExpressionNode node)
        // Merge FlowStateBeforeBlock with Block.FlowStateAfter because the body may not be executed.
        // Then drop the loop variable binding which is created outside the block.
        => node.FlowStateBeforeBlock.Merge(node.Block.FlowStateAfter).DropBindings([node]);

    public static partial IFlowState ForeachExpression_FlowStateAfter(IForeachExpressionNode node)
    {
        // The FlowStateAfterBlock has already been merged properly with the before block, no need
        // to merge again here.
        var flowStateAfter = node.FlowStateAfterBlock;
        if (node.IterateContextualizedCall is not null && node.InExpression is not null)
            // Drop the temporary that is holding the iterator
            flowStateAfter = flowStateAfter.DropValue(node.IteratorValueId);

        // TODO when the `foreach` has a type other than void, correctly handle the value id
        flowStateAfter = flowStateAfter.Constant(node.ValueId);
        return flowStateAfter;
    }

    public static partial void ForeachExpression_Contribute_Diagnostics(IForeachExpressionNode node, DiagnosticCollectionBuilder diagnostics)
    {
        var iterableType = node.InExpression?.Type ?? Type.Unknown;
        if (iterableType is UnknownType)
            // Don't know if there are any errors until the type is known
            return;

        if (node.IteratorType is UnknownType)
            diagnostics.Add(OtherSemanticError.ForeachNoIterateOrNextMethod(node.File, node.TempInExpression.Syntax, iterableType.ToNonLiteral()));
        else if (node.ReferencedNextMethod is null)
            diagnostics.Add(OtherSemanticError.ForeachNoNextMethod(node.File, node.TempInExpression.Syntax, iterableType.ToNonLiteral()));
    }
}
