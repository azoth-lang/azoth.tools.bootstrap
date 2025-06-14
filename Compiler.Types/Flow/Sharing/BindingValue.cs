using System.Collections.Concurrent;
using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Flow.Sharing;

/// <summary>
/// A value for a binding (i.e. a variable, parameter, or field).
/// </summary>
[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
public sealed class BindingValue : Comparable<ICapabilityValue>, ICapabilityValue
{
    #region Cache
    private static readonly ConcurrentDictionary<ulong, BindingValue> TopLevelCache = new();

    private static BindingValue TopLevelFactory(ulong value) => new(value, CapabilityIndex.TopLevel);
    #endregion

    public static BindingValue Create(ValueId id, CapabilityIndex index)
        => index.IsTopLevel ? TopLevelCache.GetOrAdd(id.Value, TopLevelFactory) : new(id.Value, index);

    public static BindingValue CreateTopLevel(ValueId id)
        => TopLevelCache.GetOrAdd(id.Value, TopLevelFactory);

    public static IReadOnlyDictionary<BindingValue, Capability> ForType(ValueId id, IMaybeType type)
        => ICapabilityValue.ForType(id, type, Create);

    public bool IsVariableOrParameter => true;
    public ulong Value { get; }
    public CapabilityIndex Index { get; }

    private BindingValue(ulong value, CapabilityIndex index)
    {
        Value = value;
        Index = index;
    }

    #region Equality and Comparison
    public bool Equals(IValue? other)
        => ReferenceEquals(this, other)
           || other is BindingValue value
               && Value.Equals(value.Value)
               && Index.Equals(value.Index);

    public override int CompareTo(ICapabilityValue? other)
    {
        if (other == null) return 1;
        if (ReferenceEquals(this, other)) return 0;
        return other switch
        {
            BindingValue v => Value.CompareTo(v.Value).ThenCompareBy(Index.CompareTo(v.Index)),
            CapabilityValue _ => -1,
            _ => throw ExhaustiveMatch.Failed(other)
        };
    }

    public override int GetHashCode() => HashCode.Combine(Value, Index);
    #endregion

    public override string ToString() => $"⧼value{Value}⧽.{Index}";
}
