using System.Diagnostics;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class BlockExpressionSyntax : ExpressionSyntax, IBlockExpressionSyntax
{
    public IFixedList<IStatementSyntax> Statements { [DebuggerStepThrough] get; }

    public BlockExpressionSyntax(
        TextSpan span,
        IFixedList<IStatementSyntax> statements)
        : base(span)
    {
        Statements = statements;
    }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;

    public override string ToString()
    {
        if (Statements.Any())
            return $"{{ {Statements.Count} Statements }}";

        return $"{{ }}";
    }
}
