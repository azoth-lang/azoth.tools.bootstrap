using Azoth.Tools.Bootstrap.Compiler.Lexing;
using Azoth.Tools.Bootstrap.Compiler.Tokens;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing;

internal class ModifierParser : RecursiveDescentParser
{
    public ModifierParser(ITokenIterator<IEssentialToken> tokens)
        : base(tokens) { }

    public IAccessModifierToken? ParseAccessModifier()
        => Tokens.AcceptToken<IAccessModifierToken>();

    public IConstKeywordToken? ParseConstModifier() => Tokens.AcceptToken<IConstKeywordToken>();

    public IMoveKeywordToken? ParseMoveModifier() => Tokens.AcceptToken<IMoveKeywordToken>();

    public void ParseEndOfModifiers()
    {
        while (Tokens.Current is not IEndOfFileToken)
        {
            Tokens.UnexpectedToken();
        }
    }
}
