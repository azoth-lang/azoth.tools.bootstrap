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
    /// <summary>
    /// Whether this is a declared variable or parameter (as opposed to a temp reference etc.)
    /// </summary>
    bool IsVariableOrParameter { get; }
    ulong Value { get; }
    CapabilityIndex Index { get; }

    protected static IReadOnlyDictionary<T, FlowCapability> ForType<T>(ValueId id, Pseudotype type, Func<ValueId, CapabilityIndex, T> create)
        where T : ICapabilityValue
    {
        var index = new Stack<int>();
        var values = new Dictionary<T, FlowCapability>();
        ForType(id, type.ToUpperBound(), index, true, values, create);
        return values.AsReadOnly();
    }

    private static void ForType<T>(
        ValueId id,
        DataType type,
        Stack<int> index,
        bool capture,
        Dictionary<T, FlowCapability> values,
        Func<ValueId, CapabilityIndex, T> create)
        where T : ICapabilityValue
    {
        switch (type)
        {
            case CapabilityType t when capture:
                values.Add(create(id, new(index)), t.Capability);
                break;
            case OptionalType optionalType:
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
