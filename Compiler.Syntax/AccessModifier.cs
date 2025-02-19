using System;
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
