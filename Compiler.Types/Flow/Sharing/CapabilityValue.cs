using System.Collections.Concurrent;
using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Flow.Sharing;

/// <summary>
/// A value that could be relevant for flow type tracking. Values represent things like the
/// temporary value that results from an expression. Values are identified by the
/// <see cref="ValueId"/> of the expression etc. that created them and the
/// <see cref="CapabilityIndex"/> of the tracked value within that result.
/// </summary>
[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
public sealed class CapabilityValue : Comparable<ICapabilityValue>, ICapabilityValue
{
    #region Cache
    private static readonly ConcurrentDictionary<ulong, CapabilityValue> TopLevelCache = new();

    private static CapabilityValue TopLevelFactory(ulong number) => new(number, CapabilityIndex.TopLevel);
    #endregion

    public static CapabilityValue Create(ValueId id, CapabilityIndex index)
        => index.IsTopLevel ? CreateTopLevel(id) : new(id.Value, index);

    public static CapabilityValue CreateTopLevel(ValueId id)
        => TopLevelCache.GetOrAdd(id.Value, TopLevelFactory);

    /// <summary>
    /// Get all the <see cref="CapabilityValue"/>s for a given <see cref="ValueId"/> with a
    /// particular type. For each, it also provides the <see cref="Capability"/> as determined by
    /// the type.
    /// </summary>
    /// <remarks>Typically there is one <see cref="CapabilityValue"/> at the root. However, if there
    /// are independent parameters than each of those is also a <see cref="CapabilityValue"/>.</remarks>
    public static IReadOnlyDictionary<CapabilityValue, Capability> ForType(ValueId id, IMaybeType type)
        => ICapabilityValue.ForType(id, type, Create);

    public bool IsVariableOrParameter => false;
    public ulong Value { get; }
    public CapabilityIndex Index { get; }

    private CapabilityValue(ulong value, CapabilityIndex index)
    {
        Value = value;
        Index = index;
    }

    #region Equality and Comparison
    public bool Equals(IValue? other)
        => ReferenceEquals(this, other)
           || other is CapabilityValue value
                && Value.Equals(value.Value)
                && Index.Equals(value.Index);

    public override int CompareTo(ICapabilityValue? other)
    {
        if (other == null) return 1;
        if (ReferenceEquals(this, other)) return 0;
        return other switch
        {
            BindingValue _ => 1,
            CapabilityValue v => Value.CompareTo(v.Value).ThenCompareBy(Index.CompareTo(v.Index)),
            _ => throw ExhaustiveMatch.Failed(other)
        };
    }

    public override int GetHashCode() => HashCode.Combine(Value, Index);
    #endregion

    public override string ToString() => $"⧼value{Value}⧽.{Index}";
}
