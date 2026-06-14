using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Tokens;

[Closed(
    typeof(IBindingToken),
    typeof(IModifierToken),
    typeof(ITypeKindKeywordToken),
    typeof(IBooleanLiteralToken),
    typeof(ICapabilityToken),
    typeof(ICapabilitySetToken),
    typeof(IOverridesOrHidesToken))]
public partial interface IKeywordToken : IEssentialToken;
