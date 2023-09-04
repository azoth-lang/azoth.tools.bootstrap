using Azoth.Tools.Bootstrap.Compiler.CodeGen.Config;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen;

public static class CodeBuilder
{
    public static string GenerateTree(Grammar grammar)
    {
        var template = new TreeCodeTemplate(grammar);
        return template.TransformText();
    }

    public static string GenerateChildren(Grammar grammar)
    {
        var template = new ChildrenCodeTemplate(grammar);
        return template.TransformText();
    }
}
