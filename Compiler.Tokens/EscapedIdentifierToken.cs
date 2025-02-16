using Azoth.Tools.Bootstrap.Compiler.Core.Code;

namespace Azoth.Tools.Bootstrap.Compiler.Tokens;

internal class EscapedIdentifierToken : IdentifierToken, IEscapedIdentifierToken
{
    public EscapedIdentifierToken(TextSpan span, string value)
        : base(span, value)
    {
    }
}

public static partial class TokenFactory
{
    public static IEscapedIdentifierToken EscapedIdentifier(TextSpan span, string value)
        => new EscapedIdentifierToken(span, value);
}
