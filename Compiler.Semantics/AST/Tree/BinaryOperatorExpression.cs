using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Compiler.Types;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AST.Tree
{
    internal class BinaryOperatorExpression : Expression, IBinaryOperatorExpression
    {
        public IExpression LeftOperand { get; }
        public BinaryOperator Operator { get; }
        public IExpression RightOperand { get; }

        public BinaryOperatorExpression(
            TextSpan span,
            DataType dataType,
            ExpressionSemantics semantics,
            IExpression leftOperand,
            BinaryOperator @operator,
            IExpression rightOperand)
            : base(span, dataType, semantics)
        {
            LeftOperand = leftOperand;
            Operator = @operator;
            RightOperand = rightOperand;
        }

        protected override OperatorPrecedence ExpressionPrecedence =>
            Operator switch
            {
                BinaryOperator.And => OperatorPrecedence.LogicalAnd,
                BinaryOperator.Or => OperatorPrecedence.LogicalOr,

                BinaryOperator.Plus => OperatorPrecedence.Additive,
                BinaryOperator.Minus => OperatorPrecedence.Additive,

                BinaryOperator.Asterisk => OperatorPrecedence.Multiplicative,
                BinaryOperator.Slash => OperatorPrecedence.Multiplicative,

                BinaryOperator.DotDot => OperatorPrecedence.Range,
                BinaryOperator.DotDotLessThan => OperatorPrecedence.Range,
                BinaryOperator.LessThanDotDot => OperatorPrecedence.Range,
                BinaryOperator.LessThanDotDotLessThan => OperatorPrecedence.Range,

                BinaryOperator.EqualsEquals => OperatorPrecedence.Relational,
                BinaryOperator.NotEqual => OperatorPrecedence.Relational,
                BinaryOperator.LessThan => OperatorPrecedence.Relational,
                BinaryOperator.LessThanOrEqual => OperatorPrecedence.Relational,
                BinaryOperator.GreaterThan => OperatorPrecedence.Relational,
                BinaryOperator.GreaterThanOrEqual => OperatorPrecedence.Relational,

                _ => throw ExhaustiveMatch.Failed(Operator),
            };

        public override string ToString()
            => $"{LeftOperand.ToGroupedString(ExpressionPrecedence)} {Operator.ToSymbolString()} {RightOperand.ToGroupedString(ExpressionPrecedence)}";
    }
}
