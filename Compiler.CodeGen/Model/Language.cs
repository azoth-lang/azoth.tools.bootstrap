using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;

public class Language
{
    public LanguageNode Syntax { get; }
    public string? Name => Syntax.Name;
    public Symbol Entry { get; }
    public Grammar Grammar { get; }
    public Language? Extends { get; }

    public Language(LanguageNode syntax)
    {
        Syntax = syntax;
        Extends = syntax.Extends is null ? null : new Language(syntax.Extends);
        Grammar = new Grammar(this, syntax.Grammar);
        Entry = new Symbol(Grammar, syntax.Entry);
    }
}
