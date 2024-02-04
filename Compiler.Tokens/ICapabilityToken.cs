using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Tokens;

[Closed(
    typeof(IIsolatedKeywordToken),
    typeof(IConstKeywordToken),
    typeof(IMutableKeywordToken),
    typeof(IIdKeywordToken),
    typeof(ITempKeywordToken))]
public interface ICapabilityToken : IKeywordToken { }

public partial interface IIsolatedKeywordToken : ICapabilityToken { }
public partial interface IConstKeywordToken : ICapabilityToken { }
public partial interface IMutableKeywordToken : ICapabilityToken { }
public partial interface IIdKeywordToken : ICapabilityToken { }
public partial interface ITempKeywordToken : ICapabilityToken { }
