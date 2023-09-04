using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AST.Tree;

internal class ReturnExpression : Expression, IReturnExpression
{
    public IExpression? Value { get; }

    public ReturnExpression(
        TextSpan span,
        DataType dataType,
        ExpressionSemantics semantics,
        IExpression? value)
        : base(span, dataType, semantics)
    {
        Value = value;
    }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Min;

    public override string ToString() => Value is null ? "return" : $"return {Value}";
}
