using Azoth.Tools.Bootstrap.Compiler.Core;

namespace Azoth.Tools.Bootstrap.Compiler.Tokens;

internal class BareIdentifierToken : IdentifierToken, IBareIdentifierToken
{
    public BareIdentifierToken(TextSpan span, string value)
        : base(span, value)
    {
    }
}

public static partial class TokenFactory
{
    public static IIdentifierToken BareIdentifier(TextSpan span, string value)
        => new BareIdentifierToken(span, value);
}
