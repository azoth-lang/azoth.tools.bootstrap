using System.Collections.Generic;
using System.IO;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Core;

internal static class StringExtensions
{
    public static IEnumerable<string> ToLines(this string value)
    {
        using var reader = new StringReader(value);
        while (reader.ReadLine() is string line)
            yield return line;
    }

    public static string ToCamelCase(this string value)
    {
        if (value.Length == 0)
            return value;
        return char.ToLowerInvariant(value[0]) + value[1..];
    }
}
