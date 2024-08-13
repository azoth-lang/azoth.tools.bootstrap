namespace Azoth.Tools.Bootstrap.Compiler.Core.Attributes;

public interface IInheritanceContext
{
    void AccessParentOf(IChildTreeNode childNode);
}
