using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Syntax;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class NextExpressionSyntax : ExpressionSyntax, INextExpressionSyntax
{
    public NextExpressionSyntax(TextSpan span)
        : base(span)
    {
    }

    public override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;

    public override string ToString() => "next";
}
