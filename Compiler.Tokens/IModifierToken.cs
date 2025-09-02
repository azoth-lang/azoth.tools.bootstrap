using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Tokens;

[Closed(
    typeof(IAbstractKeywordToken),
    typeof(IConstKeywordToken),
    typeof(IDropKeywordToken),
    typeof(IMoveKeywordToken),
    typeof(IPackageAccessModifierToken),
    typeof(IPublishedKeywordToken),
    typeof(ISafeKeywordToken),
    typeof(IUnsafeKeywordToken))]
public partial interface IModifierToken : IKeywordToken;

public partial interface IAbstractKeywordToken : IModifierToken;
public partial interface IConstKeywordToken : IModifierToken;
public partial interface IDropKeywordToken : IModifierToken;
public partial interface IMoveKeywordToken : IModifierToken;
public partial interface IPackageAccessModifierToken : IModifierToken;
public partial interface IPublishedKeywordToken : IModifierToken;
public partial interface ISafeKeywordToken : IModifierToken;
public partial interface IUnsafeKeywordToken : IModifierToken;
