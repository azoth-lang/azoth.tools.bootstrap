using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class AsyncStartExpressionSyntax : ExpressionSyntax, IAsyncStartExpressionSyntax
{
    public bool Scheduled { get; }
    public IExpressionSyntax Expression { get; }

    public AsyncStartExpressionSyntax(TextSpan span, bool scheduled, IExpressionSyntax expression)
        : base(span)
    {
        Scheduled = scheduled;
        Expression = expression;
    }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Min;

    public override string ToString()
    {
        var op = Scheduled ? "go" : "do";
        return $"{op} {Expression.ToGroupedString(ExpressionPrecedence)}";
    }
}
