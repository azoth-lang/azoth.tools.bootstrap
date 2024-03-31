using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Languages;

public partial class ClassesCodeTemplate
{
    private readonly LanguageNode language;
    private readonly GrammarNode grammar;

    public ClassesCodeTemplate(LanguageNode language)
    {
        this.language = language;
        grammar = language.Grammar;
    }

    public IReadOnlyDictionary<LanguageNode, IFixedList<RuleNode>> OtherLanguagesDefiningRules()
    {
        return language.RuleDefinedIn.Where(kvp => kvp.Value != language).GroupBy(kvp => kvp.Value)
                       .ToDictionary(g => g.Key, g => g.Select(kvp => grammar.RuleFor(kvp.Key)!).ToFixedList());
    }
}
