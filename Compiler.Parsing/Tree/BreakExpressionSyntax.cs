using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Tokens;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree
{
    internal class BreakExpressionSyntax : ExpressionSyntax, IBreakExpressionSyntax
    {
        private IExpressionSyntax? value;
        public ref IExpressionSyntax? Value => ref value;

        public BreakExpressionSyntax(
            TextSpan span,
            IExpressionSyntax? value)
            : base(span, ExpressionSemantics.Void)
        {
            this.value = value;
        }

        protected override OperatorPrecedence ExpressionPrecedence => Value != null ? OperatorPrecedence.Min : OperatorPrecedence.Primary;

        public override string ToString()
        {
            if (Value != null)
                return $"break {Value}";
            return "break";
        }
    }
}
