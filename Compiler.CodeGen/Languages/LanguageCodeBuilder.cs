using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Languages;

internal static class LanguageCodeBuilder
{
    public static string GenerateLanguage(Language language)
    {
        var template = new LanguageCodeTemplate(language);
        return template.TransformText();
    }

    public static string GenerateNodes(Language language)
    {
        var template = new NodeCodeTemplate(language);
        return template.TransformText();
    }
}
