using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Tokens;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree
{
    internal class ReturnExpressionSyntax : ExpressionSyntax, IReturnExpressionSyntax
    {
        public IExpressionSyntax? Value { [DebuggerStepThrough] get; }

        public ReturnExpressionSyntax(
            TextSpan span,
            IExpressionSyntax? value)
            : base(span, ExpressionSemantics.Never)
        {
            Value = value;
        }

        protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Min;

        public override string ToString()
        {
            return Value is null ? "return" : $"return {Value}";
        }
    }
}
