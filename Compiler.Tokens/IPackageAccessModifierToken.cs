using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Tokens;

/// <summary>
/// An access modifier that controls access within the packages that the target can be accessed in.
/// </summary>
/// <remarks><see cref="IProtectedKeywordToken"/> is not a package access modifier because it allows
/// other packages to access the target/.</remarks>
[Closed(
    typeof(IPublicKeywordToken),
    typeof(IProtectedKeywordToken))]
public partial interface IPackageAccessModifierToken : IKeywordToken;

public partial interface IPublicKeywordToken : IPackageAccessModifierToken;
public partial interface IProtectedKeywordToken : IPackageAccessModifierToken;
