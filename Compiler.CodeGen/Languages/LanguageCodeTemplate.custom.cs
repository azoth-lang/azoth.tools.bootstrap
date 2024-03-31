using Azoth.Tools.Bootstrap.Compiler.CodeGen.Core.Config;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Languages;

public partial class LanguageCodeTemplate
{
    private readonly Language language;
    private readonly GrammarNode grammar;

    public LanguageCodeTemplate(Language language)
    {
        this.language = language;
        grammar = language.Grammar;
    }
}
