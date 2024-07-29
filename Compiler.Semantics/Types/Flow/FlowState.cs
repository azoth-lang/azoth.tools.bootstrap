using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow.Sharing;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Framework;
using Azoth.Tools.Bootstrap.Framework.Collections;
using InlineMethod;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;

internal sealed class FlowState : IFlowState
{
    public static FlowState Empty { get; } = new FlowState();

    /// <summary>
    /// All the values that are tracked in scope in disjoint sharing sets with associated flow
    /// capabilities.
    /// </summary>
    private readonly IImmutableDisjointSets<IValue, FlowCapability, SharingSetState> values;

    /// <summary>
    /// Values that are not tracked by the flow state.
    /// </summary>
    /// <remarks>These values have capabilities of `const` or `id`.</remarks>
    private readonly ImmutableHashSet<IValue> untrackedValues;

    private int hashCode;

    public bool IsEmpty => values.Count == 0 && untrackedValues.Count == 0;

    private FlowState()
    {
        values = ImmutableDisjointHashSets<IValue, FlowCapability, SharingSetState>.Empty;
        untrackedValues = ImmutableHashSet<IValue>.Empty;
    }

    private FlowState(IImmutableDisjointSets<IValue, FlowCapability, SharingSetState> values,
        ImmutableHashSet<IValue> untrackedValues)
    {
        this.values = values;
        this.untrackedValues = untrackedValues;
    }

    public IFlowState Declare(INamedParameterNode parameter)
        => Declare(parameter, parameter.IsLentBinding);

    public IFlowState Declare(ISelfParameterNode parameter)
        => Declare(parameter, parameter.IsLentBinding);

    private FlowState Declare(IParameterNode parameter, bool isLent)
    {
        var builder = ToBuilder();
        var bindingValuePairs = BindingValue.ForType(parameter.ValueId, parameter.BindingType);
        foreach (var (value, flowCapability) in bindingValuePairs)
        {
            var capability = flowCapability.Original;
            var sharingIsTracked = capability.SharingIsTracked();
            // These capabilities don't have to worry about external references
            var needsExternalReference = capability != Capability.Isolated
                                         && capability != Capability.TemporarilyIsolated;

            if (!sharingIsTracked)
                builder.AddUntracked(value);
            else if (!needsExternalReference)
                builder.AddSet(isLent, value, flowCapability);
            else if (isLent)
                // Lent parameters each have their own external reference
                builder.AddSet(isLent, value, flowCapability, ExternalReference.CreateLentParameter(value));
            else
                // Non-lent parameters share the same external reference
                builder.AddToNonLentParameterSet(value, flowCapability);
        }

        return builder.ToImmutable();
    }

    public IFlowState Declare(INamedBindingNode binding, ValueId? initializerValueId)
    {
        var builder = ToBuilder();
        var initializerValue = initializerValueId is ValueId v ? ResultValue.Create(v) : null;
        var initializerSet = initializerValue is not null ? builder.TrySetFor(initializerValue) : null;
        var bindingValuePairs = BindingValue.ForType(binding.ValueId, binding.BindingType);
        foreach (var (value, flowCapability) in bindingValuePairs)
        {
            if (!flowCapability.Original.SharingIsTracked())
                builder.AddUntracked(value);
            else if (initializerSet is int set)
                builder.AddToSet(set, value, flowCapability);
            else
                builder.AddSet(false, value, flowCapability);
        }

        if (initializerValue is not null)
            builder.Remove(initializerValue);

        return builder.ToImmutable();
    }

    public IFlowState Constant(ValueId valueId)
    {
        var builder = ToBuilder();
        var value = ResultValue.Create(valueId);
        builder.AddUntracked(value);
        return builder.ToImmutable();
    }

    public IFlowState Alias(IBindingNode? binding, ValueId valueId)
    {
        // TODO maybe sharing should be tracked even in this case? Or should it be treated as untracked?
        if (binding is null) return this;

        var builder = ToBuilder();
        var bindingValue = BindingValue.TopLevel(binding);
        var result = ResultValue.Create(valueId);
        if (binding.SharingIsTracked())
        {
            var set = builder.TrySetFor(bindingValue) ?? throw new InvalidOperationException();
            var flowCapability = builder[bindingValue].WhenAliased();
            builder.AddToSet(set, result, flowCapability);
        }
        else
            builder.AddUntracked(result);

        return builder.ToImmutable();
    }

    public DataType Type(IBindingNode? binding) => Type(binding, Functions.Identity);

    public DataType AliasType(IBindingNode? binding) => Type(binding, c => c.OfAlias());

