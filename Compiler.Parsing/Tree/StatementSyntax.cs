using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal abstract class StatementSyntax : CodeSyntax, IStatementSyntax
{
    private protected StatementSyntax(TextSpan span)
        : base(span)
    {
    }
}
