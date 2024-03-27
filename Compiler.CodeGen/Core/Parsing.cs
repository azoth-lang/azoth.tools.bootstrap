using System;
using System.Collections.Generic;
using System.Linq;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Core;

internal static class Parsing
{
    public const string DirectiveMarker = "◊";

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
        line = line?.TrimSemicolon();
        line = line?.Trim();
        return line;
    }

    private static string ConfigStart(string config) => DirectiveMarker + config;

    public static IEnumerable<string> GetUsingNamespaces(IEnumerable<string> lines)
    {
        const string start = DirectiveMarker + "using";
        lines = lines.Where(l => l.StartsWith(start, StringComparison.InvariantCulture));
        // TODO error if no semicolon
        return lines.Select(l => l[start.Length..].TrimSemicolon().Trim());
    }

    public static string TrimSemicolon(this string line)
    {
        if (!line.EndsWith(';'))
            throw new FormatException($"Line does not end with a semicolon: '{line}'");
        return line.TrimEnd(';');
    }

    public static string TrimComment(this string line)
    {
        var commentIndex = line.IndexOf("//", StringComparison.InvariantCulture);
        // Must trim end because we often assume no trailing whitespace
        return commentIndex == -1 ? line : line[..commentIndex].TrimEnd();
    }
}
