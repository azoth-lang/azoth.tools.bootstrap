using System;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Lexing;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing;

/// <summary>
/// * Required: If the current token is of the given type consume it, otherwise leave it, but
///   add a compiler error that an expected token is missing AND throw a <see cref="ParseFailedException"/>.
/// * Expected: If the current token is of the given type consume it, otherwise leave it, but
///   add a compiler error that an expected token is missing.
/// * Accept: If the current token is of the given type consume it, otherwise leave it.
/// * Consume: Consume a token of the given type, throws <see cref="InvalidOperationException"/> if
///   the token is not of the given type.
/// </summary>
public static class TokenIteratorExtensions
{
    #region Required
    public static TextSpan Required<T>(this ITokenIterator<IToken> tokens)
        where T : IToken
    {
        if (tokens.Current is T token)
        {
            tokens.Next();
            return token.Span;
        }

        tokens.Context.Diagnostics.Add(
            ParseError.MissingToken(tokens.Context.File, typeof(T), tokens.Current));
        throw new ParseFailedException($"Requires {typeof(T).GetFriendlyName()}, found {tokens.Current.GetType().GetFriendlyName()}");
    }

    public static T RequiredToken<T>(this ITokenIterator<IToken> tokens)
        where T : IToken
    {
        if (tokens.Current is T token)
        {
            tokens.Next();
            return token;
        }

        tokens.Context.Diagnostics.Add(
            ParseError.MissingToken(tokens.Context.File, typeof(T), tokens.Current));
        throw new ParseFailedException($"Requires {typeof(T).GetFriendlyName()}, found {tokens.Current.GetType().GetFriendlyName()}");
    }
    #endregion

    #region Accept
    /// <summary>
    /// If the current token is of the type <typeparamref name="T"/> consume it, otherwise leave it.
    /// </summary>
    public static bool Accept<T>(this ITokenIterator<IToken> tokens)
        where T : class, IToken
    {
        if (tokens.Current is T)
        {
            tokens.Next();
            return true;
        }

        return false;
    }

    /// <summary>
    /// If the current token is of the type <typeparamref name="T"/> consume and return it,
    /// otherwise leave it.
    /// </summary>
    public static T? AcceptToken<T>(this ITokenIterator<IToken> tokens)
        where T : class, IToken
    {
        if (tokens.Current is T token)
        {
            tokens.Next();
            return token;
        }

        return null;
    }
    #endregion

    #region Expect
    public static TextSpan Expect<T>(this ITokenIterator<IToken> tokens)
        where T : IToken
    {
        if (tokens.Current is T token)
        {
            tokens.Next();
            return token.Span;
        }

        tokens.Context.Diagnostics.Add(
            ParseError.MissingToken(tokens.Context.File, typeof(T), tokens.Current));
        // An empty span at the current location
        return new TextSpan(tokens.Current.Span.Start, 0);
    }

    public static (T?, TextSpan) ExpectToken<T>(this ITokenIterator<IToken> tokens)
        where T : class, IToken
    {
        if (tokens.Current is T token)
        {
            tokens.Next();
            return (token, token.Span);
        }

        tokens.Context.Diagnostics.Add(ParseError.MissingToken(tokens.Context.File, typeof(T), tokens.Current));
        return (null, new TextSpan(tokens.Current.Span.Start, 0));
    }
    #endregion

    #region Consume
    /// <summary>
    /// Consume a token of the given type returning the text span of the consumed token.
    /// </summary>
    /// <returns>The text span of the consumed token.</returns>
    /// <exception cref="InvalidOperationException">The next token is not of the given type.</exception>
    /// <remarks>Use when the next token is known to be of the given type and it is a bug if it isn't.</remarks>
    public static TextSpan Consume<T>(this ITokenIterator<IToken> tokens)
        where T : IToken
    {
        if (tokens.Current is T token)
        {
            tokens.Next();
            return token.Span;
        }

        throw new InvalidOperationException($"Requires {typeof(T).GetFriendlyName()}, found {tokens.Current.GetType().GetFriendlyName()}");
    }

    /// <summary>
    /// Consume a token of the given type.
    /// </summary>
    /// <returns>The consumed token.</returns>
    /// <exception cref="InvalidOperationException">The next token is not of the given type.</exception>
    /// <remarks>Use when the next token is known to be of the given type and it is a bug if it isn't.</remarks>
    public static T ConsumeToken<T>(this ITokenIterator<IToken> tokens)
        where T : class, IToken
    {
        if (tokens.Current is T token)
        {
            tokens.Next();
            return token;
        }

        throw new InvalidOperationException($"Requires {typeof(T).GetFriendlyName()}, found {tokens.Current.GetType().GetFriendlyName()}");
    }
    #endregion

    /// <summary>
    /// The current token is unexpected, report an error and consume it.
    /// </summary>
    public static TextSpan UnexpectedToken(this ITokenIterator<IToken> tokens)
    {
        // TODO shouldn't we ignore or combine unexpected token errors until we parse something successfully?
        var span = tokens.Current.Span;
        tokens.Context.Diagnostics.Add(ParseError.UnexpectedToken(tokens.Context.File, span));
        tokens.Next();
        return span;
    }

    public static bool AtEnd<T>(this ITokenIterator<IToken> tokens)
        where T : IToken
    {
        switch (tokens.Current)
        {
            case T _:
            case IEndOfFileToken _:
                return true;
            default:
                return false;
        }
    }
}
