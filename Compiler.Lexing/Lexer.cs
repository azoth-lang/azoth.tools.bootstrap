using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Tokens;

namespace Azoth.Tools.Bootstrap.Compiler.Lexing;

public class Lexer
{
    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Unit testability")]
    public ITokenIterator<IToken> Lex(ParseContext context)
        => new TokenIterator<IToken>(context, Lex(context.File, context.Diagnostics));

    private static IEnumerable<IToken> Lex(CodeFile file, DiagnosticCollectionBuilder diagnostics)
    {
        var code = file.Code;
        var text = code.Text;
        var tokenStart = 0;
        var tokenEnd = -1; // One past the end position to allow for zero length spans
        while (tokenStart < text.Length)
        {
            var currentChar = text[tokenStart];
            switch (currentChar)
            {
                case '{':
                    yield return TokenFactory.OpenBrace(SymbolSpan());
                    break;
                case '}':
                    yield return TokenFactory.CloseBrace(SymbolSpan());
                    break;
                case '(':
                    yield return TokenFactory.OpenParen(SymbolSpan());
                    break;
                case ')':
                    yield return TokenFactory.CloseParen(SymbolSpan());
                    break;
                case '[':
                    yield return TokenFactory.OpenBracket(SymbolSpan());
                    break;
                case ']':
                    yield return TokenFactory.CloseBracket(SymbolSpan());
                    break;
                case '|':
                    if (NextChar() is '>')
                        // it is `|>`
                        yield return TokenFactory.RightTriangle(SymbolSpan(2));
                    else
                        // it is `|`
                        yield return NewReservedOperator();
                    break;
                case '@':
                    if (NextChar() is '=')
                        switch (CharAt(2))
                        {
                            case '=':
                                // it is `@==`
                                yield return TokenFactory.ReferenceEquals(SymbolSpan(3));
                                break;
                            case '/':
                                if (CharAt(3) is '=')
                                    // it is `@=/=`
                                    yield return TokenFactory.NotReferenceEquals(SymbolSpan(4));
                                else
                                    goto default;
                                break;
                            default:
                                // it is `@=` but not `@==` or `@=/=`, lex as `@` ...
                                yield return NewReservedOperator();
                                break;
                        }
                    else
                        yield return NewReservedOperator();
                    break;
                case '&':
                case '`':
                case '$':
                    yield return NewReservedOperator();
                    break;
                case ';':
                    yield return TokenFactory.Semicolon(SymbolSpan());
                    break;
                case ',':
                    yield return TokenFactory.Comma(SymbolSpan());
                    break;
                case '#':
                    yield return NextChar() switch
                    {
                        // it is `##`
                        '#' => NewReservedOperator(2),
                        // it is `#(`
                        '(' => NewReservedOperator(2),
                        // it is `#[`
                        '[' => NewReservedOperator(2),
                        // it is `#{`
                        '{' => NewReservedOperator(2),
                        // it is `#`
                        _ => TokenFactory.Hash(SymbolSpan()),
                    };
                    break;
                case '.':
                    if (NextChar() is '.')
                    {
                        if (CharAt(2) is '<')
                            // it is `..<`
                            yield return TokenFactory.DotDotLessThan(SymbolSpan(3));
                        else
                            // it is `..`
                            yield return TokenFactory.DotDot(SymbolSpan(2));
                    }
                    else
                        yield return TokenFactory.Dot(SymbolSpan());
                    break;
                case ':':
                    if (NextChar() is ':')
                    {
                        if (CharAt(2) is '.')
                            yield return TokenFactory.ColonColonDot(SymbolSpan(3));
                        else
                            yield return TokenFactory.ColonColon(SymbolSpan(2));
                    }
                    else
                        // it is `:`
                        yield return TokenFactory.Colon(SymbolSpan());
                    break;
                case '?':
                    yield return NextChar() switch
                    {
                        // it is `??`
                        '?' => TokenFactory.QuestionQuestion(SymbolSpan(2)),
                        // it is `?.`
                        '.' => TokenFactory.QuestionDot(SymbolSpan(2)),
                        // it is `?`
                        _ => TokenFactory.Question(SymbolSpan())
                    };
                    break;
                case '^':
                    if (NextChar() is '.')
                        // it is `^.`
                        yield return NewReservedOperator(2);
                    else
                        // it is `^`
                        yield return NewReservedOperator();
                    break;
                case '+':
                    if (NextChar() is '=')
                        // it is `+=`
                        yield return TokenFactory.PlusEquals(SymbolSpan(2));
                    else
                        // it is `+`
                        yield return TokenFactory.Plus(SymbolSpan());
                    break;
                case '-':
                    yield return NextChar() switch
                    {
                        // it is `-=`
                        '=' => TokenFactory.MinusEquals(SymbolSpan(2)),
                        // it is `->`
                        '>' => TokenFactory.RightArrow(SymbolSpan(2)),
                        // it is `-`
                        _ => TokenFactory.Minus(SymbolSpan())
                    };
                    break;
                case '*':
                    if (NextChar() is '=')
                        // it is `*=`
                        yield return TokenFactory.AsteriskEquals(SymbolSpan(2));
                    else
                        // it is `*`
                        yield return TokenFactory.Asterisk(SymbolSpan());

                    break;
                case '/':
                    switch (NextChar())
                    {
                        case '/':
                            // it is a line comment `//`
                            tokenEnd = tokenStart + 2;
                            // Include newline at end
                            while (tokenEnd < text.Length)
                            {
                                currentChar = text[tokenEnd];
                                tokenEnd += 1;
                                if (currentChar is '\r' or '\n')
                                    break;
                            }

                            yield return TokenFactory.Comment(TokenSpan());
                            break;
                        case '*':
                            // it is a block comment `/*`
                            tokenEnd = tokenStart + 2;
                            var lastCharWasStar = false;
                            // Include slash at end
                            for (; ; )
                            {
                                // If we ran into the end of the file, error
                                if (tokenEnd >= text.Length)
                                {
                                    diagnostics.Add(LexError.UnclosedBlockComment(file,
                                        TextSpan.FromStartEnd(tokenStart, tokenEnd)));
                                    break;
                                }
                                currentChar = text[tokenEnd];
                                tokenEnd += 1;
                                if (lastCharWasStar && currentChar == '/')
                                    break;
                                lastCharWasStar = currentChar == '*';
                            }

                            yield return TokenFactory.Comment(TokenSpan());
                            break;
                        case '=':
                            // it is `/=`
                            yield return TokenFactory.SlashEquals(SymbolSpan(2));
                            break;
                        default:
                            // it is `/`
                            yield return TokenFactory.Slash(SymbolSpan());
                            break;
                    }
                    break;
                case '=':
                    switch (NextChar())
                    {
                        case '>':
                            // it is `=>`
                            yield return TokenFactory.RightDoubleArrow(SymbolSpan(2));
                            break;
                        case '=':
                            // it is `==`
                            yield return TokenFactory.EqualsEquals(SymbolSpan(2));
                            break;
                        case '/':
                            if (CharAt(2) is '=')
                                // it is `=/=`
                                yield return TokenFactory.NotEqual(SymbolSpan(3));
                            else
                                goto default;
                            break;
                        default:
                            // it is `=`
                            yield return TokenFactory.Equals(SymbolSpan());
                            break;
                    }
                    break;
                case '!' when NextChar() is '=':
                {
                    var span = SymbolSpan(2);
                    diagnostics.Add(LexError.CStyleNotEquals(file, span));
                    yield return TokenFactory.NotEqual(span);
                    break;
                }
                case '>':
                    if (NextChar() is '=')
                        // it is `>=`
                        yield return TokenFactory.GreaterThanOrEqual(SymbolSpan(2));
                    else
                        // it is `>`
                        yield return TokenFactory.GreaterThan(SymbolSpan());
                    break;
                case '<':
                    switch (NextChar())
                    {
                        case '=':
                            // it is `<=`
                            yield return TokenFactory.LessThanOrEqual(SymbolSpan(2));
                            break;
                        case ':':
                            // it is `<:`
                            yield return TokenFactory.LessThanColon(SymbolSpan(2));
                            break;
                        case '.':
                            if (CharAt(2) is '.')
                            {
                                if (CharAt(3) is '<')
                                    // it is `<..<`
                                    yield return TokenFactory.LessThanDotDotLessThan(SymbolSpan(4));
                                else
                                    // it is `<..`
                                    yield return TokenFactory.LessThanDotDot(SymbolSpan(3));
                            }
                            else
                                goto default;
                            break;
                        default:
                            // it is `<`
                            yield return TokenFactory.LessThan(SymbolSpan());
                            break;
                    }
                    break;
                case '"':
                    yield return LexString();
                    break;
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                {
                    tokenEnd = tokenStart + 1;
                    while (tokenEnd < text.Length && IsIntegerCharacter(text[tokenEnd]))
                        tokenEnd += 1;

                    var span = TokenSpan();
                    var value = BigInteger.Parse(code[span], CultureInfo.InvariantCulture);
                    yield return TokenFactory.IntegerLiteral(span, value);
                    break;
                }
                case '\\':
                {
                    if (NextChar() is '"')
                        yield return LexStringIdentifier();
                    else
                    {
                        tokenEnd = tokenStart + 1;
                        while (tokenEnd < text.Length && IsIdentifierCharacter(text[tokenEnd]))
                            tokenEnd += 1;

                        if (tokenEnd == tokenStart + 1)
                            yield return NewUnexpectedCharacter();
                        else
                            yield return NewEscapedIdentifier();
                    }
                    break;
                }
                default:
                    if (char.IsWhiteSpace(currentChar))
                    {
                        tokenEnd = tokenStart + 1;
                        // Include whitespace at end
                        while (tokenEnd < text.Length && char.IsWhiteSpace(text[tokenEnd]))
                            tokenEnd += 1;

                        yield return TokenFactory.Whitespace(TokenSpan());
                    }
                    else if (IsIdentifierStartCharacter(currentChar))
                    {
                        tokenEnd = tokenStart + 1;
                        while (tokenEnd < text.Length && IsIdentifierCharacter(text[tokenEnd]))
                            tokenEnd += 1;

                        yield return NewIdentifierOrKeywordToken();
                    }
                    else
                        yield return NewUnexpectedCharacter();
                    break;
            }
            tokenStart = tokenEnd;
        }

        // The end of file token provides something to attach any final errors to
        yield return TokenFactory.EndOfFile(SymbolSpan(0));
        yield break;

        TextSpan SymbolSpan(int length = 1)
        {
            var end = tokenStart + length;
            return TokenSpan(end);
        }
        TextSpan TokenSpan(int? end = null)
        {
            tokenEnd = end ?? tokenEnd;
            return TextSpan.FromStartEnd(tokenStart, tokenEnd);
        }

        IToken NewIdentifierOrKeywordToken()
        {
            var span = TokenSpan();
            var value = code[span];

            // Check for keywords with ! or ? after
            var nextChar = tokenEnd < text.Length ? text[tokenEnd] : '\0';
            if (nextChar is '?' or '!')
            {
                var extendedValue = value + nextChar;
                if (TokenTypes.KeywordFactories.TryGetValue(extendedValue, out var extendedKeywordFactory))
                {
                    tokenEnd += 1;
                    return extendedKeywordFactory(TokenSpan());
                }
            }

            // Standard keywords
            if (TokenTypes.KeywordFactories.TryGetValue(value, out var keywordFactory))
                return keywordFactory(span);

            // Reserved words
            if (value == "uint8")
            {
                diagnostics.Add(LexError.UInt8InsteadOfByte(file, span));
                return TokenTypes.KeywordFactories["byte"](span);
            }
            if (value == "continue")
                diagnostics.Add(LexError.ContinueInsteadOfNext(file, span));
            else if (TokenTypes.ReservedWords.Contains(value)
                     || TokenTypes.IsReservedTypeName(value))
                diagnostics.Add(LexError.ReservedWord(file, span, value));

            return TokenFactory.BareIdentifier(span, value);
        }
        IToken NewEscapedIdentifier()
        {
            var identifierStart = tokenStart + 1;
            var span = TokenSpan();
            var value = text[identifierStart..tokenEnd];
            var isValidToEscape = TokenTypes.Keywords.Contains(value)
                                  || TokenTypes.ReservedWords.Contains(value)
                                  || TokenTypes.IsReservedTypeName(value)
                                  || char.IsDigit(value[0]);
            if (!isValidToEscape)
                diagnostics.Add(LexError.EscapedIdentifierShouldNotBeEscaped(file, span, value));
            return TokenFactory.EscapedIdentifier(span, value);
        }
        IToken NewUnexpectedCharacter()
        {
            var span = SymbolSpan();
            var value = code[span];
            diagnostics.Add(LexError.UnexpectedCharacter(file, span, value[0]));
            return TokenFactory.Unexpected(span);
        }
        IToken NewReservedOperator(int length = 1)
        {
            var span = SymbolSpan(length);
            var value = code[span];
            diagnostics.Add(LexError.ReservedOperator(file, span, value));
            return TokenFactory.Unexpected(span);
        }

        char? NextChar()
        {
            var index = tokenStart + 1;
            return index < text.Length ? text[index] : default;
        }

        char? CharAt(int offset)
        {
            var index = tokenStart + offset;
            return index < text.Length ? text[index] : default;
        }

        IStringLiteralToken LexString()
        {
            var content = LexStringLike(1, out var isUnclosed);
            var span = TextSpan.FromStartEnd(tokenStart, tokenEnd);
            if (isUnclosed)
                diagnostics.Add(LexError.UnclosedStringLiteral(file, span));

            return TokenFactory.StringLiteral(span, content);
        }

        IIdentifierStringToken LexStringIdentifier()
        {
            var content = LexStringLike(2, out var isUnclosed);
            var span = TextSpan.FromStartEnd(tokenStart, tokenEnd);
            if (isUnclosed)
                diagnostics.Add(LexError.UnclosedStringIdentifier(file, span));

            return TokenFactory.IdentifierString(span, content);
        }

        string LexStringLike(int startOffset, out bool isUnclosed)
        {
            tokenEnd = tokenStart + startOffset;
            var content = new StringBuilder();
            char currentChar;
            while (tokenEnd < text.Length && (currentChar = text[tokenEnd]) != '"')
            {
                tokenEnd += 1;

                if (currentChar != '\\')
                {
                    content.Append(currentChar);
                    continue;
                }

                // Escape Sequence (i.e. "\\")
                // In case of an invalid escape sequence, we just drop the `\` from the value

                if (tokenEnd >= text.Length)
                {
                    // Just the slash is invalid
                    var errorSpan = TextSpan.FromStartEnd(tokenEnd - 1, tokenEnd);
                    diagnostics.Add(LexError.InvalidEscapeSequence(file, errorSpan));
                    break; // we hit the end of file and need to not add to tokenEnd any more
                }

                // Escape Sequence with next char (i.e. "\\x")
                var escapeStart = tokenEnd - 1;
                currentChar = text[tokenEnd];
                tokenEnd += 1;
                switch (currentChar)
                {
                    case '"':
                    case '\'':
                    case '\\':
                        content.Append(currentChar);
                        break;
                    case 'n':
                        content.Append('\n');
                        break;
                    case 'r':
                        content.Append('\r');
                        break;
                    case '0':
                        content.Append('\0');
                        break;
                    case 't':
                        content.Append('\t');
                        break;
                    case 'u':
                    {
                        if (tokenEnd < text.Length && text[tokenEnd] == '(')
                            tokenEnd += 1;
                        else
                        {
                            content.Append('u');
                            var errorSpan = TextSpan.FromStartEnd(escapeStart, tokenEnd);
                            diagnostics.Add(LexError.InvalidEscapeSequence(file, errorSpan));
                            break;
                        }

                        var codepoint = new StringBuilder(6);
                        while (tokenEnd < text.Length && IsHexDigit(currentChar = text[tokenEnd]))
                        {
                            codepoint.Append(currentChar);
                            tokenEnd += 1;
                        }

                        int value;
                        if (codepoint.Length is > 0 and <= 6 && (value = Convert.ToInt32(codepoint.ToString(), 16)) <= 0x10FFFF)
                        {
                            // TODO disallow surrogate pairs
                            content.Append(char.ConvertFromUtf32(value));
                        }
                        else
                        {
                            content.Append("u(");
                            content.Append(codepoint);
                            // Include the closing ')' in the escape sequence if it is present
                            if (tokenEnd < text.Length && text[tokenEnd] == ')')
                            {
                                content.Append(')');
                                tokenEnd += 1;
                            }

                            var errorSpan = TextSpan.FromStartEnd(escapeStart, tokenEnd);
                            diagnostics.Add(LexError.InvalidEscapeSequence(file, errorSpan));
                            break;
                        }

                        if (tokenEnd < text.Length && text[tokenEnd] == ')')
                            tokenEnd += 1;
                        else
                        {
                            var errorSpan = TextSpan.FromStartEnd(escapeStart, tokenEnd);
                            diagnostics.Add(LexError.InvalidEscapeSequence(file, errorSpan));
                        }

                        break;
                    }
                    default:
                    {
                        // Last two chars form the invalid sequence
                        var errorSpan = TextSpan.FromStartEnd(tokenEnd - 2, tokenEnd);
                        diagnostics.Add(LexError.InvalidEscapeSequence(file, errorSpan));
                        // drop the `/` keep the character after
                        content.Append(currentChar);
                        break;
                    }
                }
            }

            if (tokenEnd < text.Length)
            {
                // To include the close quote
                if (text[tokenEnd] == '"') tokenEnd += 1;
                isUnclosed = false;
            }
            else
                isUnclosed = true;

            return content.ToString();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]

    private static bool IsIntegerCharacter(char c) => c is >= '0' and <= '9';

    [MethodImpl(MethodImplOptions.AggressiveInlining)]

    private static bool IsIdentifierStartCharacter(char c)
        => c is (>= 'a' and <= 'z') or (>= 'A' and <= 'Z') or '_';

    [MethodImpl(MethodImplOptions.AggressiveInlining)]

    private static bool IsIdentifierCharacter(char c)
        => c is (>= 'a' and <= 'z') or (>= 'A' and <= 'Z') or '_' || char.IsNumber(c);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]

    private static bool IsHexDigit(char c)
        => c is (>= '0' and <= '9') or (>= 'a' and <= 'f') or (>= 'A' and <= 'F');
}
