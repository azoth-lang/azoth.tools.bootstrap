using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Tokens;

[Closed(
    typeof(IPublishedKeywordToken),
    typeof(IPackageAccessModifierToken),
    typeof(IConstKeywordToken),
    typeof(IMoveKeywordToken),
    typeof(ISafeKeywordToken),
    typeof(IUnsafeKeywordToken),
    typeof(IAbstractKeywordToken))]
public partial interface IModifierToken : IKeywordToken;

public partial interface IPublishedKeywordToken : IModifierToken;
public partial interface IPackageAccessModifierToken : IModifierToken;
public partial interface IConstKeywordToken : IModifierToken;
public partial interface IMoveKeywordToken : IModifierToken;
public partial interface ISafeKeywordToken : IModifierToken;
public partial interface IUnsafeKeywordToken : IModifierToken;
public partial interface IAbstractKeywordToken : IModifierToken;
