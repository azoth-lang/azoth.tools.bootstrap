using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AST.Tree;

internal class BreakExpression : Expression, IBreakExpression
{
    public IExpression? Value { get; }

    public BreakExpression(TextSpan span, DataType dataType, IExpression? value)
        : base(span, dataType)
    {
        Value = value;
    }

    protected override OperatorPrecedence ExpressionPrecedence =>
        Value is not null ? OperatorPrecedence.Min : OperatorPrecedence.Primary;

    public override string ToString()
    {
        if (Value is not null) return $"break {Value}";
        return "break";
    }
}
