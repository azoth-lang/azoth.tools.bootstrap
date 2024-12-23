using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Flow;

[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
[StructLayout(LayoutKind.Auto)]
public readonly record struct ValueId
{
    public ValueIdScope Scope { get; }
    public ulong Value { get; }

    internal ValueId(ValueIdScope Scope, ulong Value)
    {
        this.Scope = Scope;
        this.Value = Value;
    }

    public override string ToString() => $"⧼value{Value}⧽";
}
