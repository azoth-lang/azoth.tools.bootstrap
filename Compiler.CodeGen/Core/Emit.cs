using System.Linq;
using System.Text;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Core;

internal static class Emit
{
    public static string ClosedAttribute(GrammarNode grammar, RuleNode rule, string indent = "")
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
            builder.Append($"    typeof({grammar.TypeName(child.Defines)})");
        }

        builder.AppendLine(")]");
        return builder.ToString();
    }

    public static string BaseTypes(GrammarNode grammar, RuleNode rule, string? root = null)
    {
        bool anyParents = rule.Parents.Any();
        if (!anyParents && root is null) return "";

        var parents = rule.Parents.Select(grammar.TypeName);
        if (root is not null && !anyParents)
            parents = parents.Append(root);

        return " : " + string.Join(", ", parents);
    }

    public static string TypeName(LanguageNode language, TypeNode type)
        => TypeName(language.Grammar, type.Symbol);

    public static string TypeName(GrammarNode grammar, TypeNode type)
        => TypeName(grammar, type.Symbol);

    public static string TypeName(LanguageNode language, Symbol symbol)
        => TypeName(language.Grammar, symbol);

    public static string TypeName(GrammarNode grammar, Symbol symbol)
        => grammar.TypeName(symbol);

    public static string QualifiedTypeName(LanguageNode language, TypeNode type)
        => QualifiedTypeName(language, type.Symbol);

    public static string QualifiedTypeName(LanguageNode language, Symbol symbol)
        => symbol.IsQuoted ? TypeName(language, symbol) : $"{language.Name}.{TypeName(language, symbol)}";

    public static string Type(LanguageNode language, TypeNode type)
        => Type(language.Grammar, type);

    public static string Type(GrammarNode grammar, TypeNode type)
    {
        var name = TypeName(grammar, type);
        return TypeDecorations(grammar, type, name);
    }

    public static string QualifiedType(LanguageNode language, TypeNode type)
    {
        var name = QualifiedTypeName(language, type);
        return TypeDecorations(language.Grammar, type, name);
    }

    private static string TypeDecorations(GrammarNode grammar, TypeNode type, string name)
    {
        if (type.IsList) name = $"{grammar.ListType}<{name}>";
        if (type.IsOptional) name += "?";
        return name;
    }

    public static string ClassType(LanguageNode language, TypeNode type)
    {
        var name = ClassName(language, type.Symbol);
        return TypeDecorations(language.Grammar, type, name);
    }

    public static string ClassModifier(LanguageNode language, RuleNode rule)
        => language.Grammar.IsTerminal(rule) ? "sealed" : "abstract";

    public static string PropertyIsNew(GrammarNode grammar, RuleNode rule, PropertyNode property)
        => grammar.IsNewDefinition(rule, property) ? "new " : "";

    public static string ClassPropertyModifier(LanguageNode language, RuleNode rule)
        => language.Grammar.IsTerminal(rule) ? "override" : "abstract";

    public static string ClassName(LanguageNode language, Symbol? symbol)
    {
        if (symbol is null) return $"{language.Grammar.Suffix}Node";
        return symbol.IsQuoted ? symbol.Text : $"{symbol.Text}{language.Grammar.Suffix}Node";
    }

    public static string PropertyParameters(LanguageNode language, RuleNode rule)
        => string.Join(", ", language.Grammar.AllProperties(rule).Select(p => $"{Type(language, p.Type)} {p.Name.ToCamelCase()}"));

    public static string ModifiedPropertyParameters(LanguageNode language, RuleNode rule)
    {
        var grammar = language.Grammar;
        var correctLanguage = language.Extends?.RuleDefinedIn[rule.Defines]!;
        string typeName = grammar.TypeName(rule.Defines);
        var oldProperties = correctLanguage.Grammar.AllProperties(rule).Select(p => p.Name).ToFixedSet();
        return $"{correctLanguage.Name}.{typeName} {typeName.ToCamelCase()}, " + string.Join(", ",
            grammar.AllProperties(rule).Where(p => !oldProperties.Contains(p.Name))
                   .Select(p => $"{Type(language, p.Type)} {p.Name.ToCamelCase()}"));
    }

    public static string PropertyClassParameters(LanguageNode language, RuleNode rule)
        => string.Join(", ", language.Grammar.AllProperties(rule).Select(p => $"{ClassType(language, p.Type)} {p.Name.ToCamelCase()}"));

    public static string PropertyArguments(LanguageNode language, RuleNode rule)
    {
        return string.Join(", ", language.Grammar.AllProperties(rule).Select(ToArgument));
        string ToArgument(PropertyNode p)
        {
            var cast = language.Grammar.IsNonterminal(p) ? $"({SmartClassType(language, p.Type)})" : "";
            return $"{cast}{p.Name.ToCamelCase()}";
        }
    }

    public static string ModifiedPropertyArguments(LanguageNode language, RuleNode rule)
    {
        string oldNode = language.Grammar.TypeName(rule.Defines).ToCamelCase();
        var correctLanguage = language.Extends?.RuleDefinedIn[rule.Defines]!;
        var oldProperties = correctLanguage.Grammar.AllProperties(rule).Select(p => p.Name).ToFixedSet();

        return string.Join(", ", language.Grammar.AllProperties(rule).Select(ToArgument));

        string ToArgument(PropertyNode p)
        {
            var cast = language.Grammar.IsNonterminal(p) ? $"({SmartClassType(language, p.Type)})" : "";
            return oldProperties.Contains(p.Name) ? $"{cast}{oldNode}.{p.Name}" : $"{cast}{p.Name.ToCamelCase()}";
        }
    }

    public static string SmartClassType(LanguageNode language, TypeNode type)
    {
        string name = SmartClassName(language, type.Symbol);
        return TypeDecorations(language.Grammar, type, name);
    }

    public static string SmartClassName(LanguageNode language, Symbol symbol)
    {
        var correctLanguage = language.RuleDefinedIn[symbol];
        return ClassName(correctLanguage, symbol);
    }
}
