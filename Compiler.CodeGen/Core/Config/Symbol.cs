using System.Diagnostics;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Core.Config;

[DebuggerDisplay("{" + nameof(ToString) + ",nq}")]
public sealed record Symbol(string Text, bool IsQuoted = false)
{
    public override string ToString() => IsQuoted ? $"`{Text}`" : Text;
}
