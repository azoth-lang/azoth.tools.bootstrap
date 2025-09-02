using Azoth.Tools.Bootstrap.Compiler.Lexing;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Tokens;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing;

internal class ModifierParser : RecursiveDescentParser
{
    public ModifierParser(ITokenIterator<IEssentialToken> tokens)
        : base(tokens) { }

    public AccessModifierSyntax ParseAccessModifiers()
    {
        if (Tokens.AcceptToken<IPublishedKeywordToken>() is not { } publishedToken)
            return AccessModifierSyntax.Package(Tokens.AcceptToken<IPackageAccessModifierToken>());
        if (Tokens.AcceptToken<IProtectedKeywordToken>() is { } protectedToken)
            return AccessModifierSyntax.PublishedProtected(publishedToken, protectedToken);
        return AccessModifierSyntax.Published(publishedToken);
    }

    public IConstKeywordToken? ParseConstModifier() => Tokens.AcceptToken<IConstKeywordToken>();

    public IMoveKeywordToken? ParseMoveModifier() => Tokens.AcceptToken<IMoveKeywordToken>();

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
