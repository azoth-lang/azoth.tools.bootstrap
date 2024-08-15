using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CST;

/// <summary>
/// Represents an entire package worth of syntax.
/// </summary>
/// <remarks>Doesn't inherit from <see cref="IConcreteSyntax"/> because it is never
/// matched as part of syntax. It is always treated as the singular root.
///
/// Currently, references are stored as ASTs. To avoid referencing the AST
/// project from the CST project, a generic type is used.
/// </remarks>
public class PackageSyntax : IPackageSyntax
{
    public IdentifierName Name { get; }
    public IFixedSet<ICompilationUnitSyntax> CompilationUnits { get; }
    public IFixedSet<ICompilationUnitSyntax> TestingCompilationUnits { get; }
    public IFixedSet<IPackageReferenceSyntax> References { get; }
    public Diagnostics Diagnostics { get; }

    public PackageSyntax(
        IdentifierName name,
        IFixedSet<ICompilationUnitSyntax> compilationUnits,
        IFixedSet<ICompilationUnitSyntax> testingCompilationUnits,
        IFixedSet<IPackageReferenceSyntax> references)
    {
        Name = name;
        CompilationUnits = compilationUnits;
        TestingCompilationUnits = testingCompilationUnits;
        References = references;
        var builder = new DiagnosticsBuilder
        {
            CompilationUnits.Concat(TestingCompilationUnits).SelectMany(cu => cu.Diagnostics)
        };
        Diagnostics = builder.Build();
    }

    public override string ToString()
        => $"package {Name.Text}: {CompilationUnits.Count} Compilation Units";
}
