using System.Globalization;
using System.Numerics;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;

namespace Azoth.Tools.Bootstrap.Compiler.Tokens;

internal class IntegerLiteralToken : Token, IIntegerLiteralToken
{
    public BigInteger Value { get; }

    public IntegerLiteralToken(TextSpan span, BigInteger value)
        : base(span)
    {
        Value = value;
    }

    // Helpful for debugging
    public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);
}

public static partial class TokenFactory
{
    public static IIntegerLiteralToken IntegerLiteral(TextSpan span, BigInteger value)
        => new IntegerLiteralToken(span, value);
}
