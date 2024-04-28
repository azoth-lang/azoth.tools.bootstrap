using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Core;

internal static class Build
{
    public static IEnumerable<string> OrderedNamespaces(Grammar grammar, params string[] additionalNamespaces)
        => grammar.UsingNamespaces.Concat(additionalNamespaces).Distinct().OrderBy(v => v, NamespaceComparer.Instance);

    public static IEnumerable<string> OrderedNamespaces(Pass pass, params string[] additionalNamespaces)
    {
        var fromNamespaces = pass.FromLanguage?.Grammar.UsingNamespaces ?? FixedSet.Empty<string>();
        var toNamespaces = pass.ToLanguage?.Grammar.UsingNamespaces ?? FixedSet.Empty<string>();

        return pass.UsingNamespaces
                   .Concat(fromNamespaces)
                   .Concat(toNamespaces)
                   .Concat(additionalNamespaces)
                   .Distinct().OrderBy(v => v, NamespaceComparer.Instance);
    }

    public static IEnumerable<Rule> SimpleCreateRules(Pass pass)
    {
        if (pass.FromLanguage == pass.ToLanguage || pass.ToLanguage is null)
            return Enumerable.Empty<Rule>();
        return pass.ToLanguage.Grammar.Rules.Where(r => r is { IsTerminal: true, ExtendsRule: not null });
    }

    public static IEnumerable<Rule> AdvancedCreateRules(Pass pass)
    {
        if (pass.FromLanguage == pass.ToLanguage || pass.ToLanguage is null)
            return Enumerable.Empty<Rule>();
        return pass.Transforms
                   .Where(t => t.From?.Type.ToNonOptional() is SymbolType { Symbol: InternalSymbol }
                               && t.To?.Type.ToNonOptional() is SymbolType { Symbol: InternalSymbol })
                   .Select(t => (t.To?.Type.UnderlyingSymbol as InternalSymbol)?.ReferencedRule)
                   .WhereNotNull()
                   .Where(r => DifferentChildProperties(r).Any()
                               || AdvancedCreateChildParameters(pass, r).Any());
    }

    public static IEnumerable<Parameter> AdvancedCreateChildParameters(Pass pass, Rule rule)
        => DifferentChildProperties(rule)
               .Where(Emit.CouldBeModified)
               .Select(p => CalledTransform(pass, FromType(p))).WhereNotNull()
               .SelectMany(t => t.AdditionalParameters)
               .Select(p => p.ChildParameter)
               .Concat(rule.ChildRules
                           .Select(r => CalledTransform(pass, new SymbolType(r.Defines)))
                           .WhereNotNull()
                           .SelectMany(t => t.AdditionalParameters))
               .MergeByName();

    private static IEnumerable<Property> DifferentChildProperties(Rule rule)
        => rule.DifferentProperties.Except(rule.ModifiedProperties);

    public static Transform? CalledTransform(Transform transform, NonVoidType fromType)
        => CalledTransform(transform.Pass, fromType);

    public static Transform? CalledTransform(Pass pass, NonVoidType fromType)
        => pass.Transforms.SingleOrDefault(t => fromType == t.From?.Type
            || (t.From?.Type is OptionalType optionalType && optionalType.UnderlyingType == fromType));

    public static IEnumerable<Parameter> AdvancedCreateParameters(Pass pass, Rule rule)
    {
        var extendsRule = rule.ExtendsRule!;
        var modifiedProperties = rule.ModifiedProperties;
        var fromType = new SymbolType(extendsRule.Defines);
        var parameters = new List<Parameter> { Parameter.Create(fromType, "from") };
        parameters.AddRange(modifiedProperties.Select(p => p.Parameter));
        parameters.AddRange(AdvancedCreateChildParameters(pass, rule));
        return parameters.MergeByName();
    }

    public static IEnumerable<Parameter> StartRunReturnValues(Pass pass)
        => pass.EntryTransform.AdditionalParameters;

    public static IEnumerable<Parameter> EndRunReturnValues(Pass pass)
        => pass.ToContextParameter.YieldValue();

    public static NonVoidType FromType(Property property)
    {
        var fromSymbol = ((InternalSymbol)property.Type.UnderlyingSymbol).ReferencedRule.ExtendsRule!.Defines;
        var fromType = property.Type.WithSymbol(fromSymbol);
        return fromType;
    }
}
