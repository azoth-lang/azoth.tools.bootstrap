using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AST.Tree;

internal class AsyncStartExpression : Expression, IAsyncStartExpression
{
    public bool Scheduled { get; }
    public IExpression Expression { get; }

    public AsyncStartExpression(TextSpan span, DataType dataType, bool scheduled, IExpression expression)
        : base(span, dataType)
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
