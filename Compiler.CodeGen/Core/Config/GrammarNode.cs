using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Azoth.Tools.Bootstrap.Framework;
using MoreLinq;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Core.Config;

public sealed class GrammarNode
{
    public string? Namespace { get; }
    public Symbol? RootType { get; }
    public string Prefix { get; }
    public string Suffix { get; }
    public string ListType { get; }
    public IFixedList<string> UsingNamespaces { get; }
    public IFixedList<RuleNode> Rules { get; }

    public GrammarNode(
        string? @namespace,
        Symbol? rootType,
        string prefix,
        string suffix,
        string listType,
        IEnumerable<string> usingNamespaces,
        IEnumerable<RuleNode> rules)
    {
        Namespace = @namespace;
        RootType = rootType;
        Prefix = prefix;
        Suffix = suffix;
        ListType = listType;
        UsingNamespaces = usingNamespaces.ToFixedList();
        Rules = rules.ToFixedList();
        if (Rules.Select(r => r.Defines).Distinct().Count() != Rules.Count)
            throw new ValidationException("Nonterminal names must be unique");

        rulesLookup = Rules.ToFixedDictionary(r => r.Defines);
        parentRules = Rules.ToFixedDictionary(r => r, r => ParentRules(r, rulesLookup));
        ancestorRules = Rules.ToFixedDictionary(r => r, r => AncestorRules(r, parentRules));
        childRules = Rules.ToFixedDictionary(r => r, r => ChildRules(r, parentRules));
        inheritedProperties = Rules.ToFixedDictionary(r => r, r => InheritedProperties(r, parentRules).GroupToFixedDictionary(p => p.Name, ps => ps.ToFixedList()));
        allProperties = Rules.ToFixedDictionary(r => r, r => AllProperties(r, parentRules).ToFixedList());
    }

    private static IFixedSet<RuleNode> ParentRules(RuleNode rule, IReadOnlyDictionary<Symbol, RuleNode> rules)
        => rule.Parents.Choose(p => (rules.TryGetValue(p, out var rule), rule!)).ToFixedSet();

    public static IFixedSet<RuleNode> AncestorRules(
        RuleNode rule,
        IReadOnlyDictionary<RuleNode, IFixedSet<RuleNode>> parentRules)
    {
        var parents = parentRules[rule];
        return parents.Concat(parents.SelectMany(parent => AncestorRules(parent, parentRules))).ToFixedSet();
    }

    public static IFixedSet<RuleNode> ChildRules(
        RuleNode rule,
        IReadOnlyDictionary<RuleNode, IFixedSet<RuleNode>> parentRules)
        => parentRules.Where(kvp => kvp.Value.Contains(rule)).Select(kvp => kvp.Key).ToFixedSet();

    /// <summary>
    /// Properties inherited from the parents of a rule. If the same property is defined on multiple
    /// parents, it will be listed multiple times.
    /// </summary>
    private static IEnumerable<PropertyNode> InheritedProperties(
        RuleNode rule,
        IReadOnlyDictionary<RuleNode, IFixedSet<RuleNode>> parentRules)
        => parentRules[rule].SelectMany(parent => AllProperties(parent, parentRules));

    /// <summary>
    /// Get all properties for a rule. If that rule defines the property itself, that
    /// is the one definition. When the rule doesn't define the property, base classes are
    /// recursively searched for definitions. Multiple definitions are returned when multiple
    /// parents of a rule contain definitions of the property without it being defined on that rule.
    /// </summary>
    private static IEnumerable<PropertyNode> AllProperties(
        RuleNode rule,
        IReadOnlyDictionary<RuleNode, IFixedSet<RuleNode>> parentRules)
    {
        var rulePropertyNames = rule.Properties.Select(p => p.Name).ToFixedSet();
        return rule.Properties
                   .Concat(InheritedProperties(rule, parentRules)
                       .Where(prop => !rulePropertyNames.Contains(prop.Name)));
    }

    public void ValidateTreeOrdering()
    {
        foreach (var rule in Rules.Where(IsTerminal))
        {
            var baseNonTerminalPropertyNames
                = AncestorRules(rule)
                  .SelectMany(r => r.Properties)
                  .Where(IsNonterminal).Select(p => p.Name);
            var nonTerminalPropertyNames = rule.Properties.Where(IsNonterminal).Select(p => p.Name);
            var missingProperties = baseNonTerminalPropertyNames.Except(nonTerminalPropertyNames).ToList();
            if (missingProperties.Any())
                throw new ValidationException($"Rule for {rule.Defines} is missing inherited properties: {string.Join(", ", missingProperties)}. Can't determine order to visit children.");
        }
    }

    public RuleNode? RuleFor(Symbol nonterminal)
        => rulesLookup.TryGetValue(nonterminal, out var rule) ? rule : null;

    public IFixedSet<RuleNode> ParentRules(RuleNode rule)
        => parentRules[rule];

    public IFixedSet<RuleNode> AncestorRules(RuleNode rule)
        => ancestorRules[rule];

    public IFixedSet<RuleNode> ChildRules(RuleNode rule)
        => childRules[rule];

    public IReadOnlyDictionary<string, IFixedList<PropertyNode>> InheritedProperties(RuleNode rule)
        => inheritedProperties[rule];

    public IFixedList<PropertyNode> InheritedProperties(RuleNode rule, string property)
        => inheritedProperties[rule].TryGetValue(property, out var properties) ? properties : FixedList.Empty<PropertyNode>();

    public IFixedList<PropertyNode> AllProperties(RuleNode rule)
        => allProperties[rule];

    /// <summary>
    /// Something is a new definition if it replaces some parent definition.
    /// </summary>
    public bool IsNewDefinition(RuleNode rule, PropertyNode property)
        => InheritedProperties(rule, property.Name).Any();

    public bool IsTerminal(RuleNode rule)
        => !childRules[rule].Any();

    public bool IsNonterminal(PropertyNode property)
        => rulesLookup.ContainsKey(property.Type.Symbol);

    public string TypeName(Symbol symbol)
        => symbol.IsQuoted ? symbol.Text : $"{Prefix}{symbol.Text}{Suffix}";

    public IEnumerable<string> OrderedUsingNamespaces(params string[] additionalNamespaces)
        => UsingNamespaces.Concat(additionalNamespaces).Distinct().OrderBy(v => v, NamespaceComparer.Instance);

    private readonly FixedDictionary<RuleNode, IFixedSet<RuleNode>> parentRules;
    private readonly FixedDictionary<RuleNode, IFixedSet<RuleNode>> ancestorRules;
    private readonly FixedDictionary<RuleNode, IFixedSet<RuleNode>> childRules;
    private readonly FixedDictionary<RuleNode, FixedDictionary<string, IFixedList<PropertyNode>>> inheritedProperties;
    private readonly FixedDictionary<RuleNode, IFixedList<PropertyNode>> allProperties;
    private readonly FixedDictionary<Symbol, RuleNode> rulesLookup;
}
