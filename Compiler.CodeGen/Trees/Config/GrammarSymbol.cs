using System.Diagnostics;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Trees.Config;

[DebuggerDisplay("{" + nameof(ToString) + ",nq}")]
public sealed record GrammarSymbol(string Text, bool IsQuoted = false)
{
    public override string ToString() => IsQuoted ? $"'{Text}'" : Text;
}
