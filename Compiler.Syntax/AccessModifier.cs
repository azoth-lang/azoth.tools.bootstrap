using System;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Syntax;

[Flags]
public enum AccessModifier
{
    Private = 0,
    Protected = 0b001,
    Public = 0b011,
    PublishedProtected = 0b101,
    Published = 0b111,
}

public static class AccessModifierExtensions
{
    // TODO this doesn't work right because `published protected` is two tokens
    public static AccessModifier ToAccessModifier(this IAccessModifierToken? accessModifier)
    {
        return accessModifier switch
        {
            null => AccessModifier.Private,
            IPublicKeywordToken _ => AccessModifier.Public,
            IPublishedKeywordToken _ => AccessModifier.Published,
            IProtectedKeywordToken _ => AccessModifier.Public,
            _ => throw ExhaustiveMatch.Failed(accessModifier)
        };
    }

    public static string ToSourceString(this AccessModifier accessModifier)
    {
        return accessModifier switch
        {
            AccessModifier.Private => "",
            AccessModifier.Protected => "protected",
            AccessModifier.Public => "public",
            AccessModifier.PublishedProtected => "published protected",
            AccessModifier.Published => "published",
            _ => throw ExhaustiveMatch.Failed(accessModifier)
        };
    }
}
