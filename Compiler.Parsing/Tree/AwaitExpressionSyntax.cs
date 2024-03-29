using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class AwaitExpressionSyntax : DataTypedExpressionSyntax, IAwaitExpressionSyntax
{
    public IExpressionSyntax Expression { get; }

    public AwaitExpressionSyntax(TextSpan span, IExpressionSyntax expression)
        : base(span)
    {
        Expression = expression;
    }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Unary;

    public override string ToString() => $"await {Expression.ToGroupedString(ExpressionPrecedence)}";
}
