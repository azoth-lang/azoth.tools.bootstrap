using Azoth.Tools.Bootstrap.Compiler.Lexing;
using Azoth.Tools.Bootstrap.Compiler.Tokens;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing;

internal class ModifierParser : RecursiveDescentParser
{
    public ModifierParser(ITokenIterator<IEssentialToken> tokens)
        : base(tokens) { }

    public IAccessModifierToken? ParseAccessModifier()
    {
        return Tokens.Current switch
        {
            IAccessModifierToken _ => Tokens.RequiredToken<IAccessModifierToken>(),
            _ => null
        };
    }

    public IClassCapabilityToken? ParseClassCapability()
    {
        return Tokens.Current is IClassCapabilityToken ? Tokens.RequiredToken<IClassCapabilityToken>() : null;
    }

    public void ParseEndOfModifiers()
    {
        while (!(Tokens.Current is IEndOfFileToken))
        {
            Tokens.UnexpectedToken();
        }
    }
}
