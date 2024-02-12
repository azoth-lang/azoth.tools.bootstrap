using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Tokens;

[Closed(
    typeof(IIsolatedKeywordToken),
    typeof(IConstKeywordToken),
    typeof(IMutableKeywordToken),
    typeof(IIdKeywordToken),
    typeof(ITempKeywordToken))]
public interface IStandardCapabilityToken : ICapabilityToken { }

public partial interface IIsolatedKeywordToken : IStandardCapabilityToken { }
public partial interface IConstKeywordToken : IStandardCapabilityToken { }
public partial interface IMutableKeywordToken : IStandardCapabilityToken { }
public partial interface IIdKeywordToken : IStandardCapabilityToken { }
public partial interface ITempKeywordToken : IStandardCapabilityToken { }
