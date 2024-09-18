using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types;

internal static partial class ForeachExpressionTypesAspect
{
    public static partial DataType ForeachExpression_IteratorType(IForeachExpressionNode node)
    {
        var iterableType = node.InExpression?.Type ?? DataType.Unknown;
        var iterateMethod = node.ReferencedIterateMethod;
        var iteratorType = iterableType is NonEmptyType nonEmptyIterableType && iterateMethod is not null
            ? nonEmptyIterableType.ReplaceTypeParametersIn(iterateMethod.MethodGroupType.Return.Type)
            : iterableType;
        return iteratorType;
    }

    public static partial DataType ForeachExpression_IteratedType(IForeachExpressionNode node)
    {
        var nextMethodReturnType = node.ReferencedNextMethod?.MethodGroupType.Return.Type;
        if (nextMethodReturnType is not OptionalType { Referent: var iteratedType })
            return DataType.Unknown;

        if (node.IteratorType is not NonEmptyType nonEmptyIteratorType)
            return iteratedType;

        return nonEmptyIteratorType.ReplaceTypeParametersIn(iteratedType).ToNonConstValueType();
    }

    public static partial IFlowState ForeachExpression_FlowStateBeforeBlock(IForeachExpressionNode node)
    {
        var flowState = node.InExpression?.FlowStateAfter ?? IFlowState.Empty;
        // This uses the node.BindingValueId so it doesn't conflict with the `foreach` expression result
        return flowState.Declare(node, node.InExpression?.ValueId);
    }

    public static partial DataType ForeachExpression_Type(IForeachExpressionNode node)
        // TODO assign correct type to the expression
        => DataType.Void;

    public static partial IFlowState ForeachExpression_FlowStateAfter(IForeachExpressionNode node)
        // TODO loop flow state
        => (node.InExpression?.FlowStateAfter.Merge(node.Block.FlowStateAfter) ?? IFlowState.Empty)
            // TODO when the `foreach` has a type other than void, correctly handle the value id
            .Constant(node.ValueId);

    public static partial void ForeachExpression_Contribute_Diagnostics(IForeachExpressionNode node, DiagnosticCollectionBuilder diagnostics)
    {
        var iterableType = node.InExpression?.Type ?? DataType.Unknown;
        if (iterableType is UnknownType)
            // Don't know if there are any errors until the type is known
            return;

        if (node.IteratorType is UnknownType)
            diagnostics.Add(OtherSemanticError.ForeachNoIterateOrNextMethod(node.File, node.TempInExpression.Syntax, iterableType));
        else if (node.ReferencedNextMethod is null)
            diagnostics.Add(OtherSemanticError.ForeachNoNextMethod(node.File, node.TempInExpression.Syntax, iterableType));
    }
}