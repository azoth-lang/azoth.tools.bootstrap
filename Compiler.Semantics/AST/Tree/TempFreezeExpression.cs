using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AST.Tree;

internal class TempFreezeExpression : Expression, ITempFreezeExpression
{
    public IExpression Referent { get; }

    public TempFreezeExpression(TextSpan span, DataType dataType, IExpression referent)
        : base(span, dataType)
    {
        Referent = referent;
    }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Min;

    public override string ToString() => $"temp_freeze {Referent}";
}
