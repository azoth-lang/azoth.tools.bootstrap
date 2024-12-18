using System;
using System.Collections.Generic;
using System.Numerics;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Tests.Unit.Compiler.Lexing.Helpers;

/// <summary>
/// Used in tests for doing token like operations to set up and to do assertions. It acts as a
/// pseudo token because it carries around the token type it replaces and uses that as part of its
/// identity.
/// </summary>
public class PseudoToken
{
    public Type TokenType { get; }
    public string Text { get; }
    public object? Value { get; }

    public PseudoToken(Type tokenType, string text)
    {
        Requires.That(tokenType.IsInterface, nameof(tokenType), "Token type must be an interface.");
        TokenType = tokenType;
        Text = text;
    }

    public PseudoToken(Type tokenType, string text, string value)
        : this(tokenType, text)
    {
        Value = value;
    }

    public PseudoToken(Type tokenType, string text, BigInteger value)
        : this(tokenType, text)
    {
        Value = value;
    }

    public static PseudoToken EndOfFile() => new(typeof(IEndOfFileToken), "");

    public static PseudoToken For(IToken token, CodeText code)
    {
        var tokenType = token.GetType();
        if (!tokenType.IsInterface)
            tokenType = tokenType.GetInterface("I" + tokenType.Name)
                ?? throw new InvalidOperationException($"Could not find proper interface for type {tokenType.GetFriendlyName()}");
        var text = token.Text(code);
        return token switch
        {
            IIdentifierToken identifier => new(tokenType, text, identifier.Value),
            IStringLiteralToken stringLiteral => new(tokenType, text, stringLiteral.Value),
            IIntegerLiteralToken integerLiteral => new(tokenType, text, integerLiteral.Value),
            _ => new(tokenType, text)
        };
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj is PseudoToken other
               && TokenType == other.TokenType
               && Text == other.Text
               && Equals(Value, other.Value);
    }

    public override int GetHashCode() => HashCode.Combine(TokenType, Text, Value);

    public override string ToString()
    {
        var textValue = string.IsNullOrEmpty(Text) ? "" : $":„{Text.Escape()}„";
        return Value switch
        {
            null => $"{TokenType.Name}{textValue}",
            string s => $"{TokenType.Name}{textValue} 【{s.Escape()}】",
            BigInteger i => $"{TokenType.Name}{textValue} {i}",
            IReadOnlyList<Diagnostic> diagnostics => $"{TokenType.Name}{textValue} [{diagnostics.DebugFormat()}]",
            _ => $"{TokenType.Name}{textValue} InvalidValue={Value}"
        };
    }
}
