using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types;

internal static class ForeachExpressionTypeAspect
{
    public static DataType ForeachExpression_IteratorType(IForeachExpressionNode node)
    {
        var iterableType = node.IntermediateInExpression?.Type ?? DataType.Unknown;
        var iterateMethod = node.ReferencedIterateMethod;
        var iteratorAntetype = iterableType is NonEmptyType nonEmptyIterableType && iterateMethod is not null
            ? nonEmptyIterableType.ReplaceTypeParametersIn(iterateMethod.MethodGroupType.Return.Type)
            : iterableType;
        return iteratorAntetype;
    }

    public static DataType ForeachExpression_IteratedType(IForeachExpressionNode node)
    {
        var nextMethodReturnType = node.ReferencedNextMethod?.MethodGroupType.Return.Type;
        if (nextMethodReturnType is not OptionalType { Referent: var iteratedType })
            return DataType.Unknown;

        if (node.IteratorType is not NonEmptyType nonEmptyIteratorType)
            return iteratedType;

        return nonEmptyIteratorType.ReplaceTypeParametersIn(iteratedType).ToNonConstValueType();
    }

    public static IFlowState ForeachExpression_FlowStateBeforeBlock(IForeachExpressionNode node)
    {
        var flowState = node.IntermediateInExpression?.FlowStateAfter ?? IFlowState.Empty;
        return flowState.Declare(node, node.IntermediateInExpression?.ValueId);
    }

    public static DataType ForeachExpression_Type(IForeachExpressionNode _)
        // TODO assign correct type to the expression
        => DataType.Void;

    public static IFlowState ForeachExpression_FlowStateAfter(IForeachExpressionNode node)
        => (node.IntermediateInExpression?.FlowStateAfter.Merge(node.Block.FlowStateAfter) ?? IFlowState.Empty)
            // TODO when the `foreach` has a type other than void, correctly handle the value id
            .Constant(node.ValueId);
}
