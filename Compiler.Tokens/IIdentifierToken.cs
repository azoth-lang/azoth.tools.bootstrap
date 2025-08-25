using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Tokens;

[Closed(
    typeof(IBareIdentifierToken),
    typeof(IEscapedIdentifierToken),
    typeof(IIdentifierStringToken),
    typeof(ITypeKindKeywordToken))]
public partial interface IIdentifierToken;

public partial interface IBareIdentifierToken : IIdentifierToken;
public partial interface IEscapedIdentifierToken : IIdentifierToken;
public partial interface IIdentifierStringToken : IIdentifierToken;
public partial interface ITypeKindKeywordToken : IIdentifierToken;
