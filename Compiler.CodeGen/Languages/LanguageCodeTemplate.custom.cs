using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Languages;

public partial class LanguageCodeTemplate
{
    private readonly LanguageNode language;
    private readonly GrammarNode grammar;

    public LanguageCodeTemplate(LanguageNode language)
    {
        this.language = language;
        grammar = language.Grammar;
    }
}
