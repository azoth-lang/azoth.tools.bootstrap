using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree
{
    /// <summary>
    /// A result statement must be the last statement of the enclosing block
    /// </summary>
    internal class ResultStatementSyntax : StatementSyntax, IResultStatementSyntax
    {
        private IExpressionSyntax expression;
        public ref IExpressionSyntax Expression => ref expression;

        public ResultStatementSyntax(
            TextSpan span,
            IExpressionSyntax expression)
            : base(span)
        {
            this.expression = expression;
        }

        public override string ToString()
        {
            return $"=> {Expression};";
        }
    }
}
