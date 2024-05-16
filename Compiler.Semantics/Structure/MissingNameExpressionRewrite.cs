using Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;

internal static class MissingNameExpressionRewrite
{
    public static IMissingNameExpressionNode? IdentifierNameExpressionNode_Rewrite(IIdentifierNameExpressionNode node)
    {
        if (node.Name is null)
            return new MissingNameExpressionNode(node.Syntax);

        return null;
    }
}
