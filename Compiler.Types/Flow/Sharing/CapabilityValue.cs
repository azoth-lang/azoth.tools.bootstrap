using System.Collections.Concurrent;
using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Flow.Sharing;

/// <summary>
/// A value that could be relevant for flow type tracking. Values represent things like the
/// temporary value that results from an expression. Values are identified by the
/// <see cref="ValueId"/> of the expression etc. that created them and the
/// <see cref="CapabilityIndex"/> of the tracked value within that result.
/// </summary>
[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
public sealed class CapabilityValue : ICapabilityValue
{
    #region Cache
    private static readonly ConcurrentDictionary<ulong, CapabilityValue> TopLevelCache = new();

    private static CapabilityValue TopLevelFactory(ulong number) => new(number, CapabilityIndex.TopLevel);
    #endregion

    public static CapabilityValue Create(ValueId id, CapabilityIndex index)
        => index.IsTopLevel ? CreateTopLevel(id) : new(id.Value, index);

    public static CapabilityValue CreateTopLevel(ValueId id)
        => TopLevelCache.GetOrAdd(id.Value, TopLevelFactory);

    public static IReadOnlyDictionary<CapabilityValue, FlowCapability> ForType(ValueId id, IMaybeType type)
        => ICapabilityValue.ForType(id, type, Create);

    public bool IsVariableOrParameter => false;
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
