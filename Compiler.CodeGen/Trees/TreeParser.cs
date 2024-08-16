using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Core;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Trees;

internal static class TreeParser
{
    public static TreeSyntax Parse(string treeDefinition)
    {
        var lines = Parsing.ParseLines(treeDefinition).ToFixedList();

        var ns = Parsing.GetRequiredConfig(lines, "namespace");
        var rootType = Parsing.ParseSymbol(Parsing.GetConfig(lines, "root"));
        var symbolPrefix = Parsing.GetConfig(lines, "prefix") ?? "";
        var symbolSuffix = Parsing.GetConfig(lines, "suffix") ?? "";
        var classPrefix = Parsing.GetConfig(lines, "class-prefix") ?? "";
        var classSuffix = Parsing.GetConfig(lines, "class-suffix") ?? "";
        var usingNamespaces = Parsing.ParseUsingNamespaces(lines);
        var rules = ParseRules(lines);
        return new TreeSyntax(ns, rootType, symbolPrefix, symbolSuffix, classPrefix, classSuffix,
            usingNamespaces, rules);
    }

    public static IEnumerable<TreeNodeSyntax> ParseRules(IEnumerable<string> lines)
    {
        var statements = Parsing.ParseToStatements(lines).ToFixedList();
        foreach (var statement in statements)
            yield return ParseRule(statement);
    }

    public static TreeNodeSyntax ParseRule(string statement)
    {
        var (declaration, definition) = SplitDeclarationAndDefinition(statement);
        var (defines, supertypes) = ParseDeclaration(declaration);
        var properties = EnumerableExtensions.ToFixedList<PropertySyntax>(ParseProperties(definition));
        return new TreeNodeSyntax(defines, supertypes, properties);
    }

    public static (string Declaration, string? Definition) SplitDeclarationAndDefinition(string statement)
    {
        var equalSplit = statement.Split('=');
        if (equalSplit.Length > 2)
            throw new FormatException($"Too many equal signs on line: '{statement}'");
        return (equalSplit[0].Trim(), equalSplit.Length > 1 ? equalSplit[1].Trim() : null);
    }

    private static IEnumerable<PropertySyntax> ParseProperties(string? definition)
    {
        if (definition is null) yield break;

        var properties = SplitProperties(definition);
        foreach (var property in properties)
            yield return ParseProperty(property);
    }

    public static PropertySyntax ParseProperty(string property)
        => ParseBinding(property, null, (name, type) => new PropertySyntax(name, type));

    private static T ParseBinding<T>(string property, string? defaultName, Func<string, TypeSyntax, T> create)
    {
        var isOptional = property.EndsWith('?');
        property = isOptional ? property[..^1] : property;

        var parts = property.Split(':').Select(p => p.Trim()).ToArray();

        switch (parts.Length)
        {
            case 1:
            {
                var type = parts[0];
                var collectionKind = ParseCollectionKind(ref type);
                var grammarType = new TypeSyntax(Parsing.ParseSymbol(type), collectionKind, isOptional);
                var name = defaultName ?? grammarType.Symbol.Text;
                return create(name, grammarType);
            }
            case 2:
            {
                var name = parts[0];
                var type = parts[1];
                var collectionKind = ParseCollectionKind(ref type);
                var grammarType = new TypeSyntax(Parsing.ParseSymbol(type), collectionKind, isOptional);
                return create(name, grammarType);
            }
            default:
                throw new FormatException($"Too many colons in binding: '{property}'");
        }
    }

    private static CollectionKind ParseCollectionKind(ref string type)
    {
        var isList = type.EndsWith('*');
        type = isList ? type[..^1] : type;

        var isSet = type.StartsWith('{') && type.EndsWith('}');
        type = isSet ? type[1..^1] : type;

        if (isList && isSet) throw new FormatException("Property cannot be both a list and a set");
        if (isList) return CollectionKind.List;
        if (isSet) return CollectionKind.Set;
        return CollectionKind.None;
    }

    public static IEnumerable<string> SplitProperties(string definition)
        => definition.SplitOrEmpty(' ').Where(v => !string.IsNullOrWhiteSpace(v));

    private static (SymbolSyntax Defines, IEnumerable<SymbolSyntax> Supertypes) ParseDeclaration(
        string declaration)
    {
        var (defines, parents) = SplitDeclaration(declaration);
        var definesSymbol = Parsing.ParseSymbol(defines);
        var supertypes = ParseSupertypes(parents);
        return (definesSymbol, supertypes);
    }

    public static (string Defines, string? Parents) SplitDeclaration(string declaration)
    {
        var remainder = declaration.Trim();
        string? parents = null;
        if (remainder.Contains("<:"))
        {
            var split = remainder.Split("<:");
            if (split.Length > 2)
                throw new FormatException($"Too many `<:` in declaration: '{declaration}'");
            remainder = split[0].Trim();
            parents = split[1].Trim();
        }


        return (remainder, parents);
    }

    private static IEnumerable<SymbolSyntax> ParseSupertypes(string? parents)
    {
        if (parents is null) return [];
        return Enumerable.Select<string, SymbolSyntax>(SplitSupertypes(parents), p => Parsing.ParseSymbol(p));
    }

    public static IEnumerable<string> SplitSupertypes(string parents)
        => parents.Split(',').Select(p => p.Trim());
}
