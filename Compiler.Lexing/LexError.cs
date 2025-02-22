using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;

namespace Azoth.Tools.Bootstrap.Compiler.Lexing;

/// <remarks><see cref="ErrorCodeRange"/> for the ranges of various kinds of error codes.</remarks>
internal static class LexError
{
    public static Diagnostic UnclosedBlockComment(CodeFile file, TextSpan span)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Lexing,
            1001, "End-of-file found, expected `*/`");
    }

    public static Diagnostic UnclosedStringLiteral(CodeFile file, TextSpan span)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Lexing,
            1002, "End-of-file in string constant");
    }

    public static Diagnostic InvalidEscapeSequence(CodeFile file, TextSpan span)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Lexing,
            1003, "Unrecognized escape sequence");
    }

    public static Diagnostic CStyleNotEquals(CodeFile file, TextSpan span)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Lexing,
            1004, "Use `=/=` for not equal instead of `!=`");
    }

    public static Diagnostic UnexpectedCharacter(CodeFile file, TextSpan span, char character)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Lexing,
            1005, $"Unexpected character `{character}`");
    }

    public static Diagnostic ReservedWord(CodeFile file, TextSpan span, string word)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Lexing,
            1006, $"Reserved word `{word}` used as an identifier");
    }

    public static Diagnostic ContinueInsteadOfNext(CodeFile file, TextSpan span)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Lexing,
            1007, "The word `continue` is a reserved word. Use the `next` keyword or escape the identifier as `\\continue` instead");
    }

    public static Diagnostic EscapedIdentifierShouldNotBeEscaped(CodeFile file, TextSpan span, string identifier)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Lexing,
            1008, $"The word `{identifier}` is not a keyword or reserved word, it should not be escaped");
    }

    public static Diagnostic ReservedOperator(CodeFile file, TextSpan span, string op)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Lexing,
            1009, $"Unexpected character(s) `{op}`, reserved for operator or punctuators");
    }

    public static Diagnostic UInt8InsteadOfByte(CodeFile file, TextSpan span)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Lexing,
            1010, "The word `uint8` is a reserved word. Use the `byte` keyword instead");
    }

    public static Diagnostic UnclosedStringIdentifier(CodeFile file, TextSpan span)
    {
        return new(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Lexing,
            1011, "End-of-file in string identifier");
    }
}
