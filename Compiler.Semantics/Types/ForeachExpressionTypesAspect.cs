using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types;

internal static partial class ForeachExpressionTypesAspect
{
    public static partial IMaybeNonVoidType ForeachExpression_IteratorType(IForeachExpressionNode node)
    {
        var iterableType = node.InExpression?.Type.ToNonLiteral() ?? Type.Unknown;
        var iterateMethod = node.ReferencedIterateMethod;
        var iteratorType = iterableType is NonVoidType nonVoidIterableType && iterateMethod is not null
            ? nonVoidIterableType.TypeReplacements.ReplaceTypeParametersIn(iterateMethod.MethodGroupType.Return)
            : iterableType;
        // TODO report an error for void type
        return iteratorType.ToNonVoidType();
    }

    public static partial IMaybeNonVoidType ForeachExpression_IteratedType(IForeachExpressionNode node)
    {
        var nextMethodReturnType = node.ReferencedNextMethod?.MethodGroupType.Return;
        if (nextMethodReturnType is not OptionalType { Referent: var iteratedType })
            return Type.Unknown;

        if (node.IteratorType is not NonVoidType nonVoidIteratorType)
            return iteratedType;

        // TODO report an error for void type
        return nonVoidIteratorType.TypeReplacements.ReplaceTypeParametersIn(iteratedType).ToNonLiteral().ToNonVoidType();
    }

    public static partial IFlowState ForeachExpression_FlowStateBeforeBlock(IForeachExpressionNode node)
    {
        var flowState = node.InExpression?.FlowStateAfter ?? IFlowState.Empty;
        // This uses the node.BindingValueId so it doesn't conflict with the `foreach` expression result
        return flowState.Declare(node, node.InExpression?.ValueId);
    }

    public static partial IMaybeType ForeachExpression_Type(IForeachExpressionNode node)
        // TODO assign correct type to the expression
        => Type.Void;

    public static partial IFlowState ForeachExpression_FlowStateAfter(IForeachExpressionNode node)
        // TODO loop flow state
        => (node.InExpression?.FlowStateAfter.Merge(node.Block.FlowStateAfter) ?? IFlowState.Empty)
            // TODO when the `foreach` has a type other than void, correctly handle the value id
            .Constant(node.ValueId);

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
