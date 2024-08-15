using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Trees;

public partial class TreeCodeTemplate
{
    private readonly TreeModel tree;

    public TreeCodeTemplate(TreeModel tree)
    {
        this.tree = tree;
    }
}