    private DataType Type(IBindingNode? binding, Func<Capability, Capability> transform)
    {
        if (binding is null) return DataType.Unknown;
        if (!binding.SharingIsTracked())
            // Other types don't have capabilities and don't need to be tracked
            return binding.BindingType.ToUpperBound();

        var bindingValue = BindingValue.TopLevel(binding);
        var current = values[bindingValue].Current;
        // TODO what about independent parameters?
        return ((CapabilityType)binding.BindingType.ToUpperBound()).With(transform(current));
    }

    public bool IsIsolated(IBindingNode? binding)
        => binding is null || IsIsolated(values.Sets.TrySetFor(BindingValue.TopLevel(binding)));

    private static bool IsIsolated(IImmutableDisjointSet<IValue, SharingSetState>? set)
        => set?.Count == 1;

    public bool IsIsolatedExceptFor(IBindingNode? binding, ValueId? valueId)
    {
        return binding is null || (valueId is ValueId v
            ? IsIsolatedExceptFor(values.Sets.TrySetFor(BindingValue.TopLevel(binding)), ResultValue.Create(v))
            : IsIsolated(binding));
    }

    private static bool IsIsolatedExceptFor(IImmutableDisjointSet<IValue, SharingSetState>? set, IValue value)
        => set?.Count <= 2 && set.Except(value).Count() == 1;

    public bool CanFreezeExceptFor(IBindingNode? binding, ValueId? valueId)
    {
        if (binding is null) return true;
        var bindingValue = BindingValue.TopLevel(binding);
        var set = values.Sets.TrySetFor(bindingValue);
        if (set is null) return false;
        if (IsIsolated(set)) return true;

        var exceptValue = valueId is ValueId v ? ResultValue.Create(v) : null;
        foreach (var value in set.Except(bindingValue).Except(exceptValue))
        {
            if (value is ICapabilityValue capabilityValue)
            {
                // The modified capability is what matters because lending can go out of scope later
                var capability = values[capabilityValue].Modified;
                if (capability.AllowsWrite) return false;
            }
            else
                // All other sharing variable types prevent freezing
                return false;
        }

        return true;
    }

    public IFlowState CombineArguments(IEnumerable<ArgumentValueId> arguments, ValueId returnValueId)
    {
        // TODO what about independent parameters?
        var argumentResults = arguments.Select(a => new ArgumentResultValue(a.IsLent, a.ValueId)).ToFixedList();

        var builder = ToBuilder();
        // Union sets for all non-lent arguments
        int? set = builder.Union(argumentResults.Where(a => !a.IsLent && !untrackedValues.Contains(a.Value))
                                                .Select(a => a.Value));

        // Add the return value to the unioned set
        // TODO what is the correct flow capability for the result?
        builder.AddToSet(set, false, ResultValue.Create(returnValueId), default);

        // Now remove all the arguments
        builder.Remove(argumentResults.Select(a => a.Value));

        return builder.ToImmutable();
    }

    public IFlowState AccessMember(ValueId contextValueId, ValueId valueId, DataType memberType)
    {
        var builder = ToBuilder();
        var contextResultValue = ResultValue.Create(contextValueId);
        var resultValue = ResultValue.Create(valueId);
        if (memberType.SharingIsTracked())
        {
            var set = builder.TrySetFor(contextResultValue) ?? throw new InvalidOperationException();
            // TODO what is the correct flow capability for the result?
            builder.AddToSet(set, resultValue, default);
        }
        else
            builder.AddUntracked(resultValue);

        builder.Remove(contextResultValue);

        return builder.ToImmutable();
    }

    public IFlowState Merge(IFlowState? other)
    {
        if (other is null || ReferenceEquals(this, other) || other.IsEmpty)
            return this;
        if (IsEmpty)
            return other;

        if (other is not FlowState otherFlowState)
            throw new InvalidOperationException($"Cannot merge flow state of type {other.GetType()}.");

        var builder = ToBuilder();
        foreach (var otherSet in otherFlowState.values.Sets)
        {
            int? set = null;
            foreach (var value in otherSet.OfType<ICapabilityValue>())
            {
                var valueSet = builder.TrySetFor(value);
                if (valueSet is null)
                {
                    // Add any value that doesn't exist in the current flow state
                    valueSet = builder.AddSet(otherSet.Data.IsLent, value, otherFlowState.values[value]);
                    builder.MarkTracked(value); // In case it was untracked, mark it as tracked
                }
                // TODO else merge capabilities

                set = builder.Union(set, valueSet);
            }
            // TODO marge "lent" state
        }

        // Add any untracked values that don't exist in the current flow state
        foreach (var value in otherFlowState.untrackedValues)
            if (!values.Contains(value))
                builder.AddUntracked(value);

        return builder.ToImmutable();
    }

