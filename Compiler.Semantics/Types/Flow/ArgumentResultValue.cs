using System.Diagnostics;
using System.Runtime.InteropServices;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow.Sharing;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;

[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
[StructLayout(LayoutKind.Auto)]
internal readonly record struct ArgumentResultValue(bool IsLent, ResultValue Value)
{
    public ArgumentResultValue(bool isLent, ValueId valueId)
        : this(isLent, ResultValue.Create(valueId))
    {
    }

    public override string ToString() => IsLent ? $"lent {Value}" : Value.ToString();
}
