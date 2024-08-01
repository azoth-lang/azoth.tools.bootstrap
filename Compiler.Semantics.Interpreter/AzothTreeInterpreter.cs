using System.Diagnostics.CodeAnalysis;
using Azoth.Tools.Bootstrap.Compiler.AST;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter;

public class AzothTreeInterpreter
{
    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "OO")]
    public InterpreterProcess Execute(Package package)
        => InterpreterProcess.StartEntryPoint(package);

    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "OO")]
    public InterpreterProcess ExecuteTests(Package package)
        => InterpreterProcess.StartTests(package);
}
