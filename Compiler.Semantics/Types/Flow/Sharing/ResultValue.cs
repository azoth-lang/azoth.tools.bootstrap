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

    private static ResultValue Factory(ulong value) => new(value);
    #endregion

    public ulong Value { get; }
    CapabilityIndex ICapabilityValue.Index => CapabilityIndex.TopLevel;

    private ResultValue(ulong value)
    {
        Value = value;
    }

    #region Equality
    public bool Equals(IValue? other)
        => other is ResultValue value && Value == value.Value;

    public override bool Equals(object? obj) => obj is ResultValue other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Value);
    #endregion

    public override string ToString() => $"⧼result{Value}⧽";
}
