using System.Linq;
using System.Text;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Core.Config;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Languages;

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

    public static string BaseTypes(Grammar grammar, GrammarRule rule, string? root = null)
    {
        bool anyParents = rule.Parents.Any();
        if (!anyParents && root is null) return "";

        var parents = rule.Parents.Select(grammar.TypeName);
        if (root is not null && !anyParents)
            parents = parents.Append(root);

        return " : " + string.Join(", ", parents);
    }

    public static string Type(Grammar grammar, GrammarType type)
    {
        var name = grammar.TypeName(type.Symbol);
        return TypeDecorations(grammar, type, name);
    }

    private static string TypeDecorations(Grammar grammar, GrammarType type, string name)
    {
        if (type.IsRef)
        {
            name = "ref " + name;
            if (type.IsOptional) name = "[DisallowNull] " + name;
        }

        if (type.IsList) name = $"{grammar.ListType}<{name}>";
        if (type.IsOptional) name += "?";
        return name;
    }

    public static string ClassType(Language language, GrammarType type)
    {
        var name = ClassName(language, type.Symbol);
        return TypeDecorations(language.Grammar, type, name);
    }

    public static string PropertyIsNew(Grammar grammar, GrammarRule rule, GrammarProperty property)
        => grammar.IsNewDefinition(rule, property) ? "new " : "";

    public static string ClassName(Language language, GrammarSymbol symbol)
        => symbol.IsQuoted ? symbol.Text : $"{symbol.Text}{language.Grammar.Suffix}_{language.Name}";

    public static string PropertyParameters(Grammar grammar, GrammarRule rule)
        => string.Join(", ", grammar.AllProperties(rule).Select(p => $"{Type(grammar, p.Type)} {p.Name.ToCamelCase()}"));

    public static string PropertyClassParameters(Language language, GrammarRule rule)
        => string.Join(", ", language.Grammar.AllProperties(rule).Select(p => $"{ClassType(language, p.Type)} {p.Name.ToCamelCase()}"));

    public static string PropertyArguments(Language language, GrammarRule rule)
    {
        return string.Join(", ", language.Grammar.AllProperties(rule).Select(ToArgument));
        string ToArgument(GrammarProperty p)
        {
            var cast = language.Grammar.IsNonterminal(p) ? $"({SmartClassType(language, p.Type)})" : "";
            return $"{cast}{p.Name.ToCamelCase()}";
        }
    }

    private static string SmartClassType(Language language, GrammarType type)
    {
        var correctLanguage = language.RuleDefinedIn[type.Symbol];
        var name = ClassName(correctLanguage, type.Symbol);
        return TypeDecorations(language.Grammar, type, name);
    }
}
