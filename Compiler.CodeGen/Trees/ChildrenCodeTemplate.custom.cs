using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Trees;

public partial class ChildrenCodeTemplate
{
    private readonly TreeModel tree;

    public ChildrenCodeTemplate(TreeModel tree)
    {
        this.tree = tree;
    }
}
