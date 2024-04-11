using System.Collections.Generic;
using System.IO;
using System.Linq;
using Azoth.Tools.Bootstrap.Framework;

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
        value = char.ToLowerInvariant(value[0]) + value[1..];
        if (Keywords.Contains(value))
            return "@" + value;
        return value;
    }

    private static readonly IFixedSet<string> Keywords = new[]
    {
        "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked",
        "class", "const", "continue", "decimal", "default", "delegate", "do", "double", "else",
        "enum", "event", "explicit", "extern", "false", "finally", "fixed", "float", "for",
        "foreach", "goto", "if", "implicit", "in", "int", "interface", "internal", "is", "lock",
        "long", "namespace", "new", "null", "object", "operator", "out", "override", "params",
        "private", "protected", "public", "readonly", "ref", "return", "sbyte", "sealed",
        "short", "sizeof", "stackalloc", "static", "string", "struct", "switch", "this", "throw",
        "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort", "using",
        "virtual", "void", "volatile", "while"
    }.ToFixedSet();
}
