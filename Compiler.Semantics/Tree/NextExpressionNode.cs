using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class NextExpressionNode : ExpressionNode, INextExpressionNode
{
    public override INextExpressionSyntax Syntax { get; }
    public override NeverType Type => DataType.Never;

    public NextExpressionNode(INextExpressionSyntax syntax)
    {
        Syntax = syntax;
    }
}
