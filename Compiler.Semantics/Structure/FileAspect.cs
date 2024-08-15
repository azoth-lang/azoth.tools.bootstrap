using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;

internal static class FileAspect
{
    public static CodeFile CompilationUnit_InheritedFile(ICompilationUnitNode node)
        => node.File;
}
