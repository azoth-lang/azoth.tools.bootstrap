using System;
using System.Diagnostics;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;

[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
public record struct ValueId(ValueIdScope Scope, ulong Value) : IPreviousValueId
{
    public readonly ValueId CreateNext()
    {
        var next = Scope.CreateValueId();
        if (next.Value != Value + 1)
            throw new InvalidOperationException("Value Ids must be created in order.");
        return next;
    }

    public readonly override string ToString() => $"⧼value{Value}⧽";
}
