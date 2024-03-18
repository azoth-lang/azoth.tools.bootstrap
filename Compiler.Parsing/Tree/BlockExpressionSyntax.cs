using System.Diagnostics;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class BlockExpressionSyntax : DataTypedExpressionSyntax, IBlockExpressionSyntax
{
    public IFixedList<IStatementSyntax> Statements { [DebuggerStepThrough] get; }
    IPromise<DataType?> IBlockOrResultSyntax.DataType => DataType;

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
            return $"{{ {Statements.Count} Statements }} : {DataType.ToSourceCodeString()}";

        return $"{{ }} : {DataType.ToSourceCodeString()}";
    }
}
