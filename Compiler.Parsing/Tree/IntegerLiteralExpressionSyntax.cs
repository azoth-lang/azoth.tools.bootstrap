using System.Globalization;
using System.Numerics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Tokens;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class IntegerLiteralExpressionSyntax : LiteralExpressionSyntax, IIntegerLiteralExpressionSyntax
{
    public BigInteger Value { get; }

    public IntegerLiteralExpressionSyntax(TextSpan span, BigInteger value)
        : base(span)
    {
        Value = value;
    }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;

    public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);
}
