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
        var statements = Parsing.ParseToStatements(lines).ToFixedList();
        foreach (var statement in statements)
            yield return ParseRule(statement);
    }

    private static GrammarRule ParseRule(string statement)
    {
        var equalSplit = statement.Split('=');
        if (equalSplit.Length > 2) throw new FormatException($"Too many equal signs on line: '{statement}'");
        var declaration = equalSplit[0];
        var (nonterminal, parents) = ParseDeclaration(declaration);
        var definition = equalSplit.Length == 2 ? equalSplit[1].Trim() : null;
        var properties = ParseProperties(definition).ToList();
        if (properties.Select(p => p.Name).Distinct().Count() != properties.Count)
            throw new FormatException($"Rule for {nonterminal} contains duplicate property definitions");

        return new GrammarRule(nonterminal, parents, properties);
    }

    private static IEnumerable<GrammarProperty> ParseProperties(string? definition)
    {
        if (definition is null) yield break;

        var properties = definition.SplitOrEmpty(' ').Where(v => !string.IsNullOrWhiteSpace(v));
        foreach (var property in properties)
        {
            var trimmedProperty = property;

            var isRef = trimmedProperty.StartsWith('&');
            trimmedProperty = isRef ? trimmedProperty[1..] : trimmedProperty;

            var isOptional = trimmedProperty.EndsWith('?');
            trimmedProperty = isOptional ? trimmedProperty[..^1] : trimmedProperty;

            var isList = trimmedProperty.EndsWith('*');
            trimmedProperty = isList ? trimmedProperty[..^1] : trimmedProperty;

            var parts = trimmedProperty.Split(':').Select(p => p.Trim()).ToArray();

            switch (parts.Length)
            {
                case 1:
                {
                    var name = parts[0];
                    var grammarType = new GrammarType(ParseSymbol(name), isRef, isList, isOptional);
                    yield return new GrammarProperty(name, grammarType);
                }
                break;
                case 2:
                {
                    var name = parts[0];
                    var type = parts[1];
                    var grammarType = new GrammarType(ParseSymbol(type), isRef, isList, isOptional);
                    yield return new GrammarProperty(name, grammarType);
                }
                break;
                default:
                    throw new FormatException($"Too many colons in definition: '{definition}'");
            }
        }
    }

    private static (GrammarSymbol nonterminal, IEnumerable<GrammarSymbol> parents) ParseDeclaration(string declaration)
    {
        var declarationSplit = declaration.SplitOrEmpty(':');
        if (declarationSplit.Count > 2) throw new FormatException($"Too many colons in declaration: '{declaration}'");
        var nonterminal = ParseSymbol(declarationSplit[0].Trim());
        var parents = declarationSplit.Count == 2 ? declarationSplit[1] : null;
        var parentSymbols = ParseParents(parents);
        return (nonterminal, parentSymbols);
    }

    private static IEnumerable<GrammarSymbol> ParseParents(string? parents)
    {
        if (parents is null) return Enumerable.Empty<GrammarSymbol>();

        return parents
               .Split(',')
               .Select(p => p.Trim())
               .Select(p => ParseSymbol(p));
    }
}
