using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;
using Azoth.Tools.Bootstrap.Framework;

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

    public static List<(CapabilityValue Value, FlowCapability FlowCapability)> ForType(ValueId id, Pseudotype type)
    {
        var index = new Stack<int>();
        var capabilityValues = new List<(CapabilityValue Value, FlowCapability FlowCapability)>();
        ForType(id, type.ToUpperBound(), index, true, capabilityValues);
        return capabilityValues;
    }

    private static void ForType(
        ValueId id,
        DataType type,
        Stack<int> index,
        bool capture,
        List<(CapabilityValue Value, FlowCapability FlowCapability)> bindingValues)
    {
        if (capture && type is CapabilityType t)
            bindingValues.Add((Create(id, new(index)), t.Capability));

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

    private CapabilityValue(ulong value, CapabilityIndex index)
    {
        Value = value;
        Index = index;
    }

    #region Equality
    public bool Equals(IValue? other)
        => ReferenceEquals(this, other)
           || other is BindingValue bindingValue
                && Value.Equals(bindingValue.Value)
                && Index.Equals(bindingValue.Index);

    public override bool Equals(object? obj)
        => ReferenceEquals(this, obj) || obj is BindingValue other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Value, Index);
    #endregion

    public override string ToString() => $"⧼value{Value}⧽.{Index}";
}
