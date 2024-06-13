using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;

[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
[StructLayout(LayoutKind.Auto)]
public readonly record struct ValueId(ValueIdScope Scope, ulong Value) : IPreviousValueId
{
    public ValueId CreateNext()
    {
        var next = Scope.CreateValueId();
        if (next.Value != Value + 1)
            throw new InvalidOperationException("Value Ids must be created in order.");
        return next;
    }

    public override string ToString() => $"⧼value{Value}⧽";
}
