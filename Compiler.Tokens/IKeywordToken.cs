using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Tokens;

[Closed(
    typeof(IBindingToken),
    typeof(IModifierToken),
    typeof(ITypeKindKeywordToken),
    typeof(IBooleanLiteralToken),
    typeof(ICapabilityToken),
    typeof(ICapabilitySetToken))]
public partial interface IKeywordToken : IEssentialToken;
