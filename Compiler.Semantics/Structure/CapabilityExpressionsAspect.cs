using Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;

internal static class CapabilityExpressionsAspect
{
    public static IFreezeVariableExpressionNode? AmbiguousFreezeExpression_Rewrite_Variable(IAmbiguousFreezeExpressionNode node)
    {
        if (node.IntermediateReferent is not ILocalBindingNameExpressionNode localBindingName) return null;

        return new FreezeVariableExpressionNode(node.Syntax, localBindingName, isTemporary: false, isImplicit: false);
    }

    public static IFreezeValueExpressionNode? AmbiguousFreezeExpression_Rewrite_Value(IAmbiguousFreezeExpressionNode node)
    {
        if (node.IntermediateReferent is null) return null;

        return new FreezeValueExpressionNode(node.Syntax, node.IntermediateReferent, isTemporary: false, isImplicit: false);
    }

    public static IChildNode? AmbiguousMoveExpression_Rewrite_Variable(IAmbiguousMoveExpressionNode node)
    {
        if (node.IntermediateReferent is not ILocalBindingNameExpressionNode localBindingName) return null;

        return new MoveVariableExpressionNode(node.Syntax, localBindingName, isImplicit: false);
    }

    public static IChildNode? AmbiguousMoveExpression_Rewrite_Value(IAmbiguousMoveExpressionNode node)
    {
        if (node.IntermediateReferent is null) return null;

        return new MoveValueExpressionNode(node.Syntax, node.IntermediateReferent, isImplicit: false);
    }
}
