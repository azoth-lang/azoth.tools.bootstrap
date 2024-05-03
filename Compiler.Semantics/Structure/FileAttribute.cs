using Azoth.Tools.Bootstrap.Compiler.Core;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;

internal static class FileAttribute
{
    public static CodeFile CompilationUnitInherited(ICompilationUnitNode node)
        => node.File;
}
