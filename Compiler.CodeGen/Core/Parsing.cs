using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Core.Config;
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
    public static GrammarSymbol? ParseSymbol(string? symbol)
    {
        if (symbol is null) return null;
        if ((symbol.StartsWith('\'') && symbol.EndsWith('\'')
            || (symbol.StartsWith('`') && symbol.EndsWith('`'))))
            return new GrammarSymbol(symbol[1..^1], true);
        return new GrammarSymbol(symbol);
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

    public static IEnumerable<GrammarRule> ParseRules(IEnumerable<string> lines)
    {
        var statements = ParseToStatements(lines).ToFixedList();
        foreach (var statement in statements)
            yield return ParseRule(statement);
    }

    public static GrammarRule ParseRule(string statement)
    {
        var (declaration, definition) = SplitDeclarationAndDefinition(statement);
        var (nonterminal, parents) = ParseDeclaration(declaration);
        var properties = ParseProperties(definition).ToFixedList();
        return new GrammarRule(nonterminal, parents, properties);
    }

    public static (string Declaration, string? Definition) SplitDeclarationAndDefinition(string statement)
    {
        var equalSplit = statement.Split('=');
        if (equalSplit.Length > 2) throw new FormatException($"Too many equal signs on line: '{statement}'");
        return (equalSplit[0].Trim(), equalSplit.Length > 1 ? equalSplit[1].Trim() : null);
    }

    private static IEnumerable<GrammarProperty> ParseProperties(string? definition)
    {
        if (definition is null) yield break;

        var properties = SplitProperties(definition);
        foreach (var property in properties)
            yield return ParseProperty(property);
    }

    public static GrammarProperty ParseProperty(string property)
    {
        var isRef = property.StartsWith('&');
        property = isRef ? property[1..] : property;

        var isOptional = property.EndsWith('?');
        property = isOptional ? property[..^1] : property;

        var isList = property.EndsWith('*');
        property = isList ? property[..^1] : property;

        var parts = property.Split(':').Select(p => p.Trim()).ToArray();

        switch (parts.Length)
        {
            case 1:
            {
                var name = parts[0];
                var grammarType = new GrammarType(ParseSymbol(name), isRef, isList, isOptional);
                return new GrammarProperty(name, grammarType);
            }
            case 2:
            {
                var name = parts[0];
                var type = parts[1];
                var grammarType = new GrammarType(ParseSymbol(type), isRef, isList, isOptional);
                return new GrammarProperty(name, grammarType);
            }
            default:
                throw new FormatException($"Too many colons in property: '{property}'");
        }
    }

    public static IEnumerable<string> SplitProperties(string definition)
        => definition.SplitOrEmpty(' ').Where(v => !string.IsNullOrWhiteSpace(v));

    private static (GrammarSymbol nonterminal, IEnumerable<GrammarSymbol> parents) ParseDeclaration(string declaration)
    {
        var (nonterminalText, parents) = SplitNonterminalAndParents(declaration);
        var nonterminal = ParseSymbol(nonterminalText);
        var parentSymbols = ParseParents(parents);
        return (nonterminal, parentSymbols);
    }

    public static (string Nonterminal, string? Parents) SplitNonterminalAndParents(string declaration)
    {
        var declarationSplit = declaration.Split(':');
        if (declarationSplit.Length > 2) throw new FormatException($"Too many colons in declaration: '{declaration}'");
        return (declarationSplit[0].Trim(), declarationSplit.Length > 1 ? declarationSplit[1].Trim() : null);
    }

    private static IEnumerable<GrammarSymbol> ParseParents(string? parents)
    {
        if (parents is null) return Enumerable.Empty<GrammarSymbol>();

        return SplitParents(parents)
               .Select(p => ParseSymbol(p));
    }

    public static IEnumerable<string> SplitParents(string parents)
        => parents.Split(',').Select(p => p.Trim());

    public static IEnumerable<GrammarRule> ParseRules(IFixedList<string> lines, GrammarSymbol? rootType)
        => ParseRules(lines).Select(r => r.WithDefaultRootType(rootType));
}
