using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Tokens;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree
{
    internal class UnsafeExpressionSyntax : ExpressionSyntax, IUnsafeExpressionSyntax
    {
        private IExpressionSyntax expression;

        public ref IExpressionSyntax Expression => ref expression;

        public UnsafeExpressionSyntax(TextSpan span, IExpressionSyntax expression)
            : base(span)
        {
            this.expression = expression;
        }

        protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;

        public override string ToString()
        {
            return $"unsafe ({Expression})";
        }
    }
}
