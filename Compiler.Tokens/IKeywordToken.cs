using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Tokens
{
    [Closed(
        typeof(IMemberNameToken),
        typeof(IBindingToken),
        typeof(IModiferToken),
        typeof(IAccessModifierToken),
        typeof(ITypeKindKeywordToken),
        typeof(IBooleanLiteralToken),
        typeof(ICapabilityToken))]
    public partial interface IKeywordToken : IEssentialToken { }
}
