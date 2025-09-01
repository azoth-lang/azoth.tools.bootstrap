using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Tokens;

[Closed(typeof(IGetKeywordToken), typeof(ISetKeywordToken))]
public partial interface IAccessorKeywordToken;

public partial interface IGetKeywordToken : IAccessorKeywordToken;
public partial interface ISetKeywordToken : IAccessorKeywordToken;
