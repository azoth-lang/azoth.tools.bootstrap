using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;

namespace Azoth.Tools.Bootstrap.Compiler.Tokens;

internal class IdentifierStringToken : IdentifierToken, IIdentifierStringToken
{
    public IdentifierStringToken(TextSpan span, string value)
        : base(span, value)
    {
    }
}

public static partial class TokenFactory
{
    public static IIdentifierStringToken IdentifierString(TextSpan span, string value)
        => new IdentifierStringToken(span, value);
}
