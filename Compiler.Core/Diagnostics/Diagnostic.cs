using System;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;

public sealed class Diagnostic
{
    public CodeFile File { get; }
    public TextSpan Span { get; }
    public TextPosition StartPosition { get; }
    public TextPosition EndPosition { get; }
    public DiagnosticLevel Level { get; }
    public DiagnosticPhase Phase { get; }
    public int ErrorCode { get; }
    public string Message { get; }

    public Diagnostic(
        CodeFile file,
        TextSpan span,
        DiagnosticLevel level,
        DiagnosticPhase phase,
        int errorCode,
        string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Can't be null or whitespace", nameof(message));

        Requires.ValidEnum(level, nameof(level));
        Requires.ValidEnum(phase, nameof(phase));

        File = file;
        Span = span;
        StartPosition = file.Code.PositionOfStart(span);
        EndPosition = file.Code.PositionOfEnd(span);
        Level = level;
        Phase = phase;
        ErrorCode = errorCode;
        Message = message;
    }

    public bool IsFatal => Level == DiagnosticLevel.FatalCompilationError;

    public override string ToString() => $"{Level} {ErrorCode}: {Message} @{File.Reference}@{Span}";
}
