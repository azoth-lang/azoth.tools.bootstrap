using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Flow;

[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
[StructLayout(LayoutKind.Auto)]
public readonly record struct ArgumentValueId(bool IsLent, ValueId ValueId)
{
    public override string ToString() => IsLent ? $"lent {ValueId}" : ValueId.ToString();
}
