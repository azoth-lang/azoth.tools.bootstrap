using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Core;

public static class ShouldEmit
{
    public static bool Class(TreeNodeModel node)
        => !node.IsAbstract;

    public static bool Constructor(TreeNodeModel node)
        => !node.IsAbstract;
}
