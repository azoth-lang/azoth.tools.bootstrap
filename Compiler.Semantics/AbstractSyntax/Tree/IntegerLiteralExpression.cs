using System.Globalization;
using System.Numerics;
using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AbstractSyntax.Tree;

internal class IntegerLiteralExpression : LiteralExpression, IIntegerLiteralExpression
{
    public BigInteger Value { get; }

    public IntegerLiteralExpression(TextSpan span, DataType dataType, BigInteger value)
        : base(span, dataType)
    {
        Value = value;
    }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;

    public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);
}
