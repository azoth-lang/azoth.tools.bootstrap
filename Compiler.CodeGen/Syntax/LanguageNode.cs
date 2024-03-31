using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;

public sealed class LanguageNode
{
    public string Name { get; }
    public GrammarNode Grammar { get; }
    public LanguageNode? Extends { get; }
    public IFixedSet<RuleNode> DifferentRules { get; }
    public FixedDictionary<Symbol, LanguageNode> RuleDefinedIn { get; }

    public LanguageNode(string name, GrammarNode grammar, LanguageNode? extends)
    {
        Name = name;
        Grammar = grammar;
        Extends = extends;
        DifferentRules = ComputeDifferentRules(grammar, extends);
        RuleDefinedIn = ComputeRulesDefinedIn();
    }

    public bool IsModified(RuleNode rule)
    {
        if (rule.Defines.Text == "IntLiteral")
        {
            Debugger.Break();
        }
        return DifferentRules.Contains(rule) && Extends?.Grammar.RuleFor(rule.Defines) is not null;
    }

    private static IFixedSet<RuleNode> ComputeDifferentRules(GrammarNode grammar, LanguageNode? extends)
    {
        if (extends is null)
            return grammar.Rules.ToFixedSet();

        return ComputeDifferentRulesInternal(grammar, extends).ToFixedSet();
    }

    private static IEnumerable<RuleNode> ComputeDifferentRulesInternal(GrammarNode grammar, LanguageNode extends)
    {
        foreach (var rule in grammar.Rules)
        {
            var oldRule = extends.Grammar.RuleFor(rule.Defines);
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

    public FixedDictionary<Symbol, LanguageNode> ComputeRulesDefinedIn()
    {
        if (Extends is null)
            return Grammar.Rules.ToFixedDictionary(r => r.Defines, _ => this);

        return Grammar.Rules.ToFixedDictionary(r => r.Defines, ComputeRuleDefinedIn);
    }

    private LanguageNode ComputeRuleDefinedIn(RuleNode rule)
        => DifferentRules.Contains(rule) ? this : Extends!.RuleDefinedIn[rule.Defines];

    public void Deconstruct(out string name, out GrammarNode grammar, out LanguageNode? extends)
    {
        name = Name;
        grammar = Grammar;
        extends = Extends;
    }
}
