using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Framework;
using MoreLinq;

namespace Azoth.Tools.Bootstrap.Tests.Unit.Compiler.Lexing.Helpers;

public class PseudoToken
{
    public Type TokenType { get; }

    public string Text { get; }
    public object? Value { get; }

    public PseudoToken(Type tokenType, string text, object? value = null)
    {
        TokenType = tokenType;
        Text = text;
        Value = value;
    }

    public static PseudoToken EndOfFile() => new(typeof(IEndOfFileToken), "");

    public static PseudoToken For(IToken token, CodeText code)
    {
        var tokenType = token.GetType();
        var text = token.Text(code);
        return token switch
        {
            IIdentifierToken identifier => new PseudoToken(tokenType, text, identifier.Value),
            IStringLiteralToken stringLiteral => new PseudoToken(tokenType, text, stringLiteral.Value),
            IIntegerLiteralToken integerLiteral => new PseudoToken(tokenType, text, integerLiteral.Value),
            _ => new PseudoToken(tokenType, text)
        };
    }

    public override bool Equals(object? obj)
    {
        if (obj is PseudoToken token &&
            (TokenType == token.TokenType
             || TokenType.IsAssignableFrom(token.TokenType)
             || token.TokenType.IsAssignableFrom(TokenType)) &&
            Text == token.Text)
        {
            if (Value is IReadOnlyList<Diagnostic> diagnostics
                && token.Value is IReadOnlyList<Diagnostic> otherDiagnostics)
            {
                // TODO this zip looks wrong, shouldn't it be comparing something rather than always returning false?
                return diagnostics.EquiZip(otherDiagnostics, (d1, d2) => false).All(i => i);
            }
            return EqualityComparer<object>.Default.Equals(Value, token.Value);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(TokenType, Text, Value);
    }

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
