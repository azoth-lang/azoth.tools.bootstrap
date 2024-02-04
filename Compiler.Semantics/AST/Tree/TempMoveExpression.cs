using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AST.Tree;

internal class TempMoveExpression : Expression, ITempMoveExpression
{
    public IExpression Referent { get; }

    public TempMoveExpression(
        TextSpan span,
        DataType dataType,
        ExpressionSemantics semantics,
        IExpression referent)
        : base(span, dataType, semantics)
    {
        Referent = referent;
    }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Min;

    public override string ToString() => $"temp_move {Referent}";
}
