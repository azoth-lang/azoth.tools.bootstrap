using System;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;

namespace Azoth.Tools.Bootstrap.Lab;

internal sealed class StopCompilationException : Exception
{
    public StopCompilationException(FatalCompilationErrorException innerException)
        : base(null, innerException) { }
}
