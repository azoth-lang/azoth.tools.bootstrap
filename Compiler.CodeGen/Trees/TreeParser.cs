using System;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Types;
using Azoth.Tools.Bootstrap.Framework;
using static Azoth.Tools.Bootstrap.Compiler.CodeGen.Core.Parsing;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Trees;

internal static class TreeParser
{
    public static TreeSyntax Parse(string treeName, string treeDefinition)
    {
        var lines = ParseLines(treeDefinition).ToFixedList();

        var ns = GetRequiredConfig(lines, "namespace");
        var rootType = ParseSymbol(GetConfig(lines, "root-supertype"));
        var symbolPrefix = GetConfig(lines, "prefix") ?? "";
        var symbolSuffix = GetConfig(lines, "suffix") ?? "";
        var generateClasses = GetBoolConfig(lines, "gen-classes", true);
        var simplifiedTree = GetBoolConfig(lines, "simplified-tree", false);
        var classPrefix = GetConfig(lines, "class-prefix") ?? "";
        var classSuffix = GetConfig(lines, "class-suffix") ?? "";
        var usingNamespaces = ParseUsingNamespaces(lines);
        var rules = ParseNodes(lines);
        return new(treeName, ns, rootType, symbolPrefix, symbolSuffix, generateClasses, simplifiedTree,
            classPrefix, classSuffix, usingNamespaces, rules);
    }

    private static IEnumerable<TreeNodeSyntax> ParseNodes(IEnumerable<string> lines)
    {
        var statements = ParseToStatements(lines).ToFixedList();
        foreach (var statement in statements)
            yield return ParseNode(statement);
    }

    private static TreeNodeSyntax ParseNode(string statement)
    {
        var (declaration, definition) = SplitDeclarationAndDefinition(statement);
        var (isTemp, isAbstract, defines, supertypes) = ParseDeclaration(declaration);
        var attributes = ParseTreeAttributes(definition).ToFixedList();
        return new(isTemp, isAbstract, defines, supertypes, attributes);
    }

    private static (string Declaration, string? Definition) SplitDeclarationAndDefinition(string statement)
        => OptionalBisect(statement, "=", "Too many equal signs in: '{0}'");

    private static IEnumerable<TreeAttributeSyntax> ParseTreeAttributes(string? definition)
    {
        if (definition is null) yield break;

        var attributes = SplitAttributes(definition);
        foreach (var attribute in attributes)
            yield return ParseTreeAttribute(attribute);
    }

    public static TreeAttributeSyntax ParseTreeAttribute(string attribute)
    {
        if (attribute.StartsWith('/') || attribute.EndsWith('/'))
            return ParsePlaceholderSyntax(attribute);

        return ParseProperty(attribute);
    }

    private static PlaceholderSyntax ParsePlaceholderSyntax(string attribute)
    {
        if (!ParseOffStart(ref attribute, "/") || !ParseOffEnd(ref attribute, "/"))
            throw new FormatException("Placeholder must start and end with a forward slash: '{0}'");

        return new(attribute);
    }

    public static PropertySyntax ParseProperty(string property)
    {
        var (name, type) = OptionalBisect(property, ":", "Too many colons in binding: '{0}'");
        TypeSyntax typeSyntax;
        if (type is null)
        {
            typeSyntax = ParseType(name);
            name = typeSyntax.UnderlyingSymbol.Text;
        }
        else
            typeSyntax = ParseType(type);

        return new(name, typeSyntax);
    }

    private static IEnumerable<string> SplitAttributes(string definition)
    {
        var last = 0;
        while (last < definition.Length)
        {
            var nextWhitespace = definition.IndexOfWhitespace(last) ?? definition.Length;
            var nextBacktick = definition.IndexOf('`', last);
            if (nextBacktick < 0) nextBacktick = definition.Length;

            if (nextBacktick < nextWhitespace)
            {
                var nextBacktickEnd = definition.IndexOf('`', nextBacktick + 1);
                if (nextBacktickEnd < 0) nextBacktickEnd = definition.Length;
                nextWhitespace = definition.IndexOfWhitespace(nextBacktickEnd) ?? definition.Length;
            }

            if (last < nextWhitespace)
                yield return definition[last..nextWhitespace];

            last = nextWhitespace + 1;
        }
    }

    private static (bool isTemp, bool? isAbstract, SymbolSyntax Defines, IEnumerable<SymbolSyntax> Supertypes) ParseDeclaration(
        string declaration)
    {
        (declaration, var supertypes) = OptionalBisect(declaration, "<:", "Too many `<:` in: '{0}'");
        bool isTemp = ParseIsTemp(ref declaration);
        var isAbstract = ParseIsAbstract(ref declaration);
        var definesSymbol = ParseSymbol(declaration);
        var supertypeSyntax = ParseSupertypes(supertypes);
        return (isTemp, isAbstract, definesSymbol, supertypeSyntax);
    }

    private static bool ParseIsTemp(ref string declaration)
    {
        var (tempKeyword, defines) = OptionalSplitOffStart(declaration);
        if (tempKeyword == "temp")
        {
            declaration = defines;
            return true;
        }
        return false;
    }

    private static bool? ParseIsAbstract(ref string declaration)
    {
        var (keyword, defines) = OptionalSplitOffStart(declaration);
        switch (keyword)
        {
            case "abstract":
                declaration = defines;
                return true;
            case "concrete":
                declaration = defines;
                return false;
            default:
                return null;
        }
    }
}
