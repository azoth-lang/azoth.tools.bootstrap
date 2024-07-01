using Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;

internal static class CapabilityExpressionsAspect
{
    public static IFreezeVariableExpressionNode? AmbiguousFreezeExpression_Rewrite_Variable(IAmbiguousFreezeExpressionNode node)
    {
        if (node.IntermediateReferent is not IVariableNameExpressionNode variableName) return null;

        return new FreezeVariableExpressionNode(node.Syntax, variableName, isTemporary: false, isImplicit: false);
    }

    public static IFreezeValueExpressionNode? AmbiguousFreezeExpression_Rewrite_Value(IAmbiguousFreezeExpressionNode node)
    {
        if (node.IntermediateReferent == null) return null;

        return new FreezeValueExpressionNode(node.Syntax, node.IntermediateReferent, isTemporary: false, isImplicit: false);
    }
}
