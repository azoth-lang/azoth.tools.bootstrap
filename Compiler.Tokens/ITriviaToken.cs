using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Tokens;

[Closed(
    typeof(ICommentToken),
    typeof(IWhitespaceToken),
    typeof(IUnexpectedToken))]
public interface ITriviaToken : IToken;

public interface ICommentToken : ITriviaToken;
public interface IWhitespaceToken : ITriviaToken;
public interface IUnexpectedToken : ITriviaToken;


internal class WhitespaceToken : Token, IWhitespaceToken
{
    public WhitespaceToken(TextSpan span)
        : base(span) { }
}

internal class CommentToken : Token, ICommentToken
{
    public CommentToken(TextSpan span)
        : base(span) { }
}

internal class UnexpectedToken : Token, IUnexpectedToken
{
    public UnexpectedToken(TextSpan span)
        : base(span) { }
}

public static partial class TokenFactory
{
    public static IWhitespaceToken Whitespace(TextSpan span) => new WhitespaceToken(span);

    public static ICommentToken Comment(TextSpan span) => new CommentToken(span);

    public static IUnexpectedToken Unexpected(TextSpan span) => new UnexpectedToken(span);
}
