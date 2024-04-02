using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Trees;

public static class TreeCodeBuilder
{
    public static string GenerateTree(Language language)
    {
        var template = new TreeCodeTemplate(language);
        return template.TransformText();
    }

    public static string GenerateChildren(Language language)
    {
        var template = new ChildrenCodeTemplate(language);
        return template.TransformText();
    }
}
