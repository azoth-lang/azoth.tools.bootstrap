using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Tokens;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree
{
    internal class WhileExpressionSyntax : ExpressionSyntax, IWhileExpressionSyntax
    {
        private IExpressionSyntax condition;
        public ref IExpressionSyntax Condition => ref condition;

        public IBlockExpressionSyntax Block { get; }

        public WhileExpressionSyntax(
            TextSpan span,
            IExpressionSyntax condition,
            IBlockExpressionSyntax block)
            : base(span)
        {
            this.condition = condition;
            Block = block;
        }

        protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Min;

        public override string ToString()
        {
            return $"while {Condition} {Block}";
        }
    }
}
