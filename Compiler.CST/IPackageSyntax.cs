using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CST;

public interface IPackageSyntax
{
    IFixedSet<ICompilationUnitSyntax> CompilationUnits { get; }
}
