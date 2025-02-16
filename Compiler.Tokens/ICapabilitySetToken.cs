using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Tokens;

[Closed(
    typeof(IReadableKeywordToken),
    typeof(IShareableKeywordToken),
    typeof(IAliasableKeywordToken),
    typeof(ISendableKeywordToken),
    typeof(ITemporaryKeywordToken),
    typeof(IAnyKeywordToken))]
public interface ICapabilitySetToken : IKeywordToken;

public partial interface IReadableKeywordToken : ICapabilitySetToken;
public partial interface IShareableKeywordToken : ICapabilitySetToken;
public partial interface IAliasableKeywordToken : ICapabilitySetToken;
public partial interface ISendableKeywordToken : ICapabilitySetToken;
public partial interface ITemporaryKeywordToken : ICapabilitySetToken;
public partial interface IAnyKeywordToken : ICapabilitySetToken;
