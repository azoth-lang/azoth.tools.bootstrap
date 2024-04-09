using System;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;

public sealed class Symbol
{
    public SymbolNode Syntax { get; }
    public string Name { get; }
    private readonly Lazy<Rule?> referencedRule;
    public Rule? ReferencedRule => referencedRule.Value;

    public Symbol(Grammar grammar, SymbolNode syntax)
    {
        Syntax = syntax;
        Name = Syntax.IsQuoted ? Syntax.Text : $"{grammar.Prefix}{Syntax.Text}{grammar.Suffix}";
        referencedRule = new(() => grammar.RuleFor(Syntax));
    }
}