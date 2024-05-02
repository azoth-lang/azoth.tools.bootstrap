using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AbstractSyntax.Tree;

internal class TempMoveExpression : Expression, ITempMoveExpression
{
    public IExpression Referent { get; }

    public TempMoveExpression(TextSpan span, DataType dataType, IExpression referent)
        : base(span, dataType)
    {
        Referent = referent;
    }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Min;

    public override string ToString() => $"temp_move {Referent}";
}
