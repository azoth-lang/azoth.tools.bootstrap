using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class ExpressionBodySyntax : Syntax, IExpressionBodySyntax
{
    public IResultStatementSyntax ResultStatement { get; }

    private readonly FixedList<IStatementSyntax> statements;
    FixedList<IStatementSyntax> IBodyOrBlockSyntax.Statements { [DebuggerStepThrough] get => statements; }

    public ExpressionBodySyntax(TextSpan span, IResultStatementSyntax resultStatement)
        : base(span)
    {
        ResultStatement = resultStatement;
        statements = resultStatement.Yield().ToFixedList<IStatementSyntax>();
    }

    public override string ToString() => "=> …;";
}
