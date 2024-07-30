using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow.Sharing;

/// <summary>
/// A value for a binding (i.e. a variable, parameter, or field).
/// </summary>
[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
internal sealed class BindingValue : ICapabilityValue
{
    #region Cache
    private static readonly ConcurrentDictionary<ulong, BindingValue> TopLevelCache = new();

    private static BindingValue TopLevelFactory(ulong value) => new(value, CapabilityIndex.TopLevel);
    #endregion

    public static BindingValue TopLevel(IBindingNode node)
        => TopLevelCache.GetOrAdd(node.BindingValueId.Value, TopLevelFactory);

    public static List<(BindingValue Value, FlowCapability FlowCapability)> ForType(ValueId id, Pseudotype type)
    {
        var index = new Stack<int>();
        var bindingValues = new List<(BindingValue Value, FlowCapability FlowCapability)>();
        ForType(id, type.ToUpperBound(), index, true, bindingValues);
        return bindingValues;
    }

    private static void ForType(
        ValueId id,
        DataType type,
        Stack<int> index,
        bool capture,
        List<(BindingValue Value, FlowCapability FlowCapability)> bindingValues)
    {
        if (capture && type is CapabilityType t)
            bindingValues.Add((new(id.Value, new(index)), t.Capability));

        if (type is OptionalType optionalType)
        {
            index.Push(0);
            ForType(id, optionalType.Referent, index, true, bindingValues);
            index.Pop();
            return;
        }

        if (!type.HasIndependentTypeArguments)
            return;

        var capabilityType = (CapabilityType)type;
        foreach (var (arg, i) in capabilityType.BareType.GenericParameterArguments.Enumerate())
        {
            if (arg is { ParameterHasIndependence: false, Argument.HasIndependentTypeArguments: false })
                continue;

            index.Push(i);
            ForType(id, arg.Argument, index, arg.ParameterHasIndependence, bindingValues);
            index.Pop();
        }
    }

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