    public IFlowState Transform(ValueId? valueId, ValueId intoValueId, DataType withType)
    {
        if (valueId is not ValueId fromValueId) return this;

        var builder = ToBuilder();
        var fromValue = ResultValue.Create(fromValueId);
        var intoValue = ResultValue.Create(intoValueId);
        // TODO if the original value was untracked, does that meant the result should be untracked?
        if (withType.SharingIsTracked())
        {
            FlowCapability flowCapability = default;
            if (withType is CapabilityType withCapabilityType)
                flowCapability = builder[fromValue].With(withCapabilityType.Capability);
            var set = builder.TrySetFor(fromValue) ?? throw new InvalidOperationException();
            builder.AddToSet(set, intoValue, flowCapability);
        }
        else
            builder.AddUntracked(intoValue);

        builder.Remove(fromValue);
        return builder.ToImmutable();
    }

    public IFlowState Combine(ValueId left, ValueId? right, ValueId intoValueId)
    {
        var resultValues = right.YieldValue().Prepend(left).Select(ResultValue.Create).ToFixedList();

        var builder = ToBuilder();
        int? set = builder.Union(resultValues);

        // Add the return value to the unioned set
        // TODO what is the correct flow capability for the result?
        builder.AddToSet(set, false, ResultValue.Create(intoValueId), default);

        // Now remove all the inputs
        builder.Remove(resultValues);

        return builder.ToImmutable();
    }

    public IFlowState FreezeVariable(IBindingNode? binding, ValueId valueId, ValueId intoValueId)
        => Freeze(binding, valueId, intoValueId);

    public IFlowState FreezeValue(ValueId valueId, ValueId intoValueId)
        => Freeze(null, valueId, intoValueId);

    private FlowState Freeze(IBindingNode? binding, ValueId valueId, ValueId intoValueId)
    {
        var builder = ToBuilder();
        if (binding is not null)
        {
            var bindingValuePairs
                = BindingValue.ForType(binding.ValueId, (CapabilityType)binding.BindingType.ToUpperBound());
            foreach (var bindingValue in bindingValuePairs.Select(p => p.Key))
                builder.UpdateCapability(bindingValue, c => c.AfterFreeze());
        }

        var oldValue = ResultValue.Create(valueId);
        var newValue = ResultValue.Create(intoValueId);
        // If the value could reference `temp const` data, then it needs to be tracked. (However,
        // that could be detected by looking at whether the set is lent or not, correct?)
        var set = builder.TrySetFor(oldValue) ?? throw new InvalidOperationException();
        builder.AddToSet(set, newValue, default);  // TODO what is the correct flow capability for the result?
        builder.Remove(oldValue);
        return builder.ToImmutable();
    }

    public IFlowState MoveVariable(IBindingNode? binding, ValueId valueId, ValueId intoValueId)
        => Move(binding, valueId, intoValueId);

    public IFlowState MoveValue(ValueId valueId, ValueId intoValueId)
        => Move(null, valueId, intoValueId);

    private FlowState Move(IBindingNode? binding, ValueId valueId, ValueId intoValueId)
    {
        var builder = ToBuilder();
        var oldValue = ResultValue.Create(valueId);
        var newValue = ResultValue.Create(intoValueId);
        if (builder.TrySetFor(oldValue) is int set)
            builder.AddToSet(set, newValue, default); // TODO what is the correct flow capability for the result?
        else
            builder.AddUntracked(newValue);

        if (binding is not null)
        {
            var bindingValues = BindingValue
                                .ForType(binding.ValueId, (CapabilityType)binding.BindingType.ToUpperBound())
                                .Where(p => p.Value.Original.SharingIsTracked())
                                .Select(p => p.Key);
            foreach (var bindingValue in bindingValues)
                // TODO these are now `id`, doesn't that mean they no longer need tracked?
                builder.UpdateCapability(bindingValue, c => c.AfterMove());
        }

        // Old binding values are now `id` and no longer need tracked
        builder.Remove(oldValue);
        return builder.ToImmutable();
    }

    public IFlowState TempFreeze(ValueId valueId, ValueId intoValueId)
        => TemporarilyConvert(valueId, intoValueId, TempConversionTo.Constant(intoValueId));

    public IFlowState TempMove(ValueId valueId, ValueId intoValueId)
        => TemporarilyConvert(valueId, intoValueId, TempConversionTo.Isolated(intoValueId));

