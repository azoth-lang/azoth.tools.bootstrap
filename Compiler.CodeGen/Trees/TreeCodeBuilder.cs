using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Trees;

public static class TreeCodeBuilder
{
    public static string GenerateTree(GrammarNode grammar)
    {
        var template = new TreeCodeTemplate(grammar);
        return template.TransformText();
    }

    public static string GenerateChildren(GrammarNode grammar)
    {
        var template = new ChildrenCodeTemplate(grammar);
        return template.TransformText();
    }
}
