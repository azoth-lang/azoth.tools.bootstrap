using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;

public class Rule
{
    public Language Language { get; }
    public Grammar Grammar { get; }
    public RuleNode Syntax { get; }


    public Symbol Defines { get; }
    public Symbol? Parent { get; }
    public Rule? ParentRule => Parent?.ReferencedRule;
    public IFixedSet<Symbol> Supertypes { get; }
    private readonly Lazy<IFixedSet<Rule>> supertypeRules;
    public IFixedSet<Rule> SupertypeRules => supertypeRules.Value;
    public IFixedSet<Symbol> Parents { get; }
    public IFixedSet<Rule> ParentRules => parentRules.Value;
    private readonly Lazy<IFixedSet<Rule>> parentRules;
    public IFixedSet<Rule> AncestorRules => ancestorRules.Value;
    private readonly Lazy<IFixedSet<Rule>> ancestorRules;
    public IFixedSet<Rule> ChildRules => childRules.Value;
    private readonly Lazy<IFixedSet<Rule>> childRules;
    public bool IsTerminal => !ChildRules.Any();

    public IFixedList<Property> DeclaredProperties { get; }

    /// <summary>
    /// Properties inherited from the parents of a rule. If the same property is defined on multiple
    /// parents, it will be listed multiple times.
    /// </summary>
    public IFixedList<Property> InheritedProperties => inheritedProperties.Value;
    private readonly Lazy<IFixedList<Property>> inheritedProperties;

    /// <summary>
    /// Properties inherited from the supertypes of a rule. If the same property is defined on multiple
    /// parents, it will be listed multiple times.
    /// </summary>
    public IFixedList<Property> SupertypeProperties => supertypeProperties.Value;
    private readonly Lazy<IFixedList<Property>> supertypeProperties;

    /// <summary>
    /// Get all properties for a rule. If that rule defines the property itself, that
    /// is the one definition. When the rule doesn't define the property, base classes are
    /// recursively searched for definitions. Multiple definitions are returned when multiple
    /// parents of a rule contain definitions of the property without it being defined on that rule.
    /// </summary>
    public IFixedList<Property> AllProperties => allProperties.Value;
    private readonly Lazy<IFixedList<Property>> allProperties;

    public IFixedSet<Property> AncestorProperties => ancestorProperties.Value;
    private readonly Lazy<IFixedSet<Property>> ancestorProperties;

    public Rule? ExtendsRule => extendsRule.Value;
    private readonly Lazy<Rule?> extendsRule;

    /// <summary>
    /// Whether this rule is new in this language (e.g. added in this language or no language is
    /// being extended).
    /// </summary>
    public bool IsNew => isNew.Value;
    private readonly Lazy<bool> isNew;

    /// <summary>
    /// Whether this rule is modified relative to the rule in the language being extended.
    /// </summary>
    public bool IsModified => isModified.Value;
    private readonly Lazy<bool> isModified;

    /// <summary>
    /// Whether this rule or any decedents of it have been modified.
    /// </summary>
    public bool DescendantsModified
    {
        get
        {
            if (descendantsModified.TryBeginFulfilling(DescendantsModifiedInCycle))
                descendantsModified.Fulfill(ComputeDescendantsModified());

            return descendantsModified.Result;
        }
    }
    private readonly AcyclicPromise<bool> descendantsModified = new();

    internal bool TryDescendantsModified
    {
        get
        {
            if (descendantsModified.TryBeginFulfilling())
                descendantsModified.Fulfill(ComputeDescendantsModified());

            return descendantsModified.ResultOr(false);
        }
    }

    public Language DefinedInLanguage => definedInLanguage.Value;
    private readonly Lazy<Language> definedInLanguage;

