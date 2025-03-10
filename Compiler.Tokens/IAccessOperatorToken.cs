using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Tokens;

[Closed(
    typeof(IDotToken),
    typeof(IQuestionDotToken))]
public interface IAccessOperatorToken : IOperatorToken;

public partial interface IDotToken : IAccessOperatorToken;
public partial interface IQuestionDotToken : IAccessOperatorToken;
