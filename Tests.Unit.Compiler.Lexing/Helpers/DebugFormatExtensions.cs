using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;

namespace Azoth.Tools.Bootstrap.Tests.Unit.Compiler.Lexing.Helpers;

public static class DebugFormatExtensions
{
    public static string DebugFormat(this IEnumerable<Diagnostic> diagnostics)
    {
        return string.Join(", ",
            diagnostics.Select(d =>
                $"{d.ErrorCode}@{d.StartPosition.Line}:{d.StartPosition.Column}"));
    }

    public static string DebugFormat(this IEnumerable<PseudoToken> tokens)
        => string.Join(", ", tokens);
}
