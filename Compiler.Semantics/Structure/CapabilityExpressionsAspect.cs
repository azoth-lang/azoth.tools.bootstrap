namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;

internal static partial class CapabilityExpressionsAspect
{
    public static partial IFreezeVariableExpressionNode? AmbiguousFreezeExpression_ReplaceWith_FreezeVariableExpression(IAmbiguousFreezeExpressionNode node)
    {
        if (node.Referent is not ILocalBindingNameExpressionNode referent) return null;

        return IFreezeVariableExpressionNode.Create(node.Syntax, referent, isTemporary: false, isImplicit: false);
    }

    public static partial IFreezeValueExpressionNode? AmbiguousFreezeExpression_ReplaceWith_FreezeValueExpression(IAmbiguousFreezeExpressionNode node)
    {
        if (node.Referent is not { } referent) return null;

        return IFreezeValueExpressionNode.Create(node.Syntax, referent, isTemporary: false, isImplicit: false);
    }

    public static partial IMoveVariableExpressionNode? AmbiguousMoveExpression_ReplaceWith_MoveVariableExpression(IAmbiguousMoveExpressionNode node)
    {
        if (node.Referent is not ILocalBindingNameExpressionNode referent) return null;

        return IMoveVariableExpressionNode.Create(node.Syntax, referent, isImplicit: false);
    }

    public static partial IMoveValueExpressionNode? AmbiguousMoveExpression_ReplaceWith_MoveValueExpression(IAmbiguousMoveExpressionNode node)
    {
        if (node.Referent is not { } referent) return null;

        return IMoveValueExpressionNode.Create(node.Syntax, referent, isImplicit: false);
    }
}
