using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CST;

public interface IPackageSyntax : ISyntax
{
    IdentifierName Name { get; }
    IFixedSet<ICompilationUnitSyntax> CompilationUnits { get; }
    IFixedSet<ICompilationUnitSyntax> TestingCompilationUnits { get; }
    IFixedSet<IPackageReferenceSyntax> References { get; }

    Diagnostics Diagnostics { get; }
}
