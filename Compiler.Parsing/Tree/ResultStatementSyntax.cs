using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree
{
    /// <summary>
    /// A result statement must be the last statement of the enclosing block
    /// </summary>
    internal class ResultStatementSyntax : StatementSyntax, IResultStatementSyntax
    {
        public IExpressionSyntax Expression { [DebuggerStepThrough] get; }

        public ResultStatementSyntax(
            TextSpan span,
            IExpressionSyntax expression)
            : base(span)
        {
            Expression = expression;
        }

        public override string ToString()
        {
            return $"=> {Expression};";
        }
    }
}
