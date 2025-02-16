using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Tokens;

[Closed(
    typeof(IGoKeywordToken),
    typeof(IDoKeywordToken))]
public interface IAsyncStartOperatorToken : IOperatorToken;

public partial interface IGoKeywordToken : IAsyncStartOperatorToken;
public partial interface IDoKeywordToken : IAsyncStartOperatorToken;
