using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Core;

public static class ShouldEmit
{
    /// <summary>
    /// A property needs declared under three conditions:
    /// 1. there is no definition of the property in the parent
    /// 2. the single parent definition has a different type
    /// 3. the property is defined in multiple parents, in that case it is
    ///    ambiguous unless it is redefined in the current interface.
    /// </summary>
    public static bool Property(PropertyModel property)
        => property.IsDeclared;

    public static bool Class(TreeNodeModel node)
        => !node.IsAbstract;

    public static bool Constructor(TreeNodeModel node)
        => !node.IsAbstract;
}
