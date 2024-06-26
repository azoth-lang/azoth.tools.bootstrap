using System;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow.Sharing;

[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
internal sealed class ResultValue : ICapabilityValue
{
    #region Cache
    private static readonly ConcurrentDictionary<ulong, ResultValue> Cache = new();

    public static ResultValue Create(ValueId id) => Cache.GetOrAdd(id.Value, Factory);

    private static ResultValue Factory(ulong number) => new(number);
    #endregion

    private readonly ulong number;

    private ResultValue(ulong number)
    {
        this.number = number;
    }

    #region Equality
    public bool Equals(IValue? other)
        => other is ResultValue value && number == value.number;

    public override bool Equals(object? obj) => obj is ResultValue other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(number);
    #endregion

    public override string ToString() => $"⧼result{number}⧽";
}
