using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter;

public sealed class AzothTreeInterpreter
{
    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "OO")]
    public InterpreterProcess Execute(IPackageFacetNode packageFacet, IEnumerable<IPackageFacetNode> referencedPackageFacets)
        => InterpreterProcess.StartEntryPoint(packageFacet, referencedPackageFacets);

    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "OO")]
    public InterpreterProcess ExecuteTests(IPackageFacetNode packageFacet, IEnumerable<IPackageFacetNode> referencedPackageFacets)
        => InterpreterProcess.StartTests(packageFacet, referencedPackageFacets);
}
