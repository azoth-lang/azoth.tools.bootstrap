using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow.Sharing;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;
using Azoth.Tools.Bootstrap.Framework;
using Azoth.Tools.Bootstrap.Framework.Collections;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;

internal sealed class FlowState : IFlowState
{
    public static FlowState Empty { get; } = new FlowState();

    private readonly IImmutableDisjointSets<IValue, FlowCapability, SharingSetState> values;
    private int hashCode;
    public bool IsEmpty => values.Count == 0;

    private FlowState()
    {
        values = ImmutableDisjointHashSets<IValue, FlowCapability, SharingSetState>.Empty;
    }

    private FlowState(IImmutableDisjointSets<IValue, FlowCapability, SharingSetState> values)
    {
        this.values = values;
    }

    public IFlowState Declare(INamedParameterNode parameter)
    {
        var bindingType = parameter.BindingType;
        bool sharingIsTracked = parameter.ParameterType.SharingIsTracked();
        return Declare(parameter, bindingType, sharingIsTracked, parameter.IsLentBinding);
    }

    public IFlowState Declare(ISelfParameterNode parameter)
    {
        var bindingType = parameter.BindingType;
        bool sharingIsTracked = parameter.ParameterType.SharingIsTracked();
        return Declare(parameter, bindingType, sharingIsTracked, parameter.IsLentBinding);
    }

    private FlowState Declare(
        IParameterNode parameter,
        Pseudotype bindingType,
        bool sharingIsTracked, bool isLent)
    {
        if (!sharingIsTracked) return this;

        var builder = ToBuilder();

        var bindingValuePairs = BindingValue.ForType(parameter.ValueId, bindingType);
        foreach (var (value, flowCapability) in bindingValuePairs)
        {
            var capability = flowCapability.Original;
            // These capabilities don't have to worry about external references
            var needsExternalReference = capability != Capability.Isolated
                                         && capability != Capability.TemporarilyIsolated
                                         && capability != Capability.Constant
                                         && capability != Capability.Identity;

            if (!needsExternalReference)
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
            // TODO skip if sharing isn't tracked?
            if (initializerSet is int set)
                builder.AddToSet(set, value, flowCapability);
            else
                builder.AddSet(false, value, flowCapability);
        }

        if (initializerValue is not null)
            builder.Remove(initializerValue);

        return builder.ToImmutable();
    }

    public IFlowState Alias(IBindingNode? binding, ValueId valueId)
    {
        if (binding is null || !binding.SharingIsTracked())
            // If the binding isn't tracked, then the alias isn't either
            return this;

        var builder = ToBuilder();
        var bindingValue = BindingValue.TopLevel(binding);
        FlowCapability flowCapability = builder[bindingValue].WhenAliased();
        if (binding.SharingIsTracked(flowCapability))
        {
            var set = builder.TrySetFor(bindingValue) ?? throw new InvalidOperationException();
            var result = ResultValue.Create(valueId);
            builder.AddToSet(set, result, flowCapability);
        }

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
        int? set = builder.Union(argumentResults.Where(a => !a.IsLent).Select(a => a.Value));

        // Add the return value to the unioned set
        // TODO what is the correct flow capability for the result?
        builder.AddToSet(set, false, ResultValue.Create(returnValueId), default);

        // Now remove all the arguments
        builder.Remove(argumentResults.Select(a => a.Value));

        return builder.ToImmutable();
    }

    public IFlowState AccessMember(ValueId contextValueId, ValueId valueId, DataType memberType)
    {
        var contextResultValue = ResultValue.Create(contextValueId);
        // If accessing from non-tracked type, then it's not tracked
        // TODO this might be bug prone if the value was accidentally not in the flow state
        if (!values.Contains(contextResultValue)) return this;

        var builder = ToBuilder();
        if (memberType.SharingIsTracked())
        {
            var resultValue = ResultValue.Create(valueId);
            var set = builder.TrySetFor(contextResultValue) ?? throw new InvalidOperationException();
            // TODO what is the correct flow capability for the result?
            builder.AddToSet(set, resultValue, default);
        }

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
        throw new NotImplementedException();

        //foreach (var otherSet in otherFlowState.sets)
        //{
        //    if (!otherSet.Any(builder.Contains))
        //    {
        //        // Whole set is missing, add it to the flow state (safe because it is immutable)
        //        builder.AddSet(otherSet);
        //        continue;
        //    }

        //    // Union all the sets together since they all contain items in the same set in the
        //    // other flow state. (Doing this as a single operation is more efficient). Also include
        //    // the other set for any values or conversions that don't exist in the current flow state.
        //    var setsToUnion = otherSet.Select(builder.TrySetFor).WhereNotNull().Append(otherSet);
        //    builder.Union(setsToUnion);
        //}

        //foreach (var (value, flowCapability) in otherFlowState.capabilities)
        //{
        //    var existingCapability = builder.TryCapabilityFor(value);
        //    if (existingCapability is null)
        //        builder.TrackFlowCapability(value, flowCapability);
        //    else if (existingCapability != flowCapability)
        //        throw new InvalidOperationException(
        //            $"Flow capability for {value} changed from {existingCapability} to {flowCapability}.");
        //}

        //return builder.ToImmutable();
    }