    private FlowState TemporarilyConvert(ValueId valueId, ValueId intoValueId, TempConversionTo to)
    {
        var builder = ToBuilder();
        var oldValue = ResultValue.Create(valueId);
        var currentFlowCapabilities = builder[oldValue];
        var oldSet = builder.Remove(oldValue)!.Value;
        builder.UpdateSet(oldSet, state => state.Add(to.From));
        builder.ApplyRestrictions(oldSet);

        var newValue = ResultValue.Create(intoValueId);
        var newFlowCapabilities = currentFlowCapabilities.With(to.Capability);
        builder.AddSet(new SharingSetState(true, to), newValue, newFlowCapabilities);
        return builder.ToImmutable();
    }

    public IFlowState DropBindings(IEnumerable<INamedBindingNode> bindings)
    {
        var builder = ToBuilder();
        builder.Remove(bindings.SelectMany(b => BindingValue.ForType(b.ValueId, b.BindingType).Select(p => p.Key)));
        return builder.ToImmutable();
    }

    public IFlowState DropValue(ValueId valueId)
    {
        var builder = ToBuilder();
        builder.Remove(ResultValue.Create(valueId));
        return builder.ToImmutable();
    }

    #region Equality
    public bool Equals(FlowState? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        // Check collection sizes first to avoid iterating over the collections
        if (values.Count != other.values.Count
            || values.Sets.Count != other.values.Sets.Count
            || untrackedValues.Count != other.untrackedValues.Count
            // Check the hash code first since it is cached
            || GetHashCode() != other.GetHashCode())
            return false;

        // Already checked that the sizes are the same, so it suffices to check that all the
        // entries in one are in the other and equal.
        return values.All(p => other.values.TryGetValue(p.Key, out var value) && p.Value.Equals(value))
               && untrackedValues.SetEquals(other.untrackedValues)
               // Check the sets last since they are the most expensive to compare
               && AreEqual(values.Sets, other.values.Sets);
    }

    private static bool AreEqual(
        IImmutableDisjointSets<IValue, FlowCapability, SharingSetState>.ISetCollection sets1,
        IImmutableDisjointSets<IValue, FlowCapability, SharingSetState>.ISetCollection sets2)
    {
        // We assume the counts are equal, so we only need to check that the sets are equal
        var lookup = sets1.ToLookup(ComputeHashCode);
        return sets2.All(set => lookup[ComputeHashCode(set)].Any(s => s.SetEquals(set)));
    }

    private static int ComputeHashCode(IImmutableDisjointSet<IValue, SharingSetState> set)
    {
        int itemHash = 0;
        foreach (var value in set)
            itemHash ^= value.GetHashCode();
        return HashCode.Combine(set.Count, set.Data, itemHash);
    }

    public bool Equals(IFlowState? obj)
        => ReferenceEquals(this, obj) || obj is FlowState other && Equals(other);

    public override bool Equals(object? obj)
        => ReferenceEquals(this, obj) || obj is FlowState other && Equals(other);

    public override int GetHashCode()
        // ReSharper disable NonReadonlyMemberInGetHashCode
        => hashCode != 0 ? hashCode : hashCode = ComputeHashCode();
    // ReSharper restore NonReadonlyMemberInGetHashCode

    private int ComputeHashCode()
    {
        var hash = new HashCode();
        hash.Add(values.Count);
        hash.Add(values.Sets.Count);
        foreach (var set in values.Sets.OrderByDescending(s => s.Count))
            hash.Add(set.Count);
        hash.Add(untrackedValues.Count);
        return hash.ToHashCode();
    }
    #endregion

    private Builder ToBuilder()
        => new Builder(values, untrackedValues);

