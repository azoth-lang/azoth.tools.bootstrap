using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types;

internal static class ForeachExpressionTypeAspect
{
    public static DataType ForeachExpression_IteratorType(IForeachExpressionNode node)
    {
        var iterableType = node.IntermediateInExpression?.Type ?? DataType.Unknown;
        var iterateMethod = node.ReferencedIterateMethod;
        var iteratorType = iterableType is NonEmptyType nonEmptyIterableType && iterateMethod is not null
            ? nonEmptyIterableType.ReplaceTypeParametersIn(iterateMethod.MethodGroupType.Return.Type)
            : iterableType;
        return iteratorType;
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
        // This uses the node.BindingValueId so it doesn't conflict with the `foreach` expression result
        return flowState.Declare(node, node.IntermediateInExpression?.ValueId);
    }

    public static DataType ForeachExpression_Type(IForeachExpressionNode _)
        // TODO assign correct type to the expression
        => DataType.Void;

    public static IFlowState ForeachExpression_FlowStateAfter(IForeachExpressionNode node)
        // TODO loop flow state
        => (node.IntermediateInExpression?.FlowStateAfter.Merge(node.Block.FlowStateAfter) ?? IFlowState.Empty)
            // TODO when the `foreach` has a type other than void, correctly handle the value id
            .Constant(node.ValueId);

    public static void ForeachExpression_ContributeDiagnostics(IForeachExpressionNode node, Diagnostics diagnostics)
    {
        var iterableType = node.IntermediateInExpression?.Type ?? DataType.Unknown;
        if (iterableType is UnknownType)
            // Don't know if there are any errors until the type is known
            return;

        if (node.IteratorType is UnknownType)
            diagnostics.Add(OtherSemanticError.ForeachNoIterateOrNextMethod(node.File, node.InExpression.Syntax, iterableType));
        else if (node.ReferencedNextMethod is null)
            diagnostics.Add(OtherSemanticError.ForeachNoNextMethod(node.File, node.InExpression.Syntax, iterableType));
    }
}
