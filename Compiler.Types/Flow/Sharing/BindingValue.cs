using System.Collections.Concurrent;
using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Flow.Sharing;

/// <summary>
/// A value for a binding (i.e. a variable, parameter, or field).
/// </summary>
[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
public sealed class BindingValue : ICapabilityValue
{
    #region Cache
    private static readonly ConcurrentDictionary<ulong, BindingValue> TopLevelCache = new();

    private static BindingValue TopLevelFactory(ulong value) => new(value, CapabilityIndex.TopLevel);
    #endregion

    public static BindingValue Create(ValueId id, CapabilityIndex index)
        => index.IsTopLevel ? TopLevelCache.GetOrAdd(id.Value, TopLevelFactory) : new(id.Value, index);

    public static BindingValue CreateTopLevel(ValueId id)
        => TopLevelCache.GetOrAdd(id.Value, TopLevelFactory);

    public static IReadOnlyDictionary<BindingValue, FlowCapability> ForType(ValueId id, IMaybeType type)
        => ICapabilityValue.ForType(id, type, Create);

    public bool IsVariableOrParameter => true;
    public ulong Value { get; }
    public CapabilityIndex Index { get; }

    private BindingValue(ulong value, CapabilityIndex index)
    {
        Value = value;
        Index = index;
    }

    #region Equality
    public bool Equals(IValue? other)
        => ReferenceEquals(this, other)
           || other is BindingValue value
                && Value.Equals(value.Value)
                && Index.Equals(value.Index);

    public override bool Equals(object? obj)
        => ReferenceEquals(this, obj) || obj is BindingValue other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Value, Index);
    #endregion

    public override string ToString() => $"⧼value{Value}⧽.{Index}";
}
