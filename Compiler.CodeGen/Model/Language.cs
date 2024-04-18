using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;

public class Language
{
    public LanguageNode Syntax { get; }
    public string? Name => Syntax.Name;
    public Symbol Entry { get; }
    public Grammar Grammar { get; }
    public Language? Extends { get; }

    public Language(LanguageNode syntax, LanguageLoader languageLoader)
    {
        Syntax = syntax;
        Extends = syntax.Extends is null ? null : languageLoader.GetOrLoadLanguageNamed(syntax.Extends.Name!);
        Grammar = new Grammar(this, syntax.Grammar);
        Entry = Symbol.CreateFromSyntax(Grammar, syntax.Entry);
    }
}
