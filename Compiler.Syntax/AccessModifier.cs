using Azoth.Tools.Bootstrap.Compiler.Tokens;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Syntax;

public enum AccessModifier
{
    Private = 1,
    Public,
    Published,
}

public static class AccessModifierExtensions
{
    public static AccessModifier ToAccessModifier(this IAccessModifierToken? accessModifier)
    {
        return accessModifier switch
        {
            null => AccessModifier.Private,
            IPublicKeywordToken _ => AccessModifier.Public,
            IPublishedKeywordToken _ => AccessModifier.Published,
            _ => throw ExhaustiveMatch.Failed(accessModifier)
        };
    }

    public static string ToSourceString(this AccessModifier accessModifier)
    {
        return accessModifier switch
        {
            AccessModifier.Private => "",
            AccessModifier.Public => "public",
            AccessModifier.Published => "published",
            _ => throw ExhaustiveMatch.Failed(accessModifier)
        };
    }
}
