using System.Linq;
using System.Text;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Core;

internal static class Emit
{
    public static string ClosedAttribute(Rule rule, string indent = "")
    {
        var children = rule.ChildRules;
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
            builder.Append($"    typeof({child.Defines.Name})");
        }

        builder.AppendLine(")]");
        return builder.ToString();
    }

    public static string BaseTypes(Rule rule, string? root = null)
    {
        bool anyParents = rule.ParentRules.Any();
        if (!anyParents && root is null) return "";

        var parents = rule.ParentRules.Select(r => r.Defines.Name);
        if (root is not null && !anyParents)
            parents = parents.Append(root);

        return " : " + string.Join(", ", parents);
    }

    public static string CommonClosedAttribute(Rule rule, string indent = "")
    {
        var children = rule.ChildRules;
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
            builder.Append($"    typeof({CommonTypeName(child)})");
        }

        builder.AppendLine(")]");
        return builder.ToString();
    }


    public static string CommonBaseTypes(Rule rule)
    {
        var languageTypeName = QualifiedTypeName(rule.Defines);
        var parents = rule.ParentRules.Select(CommonTypeName);
        return string.Join(", ", parents.Prepend(languageTypeName));
    }

    public static string TypeName(Type type)
        => TypeName(type.Symbol);

    public static string TypeName(Symbol symbol)
        => symbol.Name;

    public static string QualifiedTypeName(Type type)
        => QualifiedTypeName(type.Symbol);

    public static string QualifiedTypeName(Symbol symbol)
    {
        var languageName = symbol.ReferencedRule?.Language.Name;
        return languageName is null ? symbol.Name : $"{languageName}.{symbol.Name}";
    }

    public static string CommonTypeName(Rule rule)
        => CommonTypeName(rule.Defines);

    public static string CommonTypeName(Type type) => CommonTypeName(type.Symbol);

    public static string CommonTypeName(Symbol symbol)
        => symbol.Syntax.IsQuoted ? symbol.Name : $"Common{symbol.Name}";

    public static string Type(Type type)
    {
        var name = TypeName(type);
        return TypeDecorations(type, name);
    }

    public static string QualifiedType(Type type)
    {
        var name = QualifiedTypeName(type);
        return TypeDecorations(type, name);
    }

    public static string CommonType(Type type)
    {
        var name = CommonTypeName(type.Symbol);
        return TypeDecorations(type, name);
    }

    public static string ClassType(Type type)
    {
        var name = ClassName(type.Language, type.Symbol);
        return TypeDecorations(type, name);
    }

    private static string TypeDecorations(Type type, string name)
    {
        if (type.IsList) name = $"{type.Grammar.ListType}<{name}>";
        if (type.IsOptional) name += "?";
        return name;
    }

    public static string ClassModifier(Rule rule)
        => rule.IsTerminal ? "sealed" : "abstract";

    public static string PropertyIsNew(Property property)
        => property.IsNewDefinition ? "new " : "";

    public static string ClassName(Language language, Symbol? symbol)
    {
        if (symbol is null) return $"{language.Grammar.Suffix}Node";
        return symbol.Syntax.IsQuoted ? symbol.Syntax.Text : $"{symbol.Syntax.Text}{language.Grammar.Suffix}Node";
    }

    public static string PropertyParameters(Rule rule)
        => string.Join(", ", rule.AllProperties.Select(p => $"{ParameterType(p.Type)} {p.Name.ToCamelCase()}"));

    public static string ModifiedPropertyParameters(Rule rule)
    {
        var correctLanguage = rule.Language.Extends!;
        string typeName = TypeName(rule.Defines);
        var oldProperties = OldProperties(rule);
        return string.Join(", ", rule.AllProperties.Where(p => !oldProperties.Contains(p.Name))
                                     .Select(p => $"{ParameterType(p.Type)} {p.Name.ToCamelCase()}")
                                     .Prepend($"{correctLanguage.Name}.{typeName} {typeName.ToCamelCase()}"));
    }

    private static string ParameterType(Type type)
    {
        var name = TypeName(type);
        return ParameterTypeDecorations(type, name);
    }

    private static string ParameterTypeDecorations(Type type, string name)
    {
        if (type.IsList) name = $"IEnumerable<{name}>";
        if (type.IsOptional) name += "?";
        return name;
    }

    private static IFixedSet<string> OldProperties(Rule rule)
        => rule.ExtendsRule!.AllProperties.Select(p => p.Name).ToFixedSet();

    public static string PropertyClassParameters(Rule rule)
        => string.Join(", ", rule.AllProperties.Select(p => $"{CommonType(p.Type)} {p.Name.ToCamelCase()}"));

    public static string PropertyArguments(Rule rule)
    {
        return string.Join(", ", rule.AllProperties.Select(ToArgument));
        static string ToArgument(Property p)
        {
            if (p.Type.IsList)
                return $"{p.Name.ToCamelCase()}.CastToFixedList<{CommonTypeName(p.Type)}>()";
            var cast = p.ReferencesRule ? $"({CommonTypeName(p.Type)})" : "";
            return $"{cast}{p.Name.ToCamelCase()}";
        }
    }

    public static string ModifiedPropertyArguments(Rule rule)
    {
        string oldNode = TypeName(rule.Defines).ToCamelCase();
        var correctLanguage = rule.DefinedInLanguage;
        var oldProperties = OldProperties(rule);

        return string.Join(", ", rule.AllProperties.Select(ToArgument));

        string ToArgument(Property p)
        {
            var cast = p.ReferencesRule ? $"({SmartClassType(p.Type)})" : "";
            return oldProperties.Contains(p.Name) ? $"{cast}{oldNode}.{p.Name}" : $"{cast}{p.Name.ToCamelCase()}";
        }
    }

    public static string SmartClassType(Type type)
    {
        string name = SmartClassName(type.Symbol);
        return TypeDecorations(type, name);
    }

    public static string SmartClassName(Symbol symbol)
    {
        var correctLanguage = symbol.ReferencedRule!.DefinedInLanguage;
        return ClassName(correctLanguage, symbol);
    }
}
