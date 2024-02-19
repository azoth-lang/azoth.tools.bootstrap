using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AST.Tree;

internal class BlockExpression : Expression, IBlockExpression
{
    public IFixedList<IStatement> Statements { get; }

    public BlockExpression(
        TextSpan span,
        DataType dataType,
        ExpressionSemantics semantics,
        IFixedList<IStatement> statements)
        : base(span, dataType, semantics)
    {
        Statements = statements;
    }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;

    public override string ToString()
    {
        if (Statements.Any()) return $"{{ {Statements.Count} Statements }} : {DataType.ToILString()}";

        return $"{{ }} : {DataType.ToILString()}";
    }
}
