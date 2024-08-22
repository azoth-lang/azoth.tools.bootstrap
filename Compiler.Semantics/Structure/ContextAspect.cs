using Azoth.Tools.Bootstrap.Compiler.Core.Code;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;

internal static partial class ContextAspect
{
    public static partial CodeFile CompilationUnit_Children_Broadcast_File(ICompilationUnitNode node)
        => node.File;
}
