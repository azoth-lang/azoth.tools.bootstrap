using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Tokens;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class NextExpressionSyntax : ExpressionSyntax, INextExpressionSyntax
{
    public NextExpressionSyntax(TextSpan span)
        : base(span, ExpressionSemantics.Never)
    {
    }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;

    public override string ToString()
    {
        return "next";
    }
}
