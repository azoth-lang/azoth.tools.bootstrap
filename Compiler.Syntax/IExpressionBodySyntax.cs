using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Syntax;

public partial interface IExpressionBodySyntax
{
    public static IExpressionBodySyntax Create(TextSpan span, IResultStatementSyntax resultStatement)
    {
        // TODO allow AG to specify that the statements is always a single result statement
        var statements = resultStatement.Yield().ToFixedList<IStatementSyntax>();
        return Create(span, resultStatement, statements);
    }
}
