using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Azoth.Tools.Bootstrap.Framework;

public static partial class StringExtensions
{
    [GeneratedRegex(@"\r\n|\n\r|\n|\r", RegexOptions.ExplicitCapture)]
    private static partial Regex LineEndings();

    public static string Repeat(this string input, int count)
    {
        if (string.IsNullOrEmpty(input) || count == 0)
            return string.Empty;

        return new StringBuilder(input.Length * count)
               .Insert(0, input, count)
               .ToString();
    }

    public static string NormalizeLineEndings(this string input, string lineEnding)
        => LineEndings().Replace(input, lineEnding);

    public static string Escape(this string input)
    {
        var escaped = new StringBuilder(input.Length + 2);
        foreach (var c in input)
        {
            switch (c)
            {
                case '\'':
                    escaped.Append(@"\'");
                    break;
                case '\"':
                    escaped.Append("\\\"");
                    break;
                case '\\':
                    escaped.Append(@"\\");
                    break;
                case '\0':
                    escaped.Append(@"\0");
                    break;
                case '\a':
                    escaped.Append(@"\a");
                    break;
                case '\b':
                    escaped.Append(@"\b");
                    break;
                case '\f':
                    escaped.Append(@"\f");
                    break;
                case '\n':
                    escaped.Append(@"\n");
                    break;
                case '\r':
                    escaped.Append(@"\r");
                    break;
                case '\t':
                    escaped.Append(@"\t");
                    break;
                case '\v':
                    escaped.Append(@"\v");
                    break;
                default:
                    if (char.GetUnicodeCategory(c) != UnicodeCategory.Control)
                        escaped.Append(c);
                    else
                    {
                        escaped.Append(@"\u(");
                        escaped.Append(((ushort)c).ToString("x", CultureInfo.InvariantCulture));
                        escaped.Append(')');
                    }

                    break;
            }
        }
        return escaped.ToString();
    }

    public static string Unescape(this string input)
    {
        var unescaped = new StringBuilder(input);
        unescaped.Replace(@"\\", @"\b") // Swap out backslash escape to not mess up others
                 .Replace(@"\r", "\r")
                 .Replace(@"\n", "\n")
                 .Replace(@"\0", "\0")
                 .Replace(@"\t", "\t")
                 .Replace(@"\'", "\'")
                 .Replace(@"\""", "\"")
                 .Replace(@"\b", "\\"); // Put in actual backslashes
        return unescaped.ToString();
    }

    /// <summary>
    /// If the string is empty, returns an empty list. Otherwise, splits the string by the
    /// given separators.
    /// </summary>
    public static IFixedList<string> SplitOrEmpty(this string value, params char[] separators)
        => value.Split(separators, StringSplitOptions.RemoveEmptyEntries).ToFixedList();

    /// <summary>
    /// If the string is empty, returns an empty list. Otherwise, splits the string at whitespace.
    /// Empty entries are removed and whitespace is trimmed.
    /// </summary>
    public static IFixedList<string> SplitWhitespaceOrEmpty(this string value)
        => value.Split(Array.Empty<char>(), StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToFixedList();

    public static int? IndexOfWhitespace(this string value)
    {
        for (var i = 0; i < value.Length; i++)
            if (char.IsWhiteSpace(value[i]))
                return i;
        return null;
    }

    public static int? IndexOfWhitespace(this string value, int startAtIndex)
    {
        for (var i = startAtIndex; i < value.Length; i++)
            if (char.IsWhiteSpace(value[i]))
                return i;
        return null;
    }

    public static int? LastIndexOfWhitespace(this string value)
    {
        for (var i = value.Length - 1; i >= 0; i--)
            if (char.IsWhiteSpace(value[i]))
                return i;
        return null;
    }
}
