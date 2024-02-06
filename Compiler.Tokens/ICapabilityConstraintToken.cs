using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Tokens;

[Closed(typeof(IReadableKeywordToken))]
public interface ICapabilityConstraintToken : IKeywordToken { }

public partial interface IReadableKeywordToken : ICapabilityConstraintToken { }
