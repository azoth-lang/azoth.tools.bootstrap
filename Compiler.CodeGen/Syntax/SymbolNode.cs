using System.Diagnostics;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;

[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
public sealed record SymbolNode(string Text, bool IsQuoted = false)
{
    public override string ToString() => IsQuoted ? $"`{Text}`" : Text;
}
