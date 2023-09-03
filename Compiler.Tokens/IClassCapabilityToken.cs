using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Tokens
{
    [Closed(
        typeof(IConstKeywordToken),
        typeof(IMutableKeywordToken),
        typeof(IIsolatedKeywordToken))]
    public interface IClassCapabilityToken : IKeywordToken { }

    public partial interface IConstKeywordToken : IClassCapabilityToken { }

    public partial interface IMutableKeywordToken : IClassCapabilityToken { }

    public partial interface IIsolatedKeywordToken : IClassCapabilityToken { }
}
