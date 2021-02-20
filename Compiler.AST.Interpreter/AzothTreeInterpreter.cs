using System;
using System.Diagnostics.CodeAnalysis;

namespace Azoth.Tools.Bootstrap.Compiler.AST.Interpreter
{
    public class AzothTreeInterpreter
    {
        [SuppressMessage("Performance", "CA1822:Mark members as static",
            Justification = "OO")]
        public InterpreterProcess Execute(Package package)
        {
            if (package.EntryPoint is null)
                throw new ArgumentException("Cannot execute package without an entry point");

            return new InterpreterProcess(package);
        }
    }
}
