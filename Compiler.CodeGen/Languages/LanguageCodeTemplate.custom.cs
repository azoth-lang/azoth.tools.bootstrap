using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Languages;

public partial class LanguageCodeTemplate
{
    private readonly Language language;
    private readonly Grammar grammar;

    public LanguageCodeTemplate(Language language)
    {
        this.language = language;
        grammar = language.Grammar;
    }
}
