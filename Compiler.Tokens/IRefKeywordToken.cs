using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Tokens;

[Closed(
    typeof(IVariableRefKeywordToken),
    typeof(IInternalRefKeywordToken))]
public interface IRefKeywordToken : IEssentialToken;

public partial interface IVariableRefKeywordToken : IRefKeywordToken;
public partial interface IInternalRefKeywordToken : IRefKeywordToken;
