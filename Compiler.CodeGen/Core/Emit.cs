using System.Linq;
using System.Text;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Core.Config;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Core;

internal static class Emit
{
    public static string ClosedAttribute(Grammar grammar, GrammarRule rule, string indent = "")
    {
        var children = grammar.ChildRules(rule);
        if (!children.Any()) return "";
        var builder = new StringBuilder();
        builder.Append(indent);
        builder.AppendLine("[Closed(");
        bool first = true;
        foreach (var child in children)
        {
            if (first)
                first = false;
            else
                builder.AppendLine(",");
            builder.Append(indent);
            builder.Append($"    typeof({grammar.TypeName(child.Nonterminal)})");
        }

        builder.AppendLine(")]");
        return builder.ToString();
    }

    public static string BaseTypes(Grammar grammar, GrammarRule rule)
    {
        var parents = rule.Parents.Select(grammar.TypeName);
        if (!rule.Parents.Any()) return "";

        return " : " + string.Join(", ", parents);
    }

    public static string Type(Grammar grammar, GrammarType type)
    {
        var value = grammar.TypeName(type.Symbol);
        if (type.IsRef)
        {
            value = "ref " + value;
            if (type.IsOptional) value = "[DisallowNull] " + value;
        }

        if (type.IsList) value = $"{grammar.ListType}<{value}>";
        if (type.IsOptional) value += "?";
        return value;
    }

    public static string PropertyIsNew(Grammar grammar, GrammarRule rule, GrammarProperty property)
        => grammar.IsNewDefinition(rule, property) ? "new " : "";
}
