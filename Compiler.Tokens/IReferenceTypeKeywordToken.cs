using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Tokens;

[Closed(
    typeof(IVariableRefKeywordToken),
    typeof(IInternalRefKeywordToken))]
public interface IReferenceTypeKeywordToken : IEssentialToken { }

public partial interface IVariableRefKeywordToken : IReferenceTypeKeywordToken { }
public partial interface IInternalRefKeywordToken : IReferenceTypeKeywordToken { }
