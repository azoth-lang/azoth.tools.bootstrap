using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;

[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
[StructLayout(LayoutKind.Auto)]
public readonly record struct ValueId : IPreviousValueId
{
    public ValueIdScope Scope { get; }
    public ulong Value { get; }

    private ValueId(ValueIdScope Scope, ulong Value)
    {
        this.Scope = Scope;
        this.Value = Value;
    }

    public static ValueId CreateFirst(ValueIdScope scope) => new(scope, 0);

    public ValueId CreateNext()
        => new(Scope, Value + 1);

    public override string ToString() => $"⧼value{Value}⧽";
}
