using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Tokens;

/// <summary>
/// A token that isn't trivia
/// </summary>
[Closed(
    typeof(IStringLiteralToken),
    typeof(IKeywordToken),
    typeof(IIdentifierOrUnderscoreToken),
    typeof(IOperatorToken),
    typeof(ILiteralToken),
    typeof(IBuiltInTypeToken),
    typeof(IBinaryOperatorToken),
    typeof(IPunctuationToken),
    typeof(IEndOfFileToken),
    typeof(IRefKeywordToken),
    typeof(IConversionOperatorToken))]
public partial interface IEssentialToken : IToken;
