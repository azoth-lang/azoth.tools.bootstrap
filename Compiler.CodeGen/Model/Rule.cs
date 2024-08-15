using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;

public class Rule
{
    public Grammar Grammar { get; }
    public RuleSyntax Syntax { get; }

    public InternalSymbol Defines { get; }
    public SymbolType DefinesType { get; }
    // TODO combine into one collection of bases
    public IFixedSet<Symbol> Supertypes { get; }
    private readonly Lazy<IFixedSet<Rule>> supertypeRules;
    public IFixedSet<Rule> SupertypeRules => supertypeRules.Value;
    public IFixedSet<Rule> AncestorRules => ancestorRules.Value;
    private readonly Lazy<IFixedSet<Rule>> ancestorRules;
    public IFixedSet<Rule> ChildRules => childRules.Value;
    private readonly Lazy<IFixedSet<Rule>> childRules;
    // TODO this is not the correct term for this. Terminal/Non-Terminal is about whether it is a rule or a token
    public bool IsTerminal => ChildRules.IsEmpty;
    public IFixedSet<Rule> DescendantRules => descendantRules.Value;
    private readonly Lazy<IFixedSet<Rule>> descendantRules;

    public IFixedList<Property> DeclaredProperties { get; }

    /// <summary>
    /// Properties inherited from the parents of a rule. If the same property is defined on multiple
    /// parents, it will be listed multiple times.
    /// </summary>
    public IFixedList<Property> InheritedProperties => inheritedProperties.Value;
    private readonly Lazy<IFixedList<Property>> inheritedProperties;

    /// <summary>
    /// Get all properties for a rule. If that rule defines the property itself, that
    /// is the one definition. When the rule doesn't define the property, base classes are
    /// recursively searched for definitions. Multiple definitions are returned when multiple
    /// parents of a rule contain definitions of the property without it being defined on that rule.
    /// </summary>
    public IFixedList<Property> AllProperties => allProperties.Value;
    private readonly Lazy<IFixedList<Property>> allProperties;

    public Rule(Grammar grammar, RuleSyntax syntax)
    {
        Grammar = grammar;
        Syntax = syntax;
        Defines = Symbol.CreateInternalFromSyntax(grammar, syntax.Defines);
        DefinesType = new SymbolType(Defines);
        Supertypes = syntax.Supertypes.Select(s => Symbol.CreateFromSyntax(grammar, s)).ToFixedSet();
        if (Grammar.Root is { } root && root != Defines && !Supertypes.Any(s => s is InternalSymbol))
            Supertypes = Supertypes.Append(root).ToFixedSet();

        supertypeRules = new(() => Supertypes.OfType<InternalSymbol>().Select(s => s.ReferencedRule)
                                             .EliminateRedundantRules().ToFixedSet());
        ancestorRules = new(() => SupertypeRules.Concat(SupertypeRules.SelectMany(p => p.AncestorRules)).ToFixedSet());
        childRules = new(() => Grammar.Rules.Where(r => r.SupertypeRules.Contains(this)).ToFixedSet());
        descendantRules = new(() => ChildRules.Concat(ChildRules.SelectMany(r => r.DescendantRules)).ToFixedSet());

        DeclaredProperties = syntax.DeclaredProperties.Select(p => new Property(this, p)).ToFixedList();
        inheritedProperties = new(() => SupertypeRules.SelectMany(r => r.AllProperties).Distinct().ToFixedList());
        allProperties = new(() =>
        {
            var rulePropertyNames = DeclaredProperties.Select(p => p.Name).ToFixedSet();
            return DeclaredProperties
                   .Concat(InheritedProperties.Where(p => !rulePropertyNames.Contains(p.Name)))
                   .ToFixedList();
        });
    }

    public IEnumerable<Property> InheritedPropertiesNamed(Property property)
        => InheritedPropertiesNamed(property.Name);
    public IEnumerable<Property> InheritedPropertiesNamed(string propertyName)
        => InheritedProperties.Where(p => p.Name == propertyName);

    public IEnumerable<Property> InheritedPropertiesWithoutMostSpecificImplementationNamed(Property property)
        => InheritedPropertiesWithoutMostSpecificImplementationNamed(property.Name);
    public IEnumerable<Property> InheritedPropertiesWithoutMostSpecificImplementationNamed(string propertyName)
    {
        var inheritedProperties = InheritedPropertiesNamed(propertyName).ToFixedSet();
        if (inheritedProperties.Count <= 1)
            return [];

        return inheritedProperties
               .SelectMany(p => p.Rule.InheritedPropertiesNamed(propertyName))
               .Distinct()
               .Except(inheritedProperties);
    }
}
