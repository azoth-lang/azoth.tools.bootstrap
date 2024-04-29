using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;

public class Rule
{
    public Language Language { get; }
    public Grammar Grammar { get; }
    public RuleNode Syntax { get; }


    public InternalSymbol Defines { get; }
    public SymbolType DefinesType { get; }
    public InternalSymbol? Base { get; }
    public Rule? BaseRule => Base?.ReferencedRule;
    // TODO combine into one collection of bases
    public IFixedSet<Symbol> Supertypes { get; }
    private readonly Lazy<IFixedSet<Rule>> supertypeRules;
    public IFixedSet<Rule> SupertypeRules => supertypeRules.Value;
    public IFixedSet<Symbol> Parents { get; }
    public IFixedSet<Rule> BaseRules => parentRules.Value;
    private readonly Lazy<IFixedSet<Rule>> parentRules;
    public IFixedSet<Rule> AncestorRules => ancestorRules.Value;
    private readonly Lazy<IFixedSet<Rule>> ancestorRules;
    public IFixedSet<Rule> ChildRules => childRules.Value;
    private readonly Lazy<IFixedSet<Rule>> childRules;
    public bool IsTerminal => !ChildRules.Any();
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

    /// <summary>
    /// Properties that are different in any way from the previous language, including that they
    /// reference the equivalent rule in this language.
    /// </summary>
    public IFixedList<Property> DifferentProperties => differentProperties.Value;
    private readonly Lazy<IFixedList<Property>> differentProperties;

    /// <summary>
    /// Properties that are modified, not just different because they reference the equivalent
    /// rule in this language.
    /// </summary>
    public IFixedList<Property> ModifiedProperties => modifiedProperties.Value;
    private readonly Lazy<IFixedList<Property>> modifiedProperties;

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
        Defines = Symbol.CreateInternalFromSyntax(grammar, syntax.Defines);
        DefinesType = new SymbolType(Defines);
        Base = Symbol.CreateInternalFromSyntax(grammar, syntax.Parent);
        Supertypes = syntax.Supertypes.Select(s => Symbol.CreateFromSyntax(grammar, s)).ToFixedSet();
        Parents = Base is null ? Supertypes : Supertypes.Prepend(Base).ToFixedSet();

        supertypeRules = new(() => Supertypes.OfType<InternalSymbol>().Select(s => s.ReferencedRule)
                                             .EliminateRedundantRules().ToFixedSet());
        parentRules = new(() => BaseRule is null ? SupertypeRules
            : SupertypeRules.Prepend(BaseRule).EliminateRedundantRules().ToFixedSet());
        ancestorRules = new(() => BaseRules.Concat(BaseRules.SelectMany(p => p.AncestorRules)).ToFixedSet());
        childRules = new(() => Grammar.Rules.Where(r => r.BaseRules.Contains(this)).ToFixedSet());
        descendantRules = new(() => ChildRules.Concat(ChildRules.SelectMany(r => r.DescendantRules)).ToFixedSet());

        DeclaredProperties = syntax.DeclaredProperties.Select(p => new Property(this, p)).ToFixedList();
        inheritedProperties = new(() => BaseRules.SelectMany(r => r.AllProperties).Distinct().ToFixedList());
        supertypeProperties = new(() => SupertypeRules.SelectMany(r => r.AllProperties)
                                                      .Except(BaseRule?.AllProperties ?? Enumerable.Empty<Property>())
                                                      .Distinct().ToFixedList());
        allProperties = new(() =>
        {
            var rulePropertyNames = DeclaredProperties.Select(p => p.Name).ToFixedSet();
            return DeclaredProperties
                   .Concat(InheritedProperties.Where(p => !rulePropertyNames.Contains(p.Name)))
                   .ToFixedList();
        });
        differentProperties = new(()
            => ExtendsRule is null
                ? FixedList.Empty<Property>()
                : AllProperties.Where(CouldBeModified)
                               .Except(ExtendsRule.AllProperties, Property.NameAndTypeComparer)
                               .ToFixedList());
        modifiedProperties = new(() => ExtendsRule is null ? FixedList.Empty<Property>()
            : AllProperties.Where(CouldBeModified)
                           .Except(ExtendsRule.AllProperties, Property.NameAndTypeEquivalenceComparer)
                           .ToFixedList());
        ancestorProperties = new(() => AncestorRules.SelectMany(r => r.DeclaredProperties).ToFixedSet());

        extendsRule = new(() => grammar.Language.Extends?.Grammar.RuleFor(Defines.ShortName));
        isNew = new(() => ExtendsRule is null);
        isModified = new(() =>
        {
            var oldRule = ExtendsRule;
            if (oldRule is null)
                return false;

            var currentProperties = AllProperties.ToFixedSet();
            var oldProperties = oldRule.AllProperties.ToFixedSet();
            return !currentProperties.SequenceEqual(oldProperties, Property.NameAndTypeEquivalenceComparer);
        });

        definedInLanguage = new(() => IsNew || IsModified ? grammar.Language : grammar.Language.Extends?.Grammar.RuleFor(Defines.ShortName)?.DefinedInLanguage!);
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
            return Enumerable.Empty<Property>();

        return inheritedProperties
               .SelectMany(p => p.Rule.InheritedPropertiesNamed(propertyName))
               .Distinct()
               .Except(inheritedProperties);
    }

    private bool ComputeDescendantsModified()
    {
        return IsModified
               // Note: added child rules are not a problem for the cases that matter here
               || ChildRules.Any(r => r.TryDescendantsModified)
               || AllProperties
                  .Select(p => p.Type.UnderlyingSymbol)
                  .OfType<InternalSymbol>()
                  .Select(s => s.ReferencedRule)
                  .Except(this)
                  .Any(r => r.TryDescendantsModified);
    }

    private static void DescendantsModifiedInCycle()
        => throw new InvalidOperationException("Cycle detected while computing descendants modified");

    private static bool CouldBeModified(Property property)
        => property.Type.UnderlyingSymbol is InternalSymbol { ReferencedRule.DescendantsModified: true }
            or ExternalSymbol;
}
