using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Tokens;

internal class StringLiteralToken : Token, IStringLiteralToken
{
    public string Value { get; }

    public StringLiteralToken(TextSpan span, string value)
        : base(span)
    {
        Value = value;
    }

    // Helpful for debugging
    public override string ToString()
    {
        return '"' + Value.Escape() + '"';
    }
}

public static partial class TokenFactory
{

    public static IStringLiteralToken StringLiteral(TextSpan span, string value)
    {
        return new StringLiteralToken(span, value);
    }
}
