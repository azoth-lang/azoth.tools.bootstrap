using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class ExpressionBodySyntax : CodeSyntax, IExpressionBodySyntax
{
    public IResultStatementSyntax ResultStatement { get; }

    private readonly IFixedList<IStatementSyntax> statements;
    IFixedList<IStatementSyntax> IBodyOrBlockSyntax.Statements { [DebuggerStepThrough] get => statements; }

    public ExpressionBodySyntax(TextSpan span, IResultStatementSyntax resultStatement)
        : base(span)
    {
        ResultStatement = resultStatement;
        statements = resultStatement.Yield().ToFixedList<IStatementSyntax>();
    }

    public override string ToString() => "=> …;";
}
