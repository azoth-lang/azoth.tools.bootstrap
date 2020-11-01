using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree
{
    internal class ArgumentSyntax : Syntax, IArgumentSyntax
    {
        private IExpressionSyntax expression;
        public ref IExpressionSyntax Expression => ref expression;

        public ArgumentSyntax(IExpressionSyntax expression)
            : base(expression.Span)
        {
            this.expression = expression;
        }

        public override string ToString()
        {
            return Expression.ToString();
        }
    }
}
