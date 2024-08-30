using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Types;
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
        => supertypes.Split(',', StringSplitOptions.TrimEntries);

    public static (string, string?) OptionalBisect(string value, string separator, string errorMessage)
    {
        var index = value.IndexOf(separator, StringComparison.InvariantCulture);
        if (index == -1) return (value, null);
        return BisectImplementation(value, separator, index, errorMessage);
    }

    public static (string, string) Bisect(string value, string separator, string errorMessage)
    {
        var index = value.IndexOf(separator, StringComparison.InvariantCulture);
        if (index == -1)
            throw new FormatException(string.Format(errorMessage, value));
        return BisectImplementation(value, separator, index, errorMessage);
    }

    private static (string, string) BisectImplementation(string value, string separator, int atIndex, string errorMessage)
    {
        var secondIndex = value.IndexOf(separator, atIndex + separator.Length, StringComparison.InvariantCulture);
        if (secondIndex != -1)
            throw new FormatException(string.Format(errorMessage, value));
        return (value[..atIndex].Trim(), value[(atIndex + separator.Length)..].Trim());
    }

    public static (string, string) Bisect(string value, string errorMessage)
    {
        var maybeIndex = value.IndexOfWhitespace();
        if (maybeIndex is not { } index)
            throw new FormatException(string.Format(errorMessage, value));
        var secondIndex = value.IndexOfWhitespace(index + 1);
        if (secondIndex is not null)
            throw new FormatException(string.Format(errorMessage, value));
        return (value[..index].Trim(), value[(index + 1)..].Trim());
    }

    public static (string?, string) OptionalSplitOffStart(string value)
    {
        var maybeIndex = value.IndexOfWhitespace();
        if (maybeIndex is not { } index) return (null, value);
        return (value[..index].Trim(), value[(index + 1)..].Trim());
    }

    public static (string, string?) OptionalSplitOffEnd(string value, string separator)
    {
        var index = value.LastIndexOf(separator, StringComparison.InvariantCulture);
        if (index == -1) return (value, null);
        return (value[..index].Trim(), value[(index + separator.Length)..].Trim());
    }

    public static (string, string?) OptionalSplitOffEnd(string value)
    {
        var maybeIndex = value.LastIndexOfWhitespace();
        if (maybeIndex is not { } index) return (value, null);
        return (value[..index].Trim(), value[(index + 1)..].Trim());
    }

    public static (string, string) SplitOffStart(string value, string separator, string errorMessage)
    {
        var index = value.IndexOf(separator, StringComparison.InvariantCulture);
        if (index == -1) throw new FormatException(string.Format(errorMessage, value));
        return (value[..index].Trim(), value[(index + separator.Length)..].Trim());
    }

    public static (string, string) SplitOffStart(string value, string errorMessage)
    {
        var maybeIndex = value.IndexOfWhitespace();
        if (maybeIndex is not { } index) throw new FormatException(string.Format(errorMessage, value));
        return (value[..index].Trim(), value[(index + 1)..].Trim());
    }

    public static (string, string) SplitOffEnd(string value, string separator, string errorMessage)
    {
        var index = value.LastIndexOf(separator, StringComparison.InvariantCulture);
        if (index == -1) throw new FormatException(string.Format(errorMessage, value));
        return (value[..index].Trim(), value[(index + separator.Length)..].Trim());
    }

    public static (string, string) SplitOffEnd(string value, string errorMessage)
    {
        var maybeIndex = value.LastIndexOfWhitespace();
        if (maybeIndex is not { } index) throw new FormatException(string.Format(errorMessage, value));
        return (value[..index].Trim(), value[(index + 1)..].Trim());
    }

    [return: NotNullIfNotNull(nameof(type))]
    public static TypeSyntax? ParseType(string? type)
    {
        if (type is null) return null;
        bool isOptional = ParseOffEnd(ref type, "?");
        if (isOptional)
            return new OptionalTypeSyntax(ParseType(type));

        var isList = ParseOffEnd(ref type, "*");
        if (isList)
            return new CollectionTypeSyntax(CollectionKind.List, ParseType(type));

        var isSet = type.StartsWith('{') && type.EndsWith('}');
        type = isSet ? type[1..^1] : type;
        if (isSet)
            return new CollectionTypeSyntax(CollectionKind.Set, ParseType(type));

        var startsEnumerable = ParseOffStart(ref type, "IEnumerable<");
        if (startsEnumerable)
        {
            if (!ParseOffEnd(ref type, ">"))
                throw new FormatException("Expected closing '>' for IEnumerable");
            return new CollectionTypeSyntax(CollectionKind.Enumerable, ParseType(type));
        }

        return new SymbolTypeSyntax(ParseSymbol(type));
    }

    public static bool ParseOffEnd(ref string value, string suffix)
    {
        var endsWith = value.EndsWith(suffix);
        if (endsWith) value = value[..^suffix.Length].Trim();
        return endsWith;
    }

    public static bool ParseOffStart(ref string value, string prefix)
    {
        var endsWith = value.StartsWith(prefix);
        if (endsWith) value = value[prefix.Length..].Trim();
        return endsWith;
    }

    public static IEnumerable<SymbolSyntax> ParseSupertypes(string? supertypes)
    {
        if (supertypes is null) return [];
        return SplitCommaSeparated(supertypes).Select(s => ParseSymbol(s));
    }
}
