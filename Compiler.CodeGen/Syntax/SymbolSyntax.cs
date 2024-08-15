using System.Diagnostics;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;

[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
public sealed record SymbolSyntax(string Text, bool IsQuoted = false)
{
    public static SymbolSyntax Void { get; } = new("void", true);

    public override string ToString() => IsQuoted ? $"`{Text}`" : Text;
}
