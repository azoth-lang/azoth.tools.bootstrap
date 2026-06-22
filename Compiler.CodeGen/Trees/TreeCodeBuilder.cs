using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Trees;

public static class TreeCodeBuilder
{
    public static string GenerateTree(TreeModel tree)
    {
        var template = new TreeCodeTemplate(tree);
        return template.TransformText();
    }

}
