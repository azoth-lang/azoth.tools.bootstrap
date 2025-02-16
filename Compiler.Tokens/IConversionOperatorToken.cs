using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Tokens;

[Closed(
    typeof(IAsKeywordToken),
    typeof(IAsExclamationKeywordToken),
    typeof(IAsQuestionKeywordToken))]
public interface IConversionOperatorToken : IOperatorToken, IEssentialToken;

public partial interface IAsKeywordToken : IConversionOperatorToken;
public partial interface IAsExclamationKeywordToken : IConversionOperatorToken;
public partial interface IAsQuestionKeywordToken : IConversionOperatorToken;
