using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Config;

public class Grammar
{
    public string? Namespace { get; }
    public GrammarSymbol? BaseType { get; }
    public string Prefix { get; }
    public string Suffix { get; }
    public string ListType { get; }
    public IFixedList<string> UsingNamespaces { get; }
    public IFixedList<GrammarRule> Rules { get; }

    public Grammar(
        string? @namespace,
        GrammarSymbol? baseType,
        string prefix,
        string suffix,
        string listType,
        IEnumerable<string> usingNamespaces,
        IEnumerable<GrammarRule> rules)
    {
        Namespace = @namespace;
        BaseType = baseType;
        Prefix = prefix;
        Suffix = suffix;
        ListType = listType;
        UsingNamespaces = usingNamespaces.ToFixedList();
        Rules = rules.ToFixedList();
    }

    public void Validate()
    {
        foreach (var rule in Rules.Where(IsLeaf))
        {
            var baseNonTerminalPropertyNames
                = AncestorRules(rule)
                  .SelectMany(r => r.Properties)
                  .Where(IsNonTerminal).Select(p => p.Name);
            var nonTerminalPropertyNames = rule.Properties.Where(IsNonTerminal).Select(p => p.Name);
            var missingProperties = baseNonTerminalPropertyNames.Except(nonTerminalPropertyNames).ToList();
            if (missingProperties.Any())
                throw new ValidationException($"Rule for {rule.Nonterminal} is missing inherited properties: {string.Join(", ", missingProperties)}. Can't determine order to visit children.");
        }
    }

    public IEnumerable<GrammarRule> ParentRules(GrammarRule rule)
        => Rules.Where(r => rule.Parents.Contains(r.Nonterminal));

    public IEnumerable<GrammarRule> AncestorRules(GrammarRule rule)
    {
        var parents = ParentRules(rule).ToList();
        return parents.Concat(parents.SelectMany(AncestorRules)).Distinct();
    }

    public bool IsLeaf(GrammarRule rule)
        => !Rules.Any(r => r.Parents.Contains(rule.Nonterminal));

    public bool IsNonTerminal(GrammarProperty property)
        => Rules.Any(r => r.Nonterminal == property.Type.Symbol);
}
