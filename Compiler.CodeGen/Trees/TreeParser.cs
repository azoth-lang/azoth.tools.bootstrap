using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;
using Azoth.Tools.Bootstrap.Framework;
using static Azoth.Tools.Bootstrap.Compiler.CodeGen.Core.Parsing;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Trees;

internal static class TreeParser
{
    public static TreeSyntax Parse(string treeDefinition)
    {
        var lines = ParseLines(treeDefinition).ToFixedList();

        var ns = GetRequiredConfig(lines, "namespace");
        var rootType = ParseSymbol(GetConfig(lines, "root"));
        var symbolPrefix = GetConfig(lines, "prefix") ?? "";
        var symbolSuffix = GetConfig(lines, "suffix") ?? "";
        var generateClasses = GetBoolConfig(lines, "gen-classes", true);
        var classPrefix = GetConfig(lines, "class-prefix") ?? "";
        var classSuffix = GetConfig(lines, "class-suffix") ?? "";
        var usingNamespaces = ParseUsingNamespaces(lines);
        var rules = ParseRules(lines);
        return new(ns, rootType, symbolPrefix, symbolSuffix, generateClasses, classPrefix, classSuffix,
            usingNamespaces, rules);
    }

    private static IEnumerable<TreeNodeSyntax> ParseRules(IEnumerable<string> lines)
    {
        var statements = ParseToStatements(lines).ToFixedList();
        foreach (var statement in statements)
            yield return ParseRule(statement);
    }

    private static TreeNodeSyntax ParseRule(string statement)
    {
        var (declaration, definition) = SplitDeclarationAndDefinition(statement);
        var (defines, supertypes) = ParseDeclaration(declaration);
        var properties = ParseProperties(definition).ToFixedList();
        return new(defines, supertypes, properties);
    }

    private static (string Declaration, string? Definition) SplitDeclarationAndDefinition(string statement)
        => OptionalSplitTwo(statement, "=", "Too many equal signs in: '{0}'");

    private static IEnumerable<PropertySyntax> ParseProperties(string? definition)
    {
        if (definition is null) yield break;

        var properties = SplitProperties(definition);
        foreach (var property in properties)
            yield return ParseProperty(property);
    }

    public static PropertySyntax ParseProperty(string property)
    {
        var (name, type) = OptionalSplitTwo(property, ":", "Too many colons in binding: '{0}'");
        TypeSyntax typeSyntax;
        if (type is null)
        {
            typeSyntax = ParseType(name);
            name = typeSyntax.Symbol.Text;
        }
        else
            typeSyntax = ParseType(type);

        return new(name, typeSyntax);
    }

    public static IEnumerable<string> SplitProperties(string definition)
        => definition.SplitOrEmpty(' ').Where(v => !string.IsNullOrWhiteSpace(v));

    private static (SymbolSyntax Defines, IEnumerable<SymbolSyntax> Supertypes) ParseDeclaration(
        string declaration)
    {
        var (defines, supertypes) = SplitDeclaration(declaration);
        var definesSymbol = ParseSymbol(defines);
        var supertypeSyntax = ParseSupertypes(supertypes);
        return (definesSymbol, supertypeSyntax);
    }

    public static (string Defines, string? Parents) SplitDeclaration(string declaration)
        => OptionalSplitTwo(declaration, "<:", "Too many `<:` in: '{0}'");

    private static IEnumerable<SymbolSyntax> ParseSupertypes(string? supertypes)
    {
        if (supertypes is null) return [];
        return SplitCommaSeparated(supertypes).Select(s => ParseSymbol(s));
    }
}
