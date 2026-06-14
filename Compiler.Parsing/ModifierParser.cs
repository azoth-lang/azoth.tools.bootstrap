using Azoth.Tools.Bootstrap.Compiler.Lexing;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Tokens;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing;

/// <summary>
/// Parser for modifiers which are collected and then parsed separately in order to parse them after
/// determining what definition they are for.
/// </summary>
internal class ModifierParser : SharedModifierParser
{
    public ModifierParser(ITokenIterator<IEssentialToken> tokens)
        : base(tokens) { }

    public AccessModifierSyntax ParseAccessModifier()
        => AcceptAccessModifier() ?? AccessModifierSyntax.Private;

    public IConstKeywordToken? ParseConstModifier() => Tokens.AcceptToken<IConstKeywordToken>();

    public IDropKeywordToken? ParseDropModifier() => Tokens.AcceptToken<IDropKeywordToken>();

    public IAbstractKeywordToken? ParseAbstractModifier()
        => Tokens.AcceptToken<IAbstractKeywordToken>();

    public void ParseEndOfModifiers()
    {
        while (Tokens.Current is not IEndOfFileToken)
        {
            Tokens.UnexpectedToken();
        }
    }
}
