using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.ConstValue;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class BoolLiteralExpressionNode : ExpressionNode, IBoolLiteralExpressionNode
{
    public override IBoolLiteralExpressionSyntax Syntax { get; }
    public bool Value => Syntax.Value;
    public BoolConstValueType Type => TypeExpressionsAspect.BoolLiteralExpression_Type(this);

    public BoolLiteralExpressionNode(IBoolLiteralExpressionSyntax syntax)
    {
        Syntax = syntax;
    }
}
