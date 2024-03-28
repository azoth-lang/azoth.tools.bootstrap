using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Azoth.Tools.Bootstrap.Framework;
using MoreLinq;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Core.Config;

public sealed class Grammar
{
    public string? Namespace { get; }
    public GrammarSymbol? RootType { get; }
    public string Prefix { get; }
    public string Suffix { get; }
    public string ListType { get; }
    public IFixedList<string> UsingNamespaces { get; }
    public IFixedList<GrammarRule> Rules { get; }

    public Grammar(
        string? @namespace,
        GrammarSymbol? rootType,
        string prefix,
        string suffix,
        string listType,
        IEnumerable<string> usingNamespaces,
        IEnumerable<GrammarRule> rules)
    {
        Namespace = @namespace;
        RootType = rootType;
        Prefix = prefix;
        Suffix = suffix;
        ListType = listType;
        UsingNamespaces = usingNamespaces.ToFixedList();
        Rules = rules.ToFixedList();
        if (Rules.Select(r => r.Nonterminal).Distinct().Count() != Rules.Count)
            throw new ValidationException("Nonterminal names must be unique");

        rulesLookup = Rules.ToFixedDictionary(r => r.Nonterminal);
        parentRules = Rules.ToFixedDictionary(r => r, r => ParentRules(r, rulesLookup));
        ancestorRules = Rules.ToFixedDictionary(r => r, r => AncestorRules(r, parentRules));
        childRules = Rules.ToFixedDictionary(r => r, r => ChildRules(r, parentRules));
        inheritedProperties = Rules.ToFixedDictionary(r => r, r => InheritedProperties(r, parentRules).GroupToFixedDictionary(p => p.Name, ps => ps.ToFixedList()));
        allProperties = Rules.ToFixedDictionary(r => r, r => AllProperties(r, parentRules).ToFixedList());
    }

    private static IFixedSet<GrammarRule> ParentRules(GrammarRule rule, IReadOnlyDictionary<GrammarSymbol, GrammarRule> rules)
        => rule.Parents.Choose(p => (rules.TryGetValue(p, out var rule), rule!)).ToFixedSet();

    public static IFixedSet<GrammarRule> AncestorRules(
        GrammarRule rule,
        IReadOnlyDictionary<GrammarRule, IFixedSet<GrammarRule>> parentRules)
    {
        var parents = parentRules[rule];
        return parents.Concat(parents.SelectMany(parent => AncestorRules(parent, parentRules))).ToFixedSet();
    }

    public static IFixedSet<GrammarRule> ChildRules(
        GrammarRule rule,
        IReadOnlyDictionary<GrammarRule, IFixedSet<GrammarRule>> parentRules)
        => parentRules.Where(kvp => kvp.Value.Contains(rule)).Select(kvp => kvp.Key).ToFixedSet();

    /// <summary>
    /// Properties inherited from the parents of a rule. If the same property is defined on multiple
    /// parents, it will be listed multiple times.
    /// </summary>
    private static IEnumerable<GrammarProperty> InheritedProperties(
        GrammarRule rule,
        IReadOnlyDictionary<GrammarRule, IFixedSet<GrammarRule>> parentRules)
        => parentRules[rule].SelectMany(parent => AllProperties(parent, parentRules));

    /// <summary>
    /// Get all properties for a rule. If that rule defines the property itself, that
    /// is the one definition. When the rule doesn't define the property, base classes are
    /// recursively searched for definitions. Multiple definitions are returned when multiple
    /// parents of a rule contain definitions of the property without it being defined on that rule.
    /// </summary>
    private static IEnumerable<GrammarProperty> AllProperties(
        GrammarRule rule,
        IReadOnlyDictionary<GrammarRule, IFixedSet<GrammarRule>> parentRules)
    {
        var rulePropertyNames = rule.Properties.Select(p => p.Name).ToFixedSet();
        return rule.Properties
                   .Concat(InheritedProperties(rule, parentRules)
                       .Where(prop => !rulePropertyNames.Contains(prop.Name)));
    }

    public void ValidateTreeOrdering()
    {
        foreach (var rule in Rules.Where(IsLeaf))
        {
            var baseNonTerminalPropertyNames
                = AncestorRules(rule)
                  .SelectMany(r => r.Properties)
                  .Where(IsNonterminal).Select(p => p.Name);
            var nonTerminalPropertyNames = rule.Properties.Where(IsNonterminal).Select(p => p.Name);
            var missingProperties = baseNonTerminalPropertyNames.Except(nonTerminalPropertyNames).ToList();
            if (missingProperties.Any())
                throw new ValidationException($"Rule for {rule.Nonterminal} is missing inherited properties: {string.Join(", ", missingProperties)}. Can't determine order to visit children.");
        }
    }

    public GrammarRule? RuleFor(GrammarSymbol nonterminal)
        => rulesLookup.TryGetValue(nonterminal, out var rule) ? rule : null;

    public IFixedSet<GrammarRule> ParentRules(GrammarRule rule)
        => parentRules[rule];

    public IFixedSet<GrammarRule> AncestorRules(GrammarRule rule)
        => ancestorRules[rule];

    public IFixedSet<GrammarRule> ChildRules(GrammarRule rule)
        => childRules[rule];

    public IReadOnlyDictionary<string, IFixedList<GrammarProperty>> InheritedProperties(GrammarRule rule)
        => inheritedProperties[rule];

    public IFixedList<GrammarProperty> InheritedProperties(GrammarRule rule, string property)
        => inheritedProperties[rule].TryGetValue(property, out var properties) ? properties : FixedList.Empty<GrammarProperty>();

    public IFixedList<GrammarProperty> AllProperties(GrammarRule rule)
        => allProperties[rule];

    /// <summary>
    /// Something is a new definition if it replaces some parent definition.
    /// </summary>
    public bool IsNewDefinition(GrammarRule rule, GrammarProperty property)
        => InheritedProperties(rule, property.Name).Any();

    public bool IsLeaf(GrammarRule rule)
        => !childRules[rule].Any();

    public bool IsNonterminal(GrammarProperty property)
        => Rules.Any(r => r.Nonterminal == property.Type.Symbol);

    public string TypeName(GrammarSymbol symbol)
        => symbol.IsQuoted ? symbol.Text : $"{Prefix}{symbol.Text}{Suffix}";

    public IEnumerable<string> OrderedUsingNamespaces(params string[] additionalNamespaces)
        => UsingNamespaces.Concat(additionalNamespaces).Distinct().OrderBy(v => v, NamespaceComparer.Instance);

    private readonly FixedDictionary<GrammarRule, IFixedSet<GrammarRule>> parentRules;
    private readonly FixedDictionary<GrammarRule, IFixedSet<GrammarRule>> ancestorRules;
    private readonly FixedDictionary<GrammarRule, IFixedSet<GrammarRule>> childRules;
    private readonly FixedDictionary<GrammarRule, FixedDictionary<string, IFixedList<GrammarProperty>>> inheritedProperties;
    private readonly FixedDictionary<GrammarRule, IFixedList<GrammarProperty>> allProperties;
    private readonly FixedDictionary<GrammarSymbol, GrammarRule> rulesLookup;
}
