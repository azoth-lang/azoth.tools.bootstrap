using System;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Core
{
    public class FatalCompilationErrorException : Exception
    {
        public FixedList<Diagnostic> Diagnostics { get; }

        public FatalCompilationErrorException(FixedList<Diagnostic> diagnostics)
        {
            Diagnostics = diagnostics;
        }
    }
}
