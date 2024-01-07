using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AST.Tree;

internal class AwaitExpression : Expression, IAwaitExpression
{
    public IExpression Expression { get; }

    public AwaitExpression(TextSpan span, DataType dataType, ExpressionSemantics semantics, IExpression expression)
        : base(span, dataType, semantics)
    {
        Expression = expression;
    }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Unary;

    public override string ToString() => $"await {Expression.ToGroupedString(ExpressionPrecedence)}";
}
