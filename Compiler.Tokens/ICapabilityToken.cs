using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Tokens
{
    [Closed(
        typeof(IIsolatedKeywordToken),
        typeof(ISharedKeywordToken),
        typeof(ILentKeywordToken),
        typeof(IConstKeywordToken),
        typeof(IMutableKeywordToken),
        typeof(IIdKeywordToken))]
    public interface ICapabilityToken : IKeywordToken { }

    public partial interface IIsolatedKeywordToken : ICapabilityToken { }
    public partial interface ISharedKeywordToken : ICapabilityToken { }
    public partial interface ILentKeywordToken : ICapabilityToken { }
    public partial interface IConstKeywordToken : ICapabilityToken { }
    public partial interface IMutableKeywordToken : ICapabilityToken { }
    public partial interface IIdKeywordToken : ICapabilityToken { }
}
