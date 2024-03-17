using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AST.Tree;

internal class BinaryOperatorExpression : Expression, IBinaryOperatorExpression
{
    public IExpression LeftOperand { get; }
    public BinaryOperator Operator { get; }
    public IExpression RightOperand { get; }

    public BinaryOperatorExpression(
        TextSpan span,
        DataType dataType,
        IExpression leftOperand,
        BinaryOperator @operator,
        IExpression rightOperand)
        : base(span, dataType)
    {
        LeftOperand = leftOperand;
        Operator = @operator;
        RightOperand = rightOperand;
    }

    protected override OperatorPrecedence ExpressionPrecedence => Operator.Precedence();

    public override string ToString()
        => $"{LeftOperand.ToGroupedString(ExpressionPrecedence)} {Operator.ToSymbolString()} {RightOperand.ToGroupedString(ExpressionPrecedence)}";
}
