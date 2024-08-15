using System;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;

public sealed class FatalCompilationErrorException : Exception
{
    public DiagnosticCollection Diagnostics { get; }

    public FatalCompilationErrorException(DiagnosticCollection diagnostics)
    {
        Diagnostics = diagnostics;
    }
}
