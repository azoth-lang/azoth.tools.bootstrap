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
}
