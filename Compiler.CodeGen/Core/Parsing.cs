using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Core;

internal static class Parsing
{
    public const string DirectiveMarker = "◊";
    public const char StatementTerminator = ';';

    public static IEnumerable<string> ParseLines(string input)
    {
        return input.ToLines()
                    .Select(l => l.TrimComment().Trim())
                    .Where(s => !string.IsNullOrEmpty(s));
    }

    public static string? GetConfig(IEnumerable<string> lines, string config)
    {
        var start = ConfigStart(config);
        var line = lines.SingleOrDefault(l => l.StartsWith(start, StringComparison.InvariantCulture));
        line = line?[start.Length..];
        line = line?.TrimStatementTerminator();
        line = line?.Trim();
        return line;
    }

    public static string GetListConfig(IFixedList<string> lines) => GetConfig(lines, "list") ?? "List";

    public static string GetSetConfig(IFixedList<string> lines) => GetConfig(lines, "set") ?? "HashSet";

    private static string ConfigStart(string config) => DirectiveMarker + config + " ";

    public static IEnumerable<string> ParseUsingNamespaces(IEnumerable<string> lines)
    {
        const string start = DirectiveMarker + "using";
        lines = lines.Where(l => l.StartsWith(start, StringComparison.InvariantCulture));
        return lines.Select(l => l[start.Length..].TrimStatementTerminator().Trim());
    }

    public static string TrimStatementTerminator(this string line)
    {
        if (!line.EndsWith(StatementTerminator))
            throw new FormatException($"Line does not end with a semicolon: '{line}'");
        return line.TrimEnd(StatementTerminator);
    }

    public static string TrimComment(this string line)
    {
        var commentIndex = line.IndexOf("//", StringComparison.InvariantCulture);
        // Must trim end because we often assume no trailing whitespace
        return commentIndex == -1 ? line : line[..commentIndex].TrimEnd();
    }

    [return: NotNullIfNotNull(nameof(symbol))]
    public static SymbolNode? ParseSymbol(string? symbol)
    {
        if (symbol is null) return null;
        if (symbol.StartsWith('`') && symbol.EndsWith('`'))
            return new SymbolNode(symbol[1..^1], true);
        return new SymbolNode(symbol);
    }

    public static IEnumerable<string> ParseToStatements(IEnumerable<string> lines)
    {
        var currentStatement = new StringBuilder();
        foreach (var line in lines)
        {
            var isConfig = line.StartsWith(DirectiveMarker, StringComparison.InvariantCulture);
            var isEndOfStatement = line.EndsWith(StatementTerminator);
            if (!isConfig)
            {
                currentStatement.AppendSeparator(' ');
                currentStatement.Append(isEndOfStatement ? line.TrimStatementTerminator() : line);
            }
            if ((isConfig || isEndOfStatement) && currentStatement.Length > 0)
            {
                yield return currentStatement.ToString().Trim();
                currentStatement.Clear();
            }
        }

        if (currentStatement.Length > 0)
            yield return currentStatement.ToString().Trim();
    }

    public static IEnumerable<RuleNode> ParseRules(IEnumerable<string> lines)
    {
        var statements = ParseToStatements(lines).ToFixedList();
        foreach (var statement in statements)
            yield return ParseRule(statement);
    }

    public static RuleNode ParseRule(string statement)
    {
        var (declaration, definition) = SplitDeclarationAndDefinition(statement);
        var (defines, parent, supertypes) = ParseDeclaration(declaration);
        var properties = ParseProperties(definition).ToFixedList();
        return new RuleNode(defines, parent, supertypes, properties);
    }

    public static (string Declaration, string? Definition) SplitDeclarationAndDefinition(string statement)
    {
        var equalSplit = statement.Split('=');
        if (equalSplit.Length > 2)
            throw new FormatException($"Too many equal signs on line: '{statement}'");
        return (equalSplit[0].Trim(), equalSplit.Length > 1 ? equalSplit[1].Trim() : null);
    }

    private static IEnumerable<PropertyNode> ParseProperties(string? definition)
    {
        if (definition is null) yield break;

        var properties = SplitProperties(definition);
        foreach (var property in properties)
            yield return ParseProperty(property);
    }

    public static PropertyNode ParseProperty(string property)
    {
        var isOptional = property.EndsWith('?');
        property = isOptional ? property[..^1] : property;

        var parts = property.Split(':').Select(p => p.Trim()).ToArray();

        switch (parts.Length)
        {
            case 1:
            {
                var name = parts[0];
                var collectionKind = ParseCollectionKind(ref name);
                var grammarType = new TypeNode(ParseSymbol(name), collectionKind, isOptional);
                return new PropertyNode(name, grammarType);
            }
            case 2:
            {
                var name = parts[0];
                var type = parts[1];
                var collectionKind = ParseCollectionKind(ref type);
                var grammarType = new TypeNode(ParseSymbol(type), collectionKind, isOptional);
                return new PropertyNode(name, grammarType);
            }
            default:
                throw new FormatException($"Too many colons in property: '{property}'");
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

    private static (SymbolNode Defines, SymbolNode? Parent, IEnumerable<SymbolNode> Supertypes) ParseDeclaration(string declaration)
    {
        var (defines, parent, parents) = SplitDeclaration(declaration);
        var definesSymbol = ParseSymbol(defines);
        var parentSymbol = ParseSymbol(parent);
        var supertypes = ParseSupertypes(parents);
        return (definesSymbol, parentSymbol, supertypes);
    }

    public static (string Defines, string? Parent, string? Parents) SplitDeclaration(string declaration)
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

        string? parent = null;
        if (remainder.Contains(':'))
        {
            var split = remainder.Split(':');
            if (split.Length > 2)
                throw new FormatException($"Too many colons in declaration: '{declaration}'");
            remainder = split[0].Trim();
            parent = split[1].Trim();
        }

        return (remainder, parent, parents);
    }

    private static IEnumerable<SymbolNode> ParseSupertypes(string? parents)
    {
        if (parents is null) return Enumerable.Empty<SymbolNode>();

        return SplitParents(parents)
               .Select(p => ParseSymbol(p));
    }

    public static IEnumerable<string> SplitParents(string parents)
        => parents.Split(',').Select(p => p.Trim());

    public static IEnumerable<RuleNode> ParseRules(IFixedList<string> lines, SymbolNode? defaultParent)
        => ParseRules(lines).Select(r => r.WithDefaultParent(defaultParent));
}
