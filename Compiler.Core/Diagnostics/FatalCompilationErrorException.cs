using System;

namespace Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;

public class FatalCompilationErrorException : Exception
{
    public DiagnosticsCollection Diagnostics { get; }

    public FatalCompilationErrorException(DiagnosticsCollection diagnostics)
    {
        Diagnostics = diagnostics;
    }
}
