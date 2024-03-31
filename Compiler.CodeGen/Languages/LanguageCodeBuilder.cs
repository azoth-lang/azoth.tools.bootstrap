using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Languages;

internal static class LanguageCodeBuilder
{
    public static string GenerateLanguage(LanguageNode language)
    {
        var template = new LanguageCodeTemplate(language);
        return template.TransformText();
    }

    public static string GenerateClasses(LanguageNode language)
    {
        var template = new ClassesCodeTemplate(language);
        return template.TransformText();
    }
}
