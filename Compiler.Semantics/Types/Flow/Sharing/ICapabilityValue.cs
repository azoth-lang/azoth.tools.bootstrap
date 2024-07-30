using System;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow.Sharing;

/// <summary>
/// A <see cref="IValue"/> that has its capability affected by flow typing and sharing.
/// </summary>
public interface ICapabilityValue : IValue
{
    ulong Value { get; }
    CapabilityIndex Index { get; }

    protected static List<(T Value, FlowCapability FlowCapability)> ForType<T>(ValueId id, Pseudotype type, Func<ValueId, CapabilityIndex, T> create)
        where T : ICapabilityValue
    {
        var index = new Stack<int>();
        var values = new List<(T Value, FlowCapability FlowCapability)>();
        ForType(id, type.ToUpperBound(), index, true, values, create);
        return values;
    }

    private static void ForType<T>(
        ValueId id,
        DataType type,
        Stack<int> index,
        bool capture,
        List<(T Value, FlowCapability FlowCapability)> values,
        Func<ValueId, CapabilityIndex, T> create)
        where T : ICapabilityValue
    {
        if (capture && type is CapabilityType t)
            values.Add((create(id, new(index)), t.Capability));

        if (type is OptionalType optionalType)
        {
            // Traverse into optional types
            ForType(id, optionalType.Referent, index, capture, values, create);
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
            ForType(id, arg.Argument, index, arg.ParameterHasIndependence, values, create);
            index.Pop();
        }
    }
}
