using Azoth.Tools.Bootstrap.Compiler.Lexing;
using Azoth.Tools.Bootstrap.Compiler.Tokens;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing;

internal class ModifierParser : RecursiveDescentParser
{
    public ModifierParser(ITokenIterator<IEssentialToken> tokens)
        : base(tokens) { }

    public IAccessModifierToken? ParseAccessModifier()
        => Tokens.AcceptToken<IAccessModifierToken>();

    public IClassCapabilityToken? ParseClassCapability()
        => Tokens.AcceptToken<IClassCapabilityToken>();

    public void ParseEndOfModifiers()
    {
        while (Tokens.Current is not IEndOfFileToken)
        {
            Tokens.UnexpectedToken();
        }
    }
}
