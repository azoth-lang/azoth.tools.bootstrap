using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AST.Tree;

internal class StringLiteralExpression : LiteralExpression, IStringLiteralExpression
{
    public string Value { get; }

    public StringLiteralExpression(
        TextSpan span,
        DataType dataType,
        ExpressionSemantics semantics,
        string value)
        : base(span, dataType, semantics)
    {
        Value = value;
    }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;

    public override string ToString() => $"\"{Value.Escape()}\"";
}
