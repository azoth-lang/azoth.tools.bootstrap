using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Tokens;

[Closed(
    typeof(IStandardCapabilityToken),
    typeof(IExplicitCapabilityToken))]
public interface ICapabilityToken : IKeywordToken;
