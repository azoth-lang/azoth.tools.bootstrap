using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Tokens;

[Closed(
    typeof(IInKeywordToken),
    typeof(IOutKeywordToken))]
public interface IVarianceToken : IEssentialToken;

public partial interface IInKeywordToken : IVarianceToken;
public partial interface IOutKeywordToken : IVarianceToken;
