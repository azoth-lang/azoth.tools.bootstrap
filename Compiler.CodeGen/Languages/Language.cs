using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Core.Config;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Languages;

public sealed class Language
{
    public string Name { get; }
    public Grammar Grammar { get; }
    public Language? Extends { get; }
    public IFixedSet<GrammarRule> DifferentRules { get; }
    public FixedDictionary<GrammarSymbol, Language> RuleDefinedIn { get; }

    public Language(string name, Grammar grammar, Language? extends)
    {
        Name = name;
        Grammar = grammar;
        Extends = extends;
        DifferentRules = ComputeDifferentRules(grammar, extends);
        RuleDefinedIn = ComputeRulesDefinedIn();
    }

    private static IFixedSet<GrammarRule> ComputeDifferentRules(Grammar grammar, Language? extends)
    {
        if (extends is null)
            return grammar.Rules.ToFixedSet();

        return ComputeDifferentRulesInternal(grammar, extends).ToFixedSet();
    }

    private static IEnumerable<GrammarRule> ComputeDifferentRulesInternal(Grammar grammar, Language extends)
    {
        foreach (var rule in grammar.Rules)
        {
            var oldRule = extends.Grammar.RuleFor(rule.Nonterminal);
            if (oldRule is null)
            {
                yield return rule;
                continue;
            }

            var currentProperties = grammar.AllProperties(rule).ToFixedSet();
            var oldProperties = extends.Grammar.AllProperties(oldRule).ToFixedSet();
            if (!currentProperties.SequenceEqual(oldProperties))
                yield return rule;
        }
    }

    public FixedDictionary<GrammarSymbol, Language> ComputeRulesDefinedIn()
    {
        if (Extends is null)
            return Grammar.Rules.ToFixedDictionary(r => r.Nonterminal, _ => this);

        return Grammar.Rules.ToFixedDictionary(r => r.Nonterminal, ComputeRuleDefinedIn);
    }

    private Language ComputeRuleDefinedIn(GrammarRule rule)
        => DifferentRules.Contains(rule) ? this : Extends!.RuleDefinedIn[rule.Nonterminal];

    public void Deconstruct(out string name, out Grammar grammar, out Language? extends)
    {
        name = Name;
        grammar = Grammar;
        extends = Extends;
    }
}