    public Rule(Grammar grammar, RuleNode syntax)
    {
        Language = grammar.Language;
        Grammar = grammar;
        Syntax = syntax;
        Defines = new Symbol(grammar, syntax.Defines);
        Parent = syntax.Parent is null ? null : new Symbol(grammar, syntax.Parent);
        Supertypes = syntax.Supertypes.Select(s => new Symbol(grammar, s)).ToFixedSet();
        Parents = Parent is null ? Supertypes : Supertypes.Prepend(Parent).ToFixedSet();

        supertypeRules = new(() => Supertypes.Select(s => s.ReferencedRule).WhereNotNull().ToFixedSet());
        parentRules = new(() => ParentRule is null ? SupertypeRules : SupertypeRules.Prepend(ParentRule).ToFixedSet());
        ancestorRules = new(() => ParentRules.Concat(ParentRules.SelectMany(p => p.AncestorRules)).ToFixedSet());
        childRules = new(() => Grammar.Rules.Where(r => r.ParentRules.Contains(this)).ToFixedSet());

        DeclaredProperties = syntax.DeclaredProperties.Select(p => new Property(this, p)).ToFixedList();
        inheritedProperties = new(() => ParentRules.SelectMany(r => r.AllProperties).Distinct().ToFixedList());
        supertypeProperties = new(() => SupertypeRules.SelectMany(r => r.AllProperties)
                                                      .Except(ParentRule?.AllProperties ?? Enumerable.Empty<Property>())
                                                      .Distinct().ToFixedList());
        allProperties = new(() =>
        {
            var rulePropertyNames = DeclaredProperties.Select(p => p.Name).ToFixedSet();
            return DeclaredProperties
                   .Concat(InheritedProperties.Where(p => !rulePropertyNames.Contains(p.Name)))
                   .ToFixedList();
        });
        ancestorProperties = new(() => AncestorRules.SelectMany(r => r.DeclaredProperties).ToFixedSet());

        extendsRule = new(() => grammar.Language.Extends?.Grammar.RuleFor(Defines.Syntax));
        isNew = new(() => ExtendsRule is null);
        isModified = new(() =>
        {
            var oldRule = ExtendsRule;
            if (oldRule is null)
                return false;

            var currentProperties = AllProperties.ToFixedSet();
            var oldProperties = oldRule.AllProperties.ToFixedSet();
            return !currentProperties.SequenceEqual(oldProperties, Property.NameAndTypeComparer);
        });

        definedInLanguage = new(() => IsNew || IsModified ? grammar.Language : grammar.Language.Extends?.Grammar.RuleFor(Defines.Syntax)?.DefinedInLanguage!);
    }

    public Property? ParentPropertiesNamed(Property property)
        => ParentPropertiesNamed(property.Name);

    public Property? ParentPropertiesNamed(string propertyName)
        => ParentRule?.AllProperties.Where(p => p.Name == propertyName).TrySingle();

    public IEnumerable<Property> InheritedPropertiesNamed(Property property)
        => InheritedPropertiesNamed(property.Name);
    public IEnumerable<Property> InheritedPropertiesNamed(string propertyName)
        => InheritedProperties.Where(p => p.Name == propertyName);

    public IEnumerable<Property> SupertypePropertiesNamed(Property property)
        => SupertypePropertiesNamed(property.Name);

    public IEnumerable<Property> SupertypePropertiesNamed(string propertyName)
        => SupertypeProperties.Where(p => p.Name == propertyName);

    public IEnumerable<Property> AncestorPropertiesNamed(Property property)
        => AncestorPropertiesNamed(property.Name);

    public IEnumerable<Property> AncestorPropertiesNamed(string propertyName)
        => AncestorProperties.Where(p => p.Name == propertyName);

    private bool ComputeDescendantsModified()
    {
        return IsModified
               // Note: added child rules are not a problem for the cases that matter here
               || ChildRules.Any(r => r.DescendantsModified)
               || DeclaredProperties.Select(p => p.Type.Symbol.ReferencedRule)
                                    .WhereNotNull()
                                    .Except(this)
                                    .Any(r => r.TryDescendantsModified);
    }

    private static void DescendantsModifiedInCycle()
        => throw new InvalidOperationException("Cycle detected while computing descendants modified");
}
