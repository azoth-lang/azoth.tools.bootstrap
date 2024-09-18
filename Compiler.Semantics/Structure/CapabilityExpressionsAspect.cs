namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;

internal static partial class CapabilityExpressionsAspect
{
    public static partial IAmbiguousExpressionNode? AmbiguousFreezeExpression_Rewrite_Variable(IAmbiguousFreezeExpressionNode node)
    {
        if (node.Referent is not ILocalBindingNameExpressionNode localBindingName) return null;

        return IFreezeVariableExpressionNode.Create(node.Syntax, localBindingName, isTemporary: false, isImplicit: false);
    }

    public static partial IAmbiguousExpressionNode? AmbiguousFreezeExpression_Rewrite_Value(IAmbiguousFreezeExpressionNode node)
    {
        if (node.Referent is null) return null;

        return IFreezeValueExpressionNode.Create(node.Syntax, node.Referent, isTemporary: false, isImplicit: false);
    }

    public static partial IAmbiguousExpressionNode? AmbiguousMoveExpression_Rewrite_Variable(IAmbiguousMoveExpressionNode node)
    {
        if (node.Referent is not ILocalBindingNameExpressionNode localBindingName) return null;

        return IMoveVariableExpressionNode.Create(node.Syntax, localBindingName, isImplicit: false);
    }

    public static partial IAmbiguousExpressionNode? AmbiguousMoveExpression_Rewrite_Value(IAmbiguousMoveExpressionNode node)
    {
        if (node.Referent is null) return null;

        return IMoveValueExpressionNode.Create(node.Syntax, node.Referent, isImplicit: false);
    }
}
