using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Flow.Sharing;

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

    protected static IReadOnlyDictionary<T, FlowCapability> ForType<T>(ValueId id, IMaybeType type, Func<ValueId, CapabilityIndex, T> create)
        where T : ICapabilityValue
    {
        var index = new Stack<int>();
        var values = new Dictionary<T, FlowCapability>();
        // TODO why was it `type.ToUpperBound()` before?
        ForType(id, type, index, capture: true, values, create);
        return values.AsReadOnly();
    }

    private static void ForType<T>(
        ValueId id,
        IMaybeType type,
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
            case OptionalType t:
                // Traverse into optional types
                ForType(id, t.Referent, index, capture, values, create);
                return;
            case SelfViewpointType t:
                // TODO if capture is true, shouldn't the viewpoint somehow affect the flow capability?
                ForType(id, t.Referent, index, capture, values, create);
                return;
            case CapabilitySetSelfType t:
                // TODO I really don't know that this is right
                var newContextType = t.BareType.ContainingType!.With(t.CapabilitySet.UpperBound);
                ForType(id, newContextType, index, capture, values, create);
                return;
        }

        if (!type.HasIndependentTypeArguments)
            return;

        // TODO handle other types? It would have to be one with independent type arguments
        var capabilityType = (CapabilityType)type;
        foreach (var (arg, i) in capabilityType.TypeParameterArguments.Enumerate())
        {
            if (arg is { ParameterHasIndependence: false, Argument.HasIndependentTypeArguments: false })
                continue;

            index.Push(i);
            ForType(id, arg.Argument, index, arg.ParameterHasIndependence, values, create);
            index.Pop();
        }
    }
}
