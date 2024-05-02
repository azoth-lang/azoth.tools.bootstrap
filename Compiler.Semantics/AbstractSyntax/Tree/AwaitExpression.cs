using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AbstractSyntax.Tree;

internal class AwaitExpression : Expression, IAwaitExpression
{
    public IExpression Expression { get; }

    public AwaitExpression(TextSpan span, DataType dataType, IExpression expression)
        : base(span, dataType)
    {
        Expression = expression;
    }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Unary;

    public override string ToString() => $"await {Expression.ToGroupedString(ExpressionPrecedence)}";
}
