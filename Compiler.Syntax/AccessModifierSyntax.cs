using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Tokens;

namespace Azoth.Tools.Bootstrap.Compiler.Syntax;

public readonly struct AccessModifierSyntax
{
    public static readonly AccessModifierSyntax Private = default;
    public static AccessModifierSyntax Published(IPublishedKeywordToken publishedToken)
        => new(publishedToken, null);
    public static AccessModifierSyntax PublishedProtected(IPublishedKeywordToken publishedToken, IProtectedKeywordToken protectedToken)
        => new(publishedToken, protectedToken);
    public static AccessModifierSyntax Package(IPackageAccessModifierToken? packageAccessModifierToken)
        => new(null, packageAccessModifierToken);

    public readonly IPublishedKeywordToken? PublishedToken;
    public readonly IPackageAccessModifierToken? PackageAccessModifierToken;

    public TextSpan? Span
    {
        [DebuggerStepThrough]
        get => TextSpan.Covering(PublishedToken?.Span, PackageAccessModifierToken?.Span);
    }

    private AccessModifierSyntax(IPublishedKeywordToken? published, IPackageAccessModifierToken? packageAccess)
    {
        PublishedToken = published;
        PackageAccessModifierToken = packageAccess;
    }

    public AccessModifier ToAccessModifier()
    {
        return (PublishedToken, PackageAccessModifierToken) switch
        {
            (null, null) => AccessModifier.Private,
            (null, IProtectedKeywordToken) => AccessModifier.Protected,
            (null, IPublicKeywordToken) => AccessModifier.Public,
            (IPublishedKeywordToken, IProtectedKeywordToken) => AccessModifier.PublishedProtected,
            (IPublishedKeywordToken, null) => AccessModifier.Published,
            _ => throw new UnreachableException($"Cannot construct an {nameof(AccessModifierSyntax)} that doesn't match these cases."),
        };
    }

    public override string ToString() => ToAccessModifier().ToSourceString();
}
