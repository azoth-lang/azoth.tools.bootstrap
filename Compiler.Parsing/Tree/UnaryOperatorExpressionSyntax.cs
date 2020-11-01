using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree
{
    internal class UnaryOperatorExpressionSyntax : ExpressionSyntax, IUnaryOperatorExpressionSyntax
    {
        public UnaryOperatorFixity Fixity { get; }
        public UnaryOperator Operator { get; }
        private IExpressionSyntax operand;
        public ref IExpressionSyntax Operand => ref operand;

        public UnaryOperatorExpressionSyntax(
            TextSpan span,
            UnaryOperatorFixity fixity,
            UnaryOperator @operator,
            IExpressionSyntax operand)
            : base(span, ExpressionSemantics.Copy)
        {
            Operator = @operator;
            this.operand = operand;
            Fixity = fixity;
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
}