    public IFlowState Transform(ValueId? valueId, ValueId intoValueId, DataType withType)
    {
        if (valueId is not ValueId fromValueId) return this;

        var fromValue = ResultValue.Create(fromValueId);
        if (!values.Contains(fromValue)) return this;

        var builder = ToBuilder();
        if (withType.SharingIsTracked())
        {
            var intoValue = ResultValue.Create(intoValueId);
            FlowCapability flowCapability = default;
            if (withType is CapabilityType withCapabilityType)
                flowCapability = builder[fromValue].With(withCapabilityType.Capability);
            var set = builder.TrySetFor(fromValue) ?? throw new InvalidOperationException();
            builder.AddToSet(set, intoValue, flowCapability);
        }
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
        var oldValue = ResultValue.Create(valueId);
        if (!values.Contains(oldValue))
            // TODO shouldn't this be an error?
            return this;

        var builder = ToBuilder();
        if (binding is not null)
        {
            var bindingValuePairs
                = BindingValue.ForType(binding.ValueId, (CapabilityType)binding.BindingType.ToUpperBound());
            foreach (var bindingValue in bindingValuePairs.Select(p => p.Key))
                builder.UpdateCapability(bindingValue, c => c.AfterFreeze());
        }

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
        var oldValue = ResultValue.Create(valueId);
        if (!values.Contains(oldValue)) return this;

        var builder = ToBuilder();

        var newValue = ResultValue.Create(intoValueId);
        var set = builder.TrySetFor(oldValue) ?? throw new InvalidOperationException();
        builder.AddToSet(set, newValue, default); // TODO what is the correct flow capability for the result?

        if (binding is not null)
        {
            var bindingValues = BindingValue
                                .ForType(binding.ValueId, (CapabilityType)binding.BindingType.ToUpperBound())
                                .Select(p => p.Key);
            foreach (var bindingValue in bindingValues)
            {
                builder.UpdateCapability(bindingValue, c => c.AfterMove());
            }

            // Old binding values are now `id` and no longer need tracked
            // TODO but then why update them?
            builder.Remove(oldValue);
        }

        builder.Remove(oldValue);
        return builder.ToImmutable();
    }

    public IFlowState TempFreeze(ValueId valueId, ValueId intoValueId)
        => TemporarilyConvert(valueId, intoValueId, TempConversionTo.Constant(intoValueId));

    public IFlowState TempMove(ValueId valueId, ValueId intoValueId)
        => TemporarilyConvert(valueId, intoValueId, TempConversionTo.Isolated(intoValueId));

    private FlowState TemporarilyConvert(ValueId valueId, ValueId intoValueId, TempConversionTo to)
    {
        var oldValue = ResultValue.Create(valueId);
        if (!values.Contains(oldValue))
            // TODO shouldn't this be an error?
            return this;

        var builder = ToBuilder();
        var currentFlowCapabilities = builder[oldValue];
        var newValue = ResultValue.Create(intoValueId);
        builder.UpdateCapability(newValue, c => c.With(to.Capability));
        throw new NotImplementedException();
        //var currentFlowCapabilities = builder.CapabilityFor(oldValue);
        //var newValue = ResultValue.Create(intoValueId);
        //builder.TrackFlowCapability(newValue, currentFlowCapabilities.With(to.Capability));
        //var newSet = oldSet.Replace(oldValue, to.From);
        //builder.UpdateSet(oldSet, newSet);
        //builder.AddSet(SharingSet.DeclareConversion(true, newValue, to));
        //builder.ApplyRestrictions(newSet);
        //return builder.ToImmutable();
    }

    public IFlowState DropBindings(IEnumerable<INamedBindingNode> bindings)
    {
        var builder = ToBuilder();
        // TODO what about independent parameters?
        builder.Remove(bindings.Select(BindingValue.TopLevel));
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
            // Check the hash code first since it is cached
            || GetHashCode() != other.GetHashCode())
            return false;

        // Already checked that the sizes are the same, so it suffices to check that all the
        // entries in one are in the other and equal.
        return values.All(p => other.values.TryGetValue(p.Key, out var value) && p.Value.Equals(value))
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
        return hash.ToHashCode();
    }
    #endregion

    private Builder ToBuilder()
        => new Builder(values);

    private readonly struct Builder
    {
        private readonly IImmutableDisjointSets<IValue, FlowCapability, SharingSetState>.IBuilder values;

        public Builder(IImmutableDisjointSets<IValue, FlowCapability, SharingSetState> values)
        {
            this.values = values.ToBuilder();
        }

        public FlowCapability this[ICapabilityValue value] => values[value];

        public bool Contains(IValue value) => values.Contains(value);

        public int? TrySetFor(IValue value) => values.TrySetFor(value);

        public int AddSet(bool isLent, ICapabilityValue value, FlowCapability flowCapability)
            => values.AddSet(new(isLent), value, flowCapability);

        public void AddSet(bool isLent, BindingValue value, FlowCapability flowCapability, ExternalReference reference)
        {
            var setId = values.AddSet(new(isLent), value, flowCapability);
            values.AddToSet(setId, reference, default);
        }

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

        public void Remove(IValue value) => values.Remove(value);

        public void Remove(IEnumerable<IValue> values)
        {
            foreach (var value in values)
                this.values.Remove(value);
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
                var valueSet = this.values.TrySetFor(value);
                set = Union(set, valueSet);
            }
            return set;
        }

        public void UpdateCapability(ICapabilityValue value, Func<FlowCapability, FlowCapability> transform)
            => values[value] = transform(values[value]);

        public FlowState ToImmutable() => new FlowState(values.ToImmutable());
    }
}
