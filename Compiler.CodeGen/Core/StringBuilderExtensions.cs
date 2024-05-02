using System.Text;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Core;

internal static class StringBuilderExtensions
{
    public static StringBuilder AppendSeparator(this StringBuilder sb, char value)
    {
        if (sb.Length > 0)
            sb.Append(value);
        return sb;
    }
}
