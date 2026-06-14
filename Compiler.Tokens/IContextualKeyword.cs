using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Tokens;

/// <summary>
/// A token that in some contexts is an identifier and in other contexts is a keyword.
/// </summary>
[Closed(
    typeof(ITypeKindKeywordToken),
    typeof(IAccessorKeywordToken))]
public partial interface IContextualKeyword : IKeywordToken;

public partial interface ITypeKindKeywordToken : IContextualKeyword;
public partial interface IAccessorKeywordToken : IContextualKeyword;
