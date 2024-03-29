using System.Collections.Generic;
using System.Linq;
using System.Text;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Config;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen;

public partial class TreeCodeTemplate
{
    private readonly Grammar grammar;

    public TreeCodeTemplate(Grammar grammar)
    {
        this.grammar = grammar;
    }

    private IEnumerable<string> OrderedNamespaces()
    {
        return grammar.UsingNamespaces
                      .Append("ExhaustiveMatching")
                      .Distinct()
                      .OrderBy(v => v, NamespaceComparer.Instance);
    }

    private string TypeName(GrammarSymbol symbol)
        => symbol.IsQuoted ? symbol.Text : $"{grammar.Prefix}{symbol.Text}{grammar.Suffix}";

    /// <summary>
    /// A property needs declared under three conditions:
    /// 1. there is no definition of the property in the parent
    /// 2. the single parent definition has a different type
    /// 3. the property is defined in multiple parents, in that case it is
    ///    ambiguous unless it is redefined in the current interface.
    /// </summary>
    private bool NeedsDeclared(GrammarRule rule, GrammarProperty property)
    {
        var baseProperties = BaseProperties(rule, property.Name).ToList();
        return baseProperties.Count != 1 || baseProperties[0].Type != property.Type;
    }

    /// <summary>
    /// Something is a new definition if it replaces some parent definition.
    /// </summary>
    private bool IsNewDefinition(GrammarRule rule, GrammarProperty property)
        => BaseProperties(rule, property.Name).Any();

    private string TypeName(GrammarType type)
    {
        var value = TypeName(type.Symbol);
        if (type.IsRef)
        {
            value = "ref " + value;
            if (type.IsOptional) value = "[DisallowNull] " + value;
        }
        if (type.IsOptional) value += "?";
        if (type.IsList) value = $"{grammar.ListType}<{value}>";
        return value;
    }

    private string BaseTypes(GrammarRule rule)
    {
        var parents = rule.Parents.Select(TypeName);
        if (!rule.Parents.Any())
            return "";

        return " : " + string.Join(", ", parents);
    }

    /// <summary>
    /// Definitions for the property on the parents of the rule.
    /// </summary>
    private IEnumerable<GrammarProperty> BaseProperties(GrammarRule rule, string propertyName)
        => grammar.ParentRules(rule).SelectMany(r => PropertyDefinitions(r, propertyName));

    /// <summary>
    /// Get the property definitions for a rule. If that rule defines the property itself, that
    /// is the one definition. When the rule doesn't define the property, base classes are
    /// recursively searched for definitions. Multiple definitions are returned when multiple
    /// parents of a rule contain definitions of the property without it being defined on that rule.
    /// </summary>
    private IEnumerable<GrammarProperty> PropertyDefinitions(GrammarRule rule, string propertyName)
    {
        var property = rule.Properties.SingleOrDefault(p => p.Name == propertyName);
        if (property is not null) return property.Yield();

        return BaseProperties(rule, propertyName);
    }

    private IFixedList<GrammarRule> ChildRules(GrammarRule rule)
        => grammar.Rules.Where(r => r.Parents.Contains(rule.Nonterminal)).ToFixedList();

    private string ClosedType(GrammarRule rule)
    {
        var children = ChildRules(rule);
        if (!children.Any()) return "";
        var builder = new StringBuilder();
        builder.AppendLine("[Closed(");
        var lastChild = children[^1];
        foreach (var child in children)
        {
            builder.Append($"    typeof({TypeName(child.Nonterminal)})");
            builder.AppendLine(child == lastChild ? ")]" : ",");
        }
        return builder.ToString();
    }
}
