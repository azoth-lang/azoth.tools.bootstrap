using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree
{
    internal class ExpressionStatementSyntax : StatementSyntax, IExpressionStatementSyntax
    {
        private IExpressionSyntax expression;
        public ref IExpressionSyntax Expression
        {
            [DebuggerStepThrough]
            get => ref expression;
        }

        public ExpressionStatementSyntax(TextSpan span, IExpressionSyntax expression)
            : base(span)
        {
            this.expression = expression;
        }

        public override string ToString()
        {
            return Expression+";";
        }
    }
}
