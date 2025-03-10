using Azoth.Tools.Bootstrap.Compiler.Core.Code;

namespace Azoth.Tools.Bootstrap.Compiler.Tokens;

internal abstract class IdentifierToken : Token
{
    public string Value { get; }

    protected IdentifierToken(TextSpan span, string value)
        : base(span)
    {
        Value = value;
    }

    // Helpful for debugging
    public override string ToString() => Value;
}
