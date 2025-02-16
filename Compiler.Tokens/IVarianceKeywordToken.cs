using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Tokens;

[Closed(
    typeof(IInKeywordToken),
    typeof(IOutKeywordToken))]
public interface IVarianceToken;

public partial interface IInKeywordToken : IVarianceToken;
public partial interface IOutKeywordToken : IVarianceToken;
