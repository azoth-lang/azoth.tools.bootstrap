using System;

namespace Azoth.Tools.Bootstrap.Compiler.Core;

public class FatalCompilationErrorException : Exception
{
    public Diagnostics Diagnostics { get; }

    public FatalCompilationErrorException(Diagnostics diagnostics)
    {
        Diagnostics = diagnostics;
    }
}
