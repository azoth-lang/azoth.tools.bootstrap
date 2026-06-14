using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Tokens;

[Closed(typeof(IOverridesKeywordToken), typeof(IHidesKeywordToken))]
public interface IOverridesOrHidesToken : IKeywordToken;

public partial interface IOverridesKeywordToken : IOverridesOrHidesToken;
public partial interface IHidesKeywordToken : IOverridesOrHidesToken;
