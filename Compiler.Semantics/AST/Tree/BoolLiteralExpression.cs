using System.Globalization;
using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AST.Tree;

internal class BoolLiteralExpression : LiteralExpression, IBoolLiteralExpression
{
    public bool Value { get; }

    public BoolLiteralExpression(
        TextSpan span,
        DataType dataType,
        ExpressionSemantics semantics,
        bool value)
        : base(span, dataType, semantics)
    {
        Value = value;
    }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;

    public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);
}
