using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Core.Config;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Languages;

public partial class ClassesCodeTemplate
{
    private readonly Language language;
    private readonly Grammar grammar;

    public ClassesCodeTemplate(Language language)
    {
        this.language = language;
        grammar = language.Grammar;
    }

    public IReadOnlyDictionary<Language, IFixedList<GrammarRule>> OtherLanguagesDefiningRules()
    {
        return language.RuleDefinedIn.Where(kvp => kvp.Value != language).GroupBy(kvp => kvp.Value)
                       .ToDictionary(g => g.Key, g => g.Select(kvp => grammar.RuleFor(kvp.Key)!).ToFixedList());
    }
}
