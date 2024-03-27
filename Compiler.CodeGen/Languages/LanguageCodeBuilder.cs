using Azoth.Tools.Bootstrap.Compiler.CodeGen.Core.Config;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Trees;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Languages;

public static class LanguageCodeBuilder
{
    public static string GenerateLanguage(Grammar grammar)
    {
        var template = new TreeCodeTemplate(grammar);
        return template.TransformText();
    }
}
