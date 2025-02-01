using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Tokens;

[Closed(
    typeof(IIRefKeywordToken))]
public interface IReferenceTypeKeywordToken : IEssentialToken { }

public partial interface IIRefKeywordToken : IReferenceTypeKeywordToken { }
//public partial interface IRefKeywordToken : IReferenceTypeKeywordToken { }
