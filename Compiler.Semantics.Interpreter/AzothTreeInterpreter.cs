using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter;

public sealed class AzothTreeInterpreter
{
    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "OO")]
    public InterpreterProcess Execute(IPackageNode package, IEnumerable<IPackageNode> referencedPackages)
        => InterpreterProcess.StartEntryPoint(package, referencedPackages);

    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "OO")]
    public InterpreterProcess ExecuteTests(IPackageNode package, IEnumerable<IPackageNode> referencedPackages)
        => InterpreterProcess.StartTests(package, referencedPackages);
}
