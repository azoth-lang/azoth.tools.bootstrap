using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
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

    public static bool GetBoolConfig(IEnumerable<string> lines, string config, bool defaultValue)
    {
        var value = GetConfig(lines, config);
        if (value is null) return defaultValue;
        return bool.Parse(value);
    }

    public static string GetRequiredConfig(IEnumerable<string> lines, string config)
        => GetConfig(lines, config) ?? throw new FormatException($"Missing required config: '{config}'");

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
    public static SymbolSyntax? ParseSymbol(string? symbol)
    {
        if (symbol is null) return null;
        if (symbol == "<default>")
            return new("");
        if (symbol.StartsWith('`') && symbol.EndsWith('`'))
            return new(symbol[1..^1], true);
        return new(symbol);
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

    public static IEnumerable<string> SplitCommaSeparated(string supertypes)
        => supertypes.Split(',').Select(p => p.Trim());

    public static (string, string?) OptionalSplitTwo(string value, string separator, string errorMessage)
    {
        var index = value.IndexOf(separator, StringComparison.InvariantCulture);
        if (index == -1) return (value, null);
        return SplitTwo(value, separator, index, errorMessage);
    }

    public static (string, string) SplitTwo(string value, string separator, string errorMessage)
    {
        var index = value.IndexOf(separator, StringComparison.InvariantCulture);
        if (index == -1)
            throw new FormatException(string.Format(errorMessage, value));
        return SplitTwo(value, separator, index, errorMessage);
    }

    private static (string, string) SplitTwo(string value, string separator, int atIndex, string errorMessage)
    {
        var secondIndex = value.IndexOf(separator, atIndex + separator.Length, StringComparison.InvariantCulture);
        if (secondIndex != -1)
            throw new FormatException(string.Format(errorMessage, value));
        return (value[..atIndex].Trim(), value[(atIndex + separator.Length)..].Trim());
    }

    public static (string, string?) OptionalSplitAtFirst(string value, string separator)
    {
        var index = value.IndexOf(separator, StringComparison.InvariantCulture);
        if (index == -1) return (value, null);
        return (value[..index].Trim(), value[(index + separator.Length)..].Trim());
    }

    public static (string, string) SplitAtFirst(string value, string separator, string errorMessage)
    {
        var index = value.IndexOf(separator, StringComparison.InvariantCulture);
        if (index == -1) throw new FormatException(string.Format(errorMessage, value));
        return (value[..index].Trim(), value[(index + separator.Length)..].Trim());
    }

    [return: NotNullIfNotNull(nameof(type))]
    public static TypeSyntax? ParseType(string? type)
    {
        if (type is null) return null;
        bool isOptional = ParseOffEnd(ref type, "?");
        var collectionKind = ParseCollectionKind(ref type);
        return new(ParseSymbol(type), collectionKind, isOptional);
    }

    private static CollectionKind ParseCollectionKind(ref string type)
    {
        var isList = ParseOffEnd(ref type, "*");

        var isSet = type.StartsWith('{') && type.EndsWith('}');
        type = isSet ? type[1..^1] : type;

        if (isList && isSet) throw new FormatException("Property cannot be both a list and a set");
        if (isList) return CollectionKind.List;
        if (isSet) return CollectionKind.Set;
        return CollectionKind.None;
    }

    public static bool ParseOffEnd(ref string value, string suffix)
    {
        var endsWith = value.EndsWith(suffix);
        if (endsWith) value = value[..^suffix.Length].Trim();
        return endsWith;
    }

    public static bool ParseOffStart(ref string value, char suffix)
    {
        var endsWith = value.StartsWith(suffix);
        if (endsWith) value = value[1..].Trim();
        return endsWith;
    }
}
