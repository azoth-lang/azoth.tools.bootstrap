using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow.Sharing;

internal sealed class CapabilityValue : ICapabilityValue
{
    #region Cache
    private static readonly ConcurrentDictionary<ulong, CapabilityValue> TopLevelCache = new();

    private static CapabilityValue TopLevelFactory(ulong number) => new(number, CapabilityIndex.TopLevel);
    #endregion

    public static CapabilityValue Create(ValueId id, CapabilityIndex index)
        => index.IsTopLevel ? TopLevelCache.GetOrAdd(id.Value, TopLevelFactory) : new(id.Value, index);

    public static CapabilityValue CreateTopLevel(ValueId id)
        => TopLevelCache.GetOrAdd(id.Value, TopLevelFactory);

    public static IReadOnlyDictionary<CapabilityValue, FlowCapability> ForType(ValueId id, Pseudotype type)
        => ICapabilityValue.ForType(id, type, Create);

    public ulong Value { get; }
    public CapabilityIndex Index { get; }

    private CapabilityValue(ulong value, CapabilityIndex index)
    {
        Value = value;
        Index = index;
    }

    #region Equality
    public bool Equals(IValue? other)
        => ReferenceEquals(this, other)
           || other is CapabilityValue value
                && Value.Equals(value.Value)
                && Index.Equals(value.Index);

    public override bool Equals(object? obj)
        => ReferenceEquals(this, obj) || obj is CapabilityValue other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Value, Index);
    #endregion

    public override string ToString() => $"⧼value{Value}⧽.{Index}";
}
