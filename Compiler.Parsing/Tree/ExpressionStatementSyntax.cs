using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class ExpressionStatementSyntax : StatementSyntax, IExpressionStatementSyntax
{
    public IExpressionSyntax Expression { [DebuggerStepThrough] get; }

    public ExpressionStatementSyntax(TextSpan span, IExpressionSyntax expression)
        : base(span)
    {
        Expression = expression;
    }

    public override string ToString() => Expression + ";";
}
