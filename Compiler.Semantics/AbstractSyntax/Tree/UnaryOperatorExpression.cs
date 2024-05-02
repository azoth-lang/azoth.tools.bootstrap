using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Types;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AbstractSyntax.Tree;

internal class UnaryOperatorExpression : Expression, IUnaryOperatorExpression
{
    public UnaryOperatorFixity Fixity { get; }
    public UnaryOperator Operator { get; }
    public IExpression Operand { get; }

    public UnaryOperatorExpression(
        TextSpan span,
        DataType dataType,
        UnaryOperatorFixity fixity,
        UnaryOperator @operator,
        IExpression operand)
        : base(span, dataType)
    {
        Fixity = fixity;
        Operator = @operator;
        Operand = operand;
    }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Unary;

    public override string ToString()
    {
        return Fixity switch
        {
            UnaryOperatorFixity.Prefix => $"{Operator.ToSymbolString()}{Operand.ToGroupedString(ExpressionPrecedence)}",
            UnaryOperatorFixity.Postfix => $"{Operand.ToGroupedString(ExpressionPrecedence)}{Operator.ToSymbolString()}",
            _ => throw ExhaustiveMatch.Failed(Fixity)
        };
    }
}
