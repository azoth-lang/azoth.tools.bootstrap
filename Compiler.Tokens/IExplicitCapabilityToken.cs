using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Tokens;

[Closed(typeof(IReadKeywordToken))]
public interface IExplicitCapabilityToken : ICapabilityToken;

public partial interface IReadKeywordToken : IExplicitCapabilityToken;
