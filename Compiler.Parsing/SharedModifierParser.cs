using Azoth.Tools.Bootstrap.Compiler.Lexing;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Tokens;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing;

/// <summary>
/// Modifer related parsing methods that are needed by both the <see cref="ModifierParser"/> and the
/// main <see cref="Parser"/>.
/// </summary>
public abstract class SharedModifierParser : RecursiveDescentParser
{
    protected SharedModifierParser(ITokenIterator<IEssentialToken> tokens)
        : base(tokens) { }

    public AccessModifierSyntax? AcceptAccessModifier()
    {
        if (Tokens.AcceptToken<IPublishedKeywordToken>() is { } publishedToken)
        {
            if (Tokens.AcceptToken<IProtectedKeywordToken>() is { } protectedToken)
                return AccessModifierSyntax.PublishedProtected(publishedToken, protectedToken);
            return AccessModifierSyntax.Published(publishedToken);
        }

        if (Tokens.AcceptToken<IPackageAccessModifierToken>() is { } packageAccessModifier)
            return AccessModifierSyntax.Package(packageAccessModifier);
        return null;
    }
}