    private readonly struct Builder
    {
        private readonly IImmutableDisjointSets<IValue, FlowCapability, SharingSetState>.IBuilder values;
        private readonly ImmutableHashSet<IValue>.Builder untrackedValues;

        public Builder(IImmutableDisjointSets<IValue, FlowCapability, SharingSetState> values,
            ImmutableHashSet<IValue> untrackedValues)
        {
            this.values = values.ToBuilder(SetRemoved);
            this.untrackedValues = untrackedValues.ToBuilder();
        }

        private static void SetRemoved(
            IImmutableDisjointSets<IValue, FlowCapability, SharingSetState>.IBuilder values,
            SharingSetState state)
        {
            if (!state.Conversions.IsEmpty)
                DropConversions(values, state.Conversions.OfType<TempConversionTo>().Select(c => c.From));
        }

        private static void DropConversions(
            IImmutableDisjointSets<IValue, FlowCapability, SharingSetState>.IBuilder values,
            IEnumerable<IConversion> conversionsRemoved)
        {
            var conversionsRemovedList = conversionsRemoved.ToFixedList();
            if (conversionsRemovedList.IsEmpty)
                // Avoid calling GetSetsForConversions if there are no conversions
                return;
            var setForConversion = GetSetsForConversions(values);
            foreach (var conversions in conversionsRemovedList.GroupBy(c => setForConversion[c]))
            {
                var setIndex = conversions.Key;
                values.UpdateSet(setIndex, d => d.Remove(conversions));
                LiftRemovedRestrictions(values, setIndex);
            }
        }

        private static Dictionary<IConversion, int> GetSetsForConversions(
            IImmutableDisjointSets<IValue, FlowCapability, SharingSetState>.IBuilder values)
            => values.SetData().SelectMany(p => p.Value.Conversions.Select(c => (c, p.Key))).ToDictionary();

        public FlowCapability this[ICapabilityValue value] => values[value];

        public int? TrySetFor(IValue value) => values.TrySetFor(value);

        public void AddUntracked(IValue value) => untrackedValues.Add(value);

        public void MarkTracked(ICapabilityValue value) => untrackedValues.Remove(value);

        public int AddSet(bool isLent, ICapabilityValue value, FlowCapability flowCapability)
            => values.AddSet(new(isLent), value, flowCapability);

        public int AddSet(SharingSetState setState, ICapabilityValue value, FlowCapability flowCapability)
            => values.AddSet(setState, value, flowCapability);

        public void AddSet(bool isLent, BindingValue value, FlowCapability flowCapability, ExternalReference reference)
        {
            var setId = values.AddSet(new(isLent), value, flowCapability);
            values.AddToSet(setId, reference, default);
        }

        [Inline]
        private int NonLentParametersSet()
        {
            if (values.TrySetFor(ExternalReference.NonLentParameters) is int set) return set;

            return values.AddSet(new(false), ExternalReference.NonLentParameters, default);
        }

        public void AddToNonLentParameterSet(BindingValue value, FlowCapability flowCapability)
            => values.AddToSet(NonLentParametersSet(), value, flowCapability);

        public void AddToSet(int set, ICapabilityValue value, FlowCapability flowCapability)
            => values.AddToSet(set, value, flowCapability);

        public void AddToSet(int? set, bool isLent, ICapabilityValue value, FlowCapability flowCapability)
        {
            if (set is int s)
                values.AddToSet(s, value, flowCapability);
            else
                AddSet(isLent, value, flowCapability);
        }

        public int? Remove(IValue value)
            => untrackedValues.Remove(value) ? null : values.Remove(value);

        public void Remove(IEnumerable<IValue> values)
        {
            foreach (var value in values)
                Remove(value);
        }

        public int? Union(int? set1, int? set2)
        {
            if (set1 is int s1 && set2 is int s2)
                return values.Union(s1, s2);
            return set1 ?? set2;
        }

        public int? Union(IEnumerable<IValue> values)
        {
            int? set = null;
            foreach (var value in values)
            {
                if (untrackedValues.Contains(value))
                    continue;
                var valueSet = this.values.TrySetFor(value)
                               ?? throw new InvalidOperationException("Attempt to union value that is not in flow state");
                set = Union(set, valueSet);
            }
            return set;
        }

        public void UpdateSet(int set, Func<SharingSetState, SharingSetState> update)
            => values.UpdateSet(set, update);

        public void UpdateCapability(ICapabilityValue value, Func<FlowCapability, FlowCapability> transform)
            => values[value] = transform(values[value]);

        public void ApplyRestrictions(int set)
            => ApplyRestrictions(values, set, applyReadWriteRestriction: true);

        private static void ApplyRestrictions(
            IImmutableDisjointSets<IValue, FlowCapability, SharingSetState>.IBuilder values,
            int set,
            bool applyReadWriteRestriction)
        {
            var restrictions = values.SetData(set).Restrictions;
            if (!applyReadWriteRestriction && restrictions == CapabilityRestrictions.ReadWrite) return;
            foreach (var value in values.SetItems(set))
                values[value] = values[value].WithRestrictions(restrictions);
        }

        [Inline]
        private static void LiftRemovedRestrictions(
                IImmutableDisjointSets<IValue, FlowCapability, SharingSetState>.IBuilder values,
                int set)
            // Don't apply read/write restrictions since they have already been applied
            => ApplyRestrictions(values, set, applyReadWriteRestriction: false);

        public FlowState ToImmutable()
            => new(values.ToImmutable(), untrackedValues.ToImmutable());
    }
}
