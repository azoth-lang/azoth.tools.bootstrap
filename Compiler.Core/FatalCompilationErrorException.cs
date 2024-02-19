using System;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Core;

public class FatalCompilationErrorException : Exception
{
    public IFixedList<Diagnostic> Diagnostics { get; }

    public FatalCompilationErrorException(IFixedList<Diagnostic> diagnostics)
    {
        Diagnostics = diagnostics;
    }
}
