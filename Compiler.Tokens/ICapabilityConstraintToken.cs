using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Tokens;

[Closed(
    typeof(IReadableKeywordToken),
    typeof(IShareableKeywordToken),
    typeof(IAliasableKeywordToken),
    typeof(ISendableKeywordToken),
    typeof(IAnyKeywordToken))]
public interface ICapabilityConstraintToken : IKeywordToken { }

public partial interface IReadableKeywordToken : ICapabilityConstraintToken { }
public partial interface IShareableKeywordToken : ICapabilityConstraintToken { }
public partial interface IAliasableKeywordToken : ICapabilityConstraintToken { }
public partial interface ISendableKeywordToken : ICapabilityConstraintToken { }
public partial interface IAnyKeywordToken : ICapabilityConstraintToken { }
