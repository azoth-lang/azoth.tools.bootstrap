using System.Diagnostics;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;

[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
public sealed record SymbolNode(string Text, bool IsQuoted = false)
{
    public static SymbolNode Void { get; } = new("void", true);

    public override string ToString() => IsQuoted ? $"`{Text}`" : Text;
}
