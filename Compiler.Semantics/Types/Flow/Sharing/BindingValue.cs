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
    private static readonly ConcurrentDictionary<ValueId, BindingValue> TopLevelCache = new();

    private static BindingValue TopLevelFactory(ValueId number) => new(number, CapabilityIndex.TopLevel);
    #endregion

    public static BindingValue TopLevel(IBindingNode node)
        => TopLevelCache.GetOrAdd(node.ValueId, TopLevelFactory);

    public static List<KeyValuePair<BindingValue, FlowCapability>> ForType(ValueId id, Pseudotype type)
    {
        var index = new Stack<int>();
        var bindingValues = new List<KeyValuePair<BindingValue, FlowCapability>>();
        ForType(id, type.ToUpperBound(), index, true, bindingValues);
        return bindingValues;
    }

    private static void ForType(
        ValueId id,
        DataType type,
        Stack<int> index,
        bool capture,
        List<KeyValuePair<BindingValue, FlowCapability>> bindingValues)
    {
        if (capture && type is CapabilityType t)
            bindingValues.Add(new(new BindingValue(id, new(index)), t.Capability));

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

    public ValueId Id { get; }
    public CapabilityIndex Index { get; }

    private BindingValue(ValueId id, CapabilityIndex index)
    {
        Id = id;
        Index = index;
    }

    #region Equality
    public bool Equals(IValue? other)
        => ReferenceEquals(this, other)
           || other is BindingValue bindingValue
                && Id.Equals(bindingValue.Id)
                && Index.Equals(bindingValue.Index);

    public override bool Equals(object? obj)
        => ReferenceEquals(this, obj) || obj is BindingValue other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Id, Index);
    #endregion

    public override string ToString() => $"{Id}.{Index}";
}
