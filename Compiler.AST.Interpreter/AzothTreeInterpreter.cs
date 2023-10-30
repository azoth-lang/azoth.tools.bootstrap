using System.Diagnostics.CodeAnalysis;

namespace Azoth.Tools.Bootstrap.Compiler.AST.Interpreter;

public class AzothTreeInterpreter
{
    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "OO")]
    public InterpreterProcess Execute(Package package)
        => InterpreterProcess.StartEntryPoint(package);

    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "OO")]
    public InterpreterProcess ExecuteTests(Package package)
        => InterpreterProcess.StartTests(package);
}
