using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Tokens;

[Closed(
    typeof(IGoKeywordToken),
    typeof(IDoKeywordToken))]
public interface IAsyncOperatorToken : IOperatorToken { }

public partial interface IGoKeywordToken : IAsyncOperatorToken { }
public partial interface IDoKeywordToken : IAsyncOperatorToken { }
