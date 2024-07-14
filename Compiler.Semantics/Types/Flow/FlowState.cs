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
        //var initializerValue = initializerValueId is ValueId v ? ResultValue.Create(v) : null;
        //// TODO other types besides CapabilityType might participate in sharing
        //if (binding.BindingType is not CapabilityType capabilityType)
        //{
        //    if (initializerValue is null) return this;
        //    // TODO is this needed? Or does the fact that it isn't a capability type mean the initializer is already untracked?
        //    var b = ToBuilder();
        //    b.Drop(initializerValue);
        //    return b.ToImmutable();
        //}

        //var bindingValuePairs = BindingValue.ForType(binding.ValueId, capabilityType);
        //var builder = ToBuilder();
        //builder.AddFlowCapabilities(bindingValuePairs);

        //if (!binding.BindingType.SharingIsTracked())
        //    return builder.ToImmutable();

        //var initializerSet = initializerValue is not null ? builder.TrySetFor(initializerValue) : null;
        //var isLent = initializerSet?.IsLent ?? false;
        //var newSharingSets = bindingValuePairs.Select(p => SharingSet.Declare(isLent, p.Key)).ToFixedList();
        //builder.AddSets(newSharingSets);

        //if (initializerValue is not null)
        //{
        //    // NOTE: this is assuming that the top level value is always first
        //    builder.Union(initializerSet.YieldValue().Append(newSharingSets.First()));
        //    builder.Drop(initializerValue);
        //}

        //return builder.ToImmutable();
        throw new NotImplementedException();
    }

    public IFlowState Alias(IBindingNode? binding, ValueId valueId) => throw new NotImplementedException();

    public DataType Type(IBindingNode? binding) => throw new NotImplementedException();

    public DataType AliasType(IBindingNode? binding) => throw new NotImplementedException();

    public bool IsIsolated(IBindingNode? binding) => throw new NotImplementedException();

    public bool IsIsolatedExceptFor(IBindingNode? binding, ValueId? valueId) => throw new NotImplementedException();

    public bool CanFreezeExceptFor(IBindingNode? binding, ValueId? valueId) => throw new NotImplementedException();

    public IFlowState CombineArguments(IEnumerable<ArgumentValueId> arguments, ValueId returnValueId) => throw new NotImplementedException();

    public IFlowState AccessMember(ValueId contextValueId, ValueId valueId, DataType memberType) => throw new NotImplementedException();

    public IFlowState Merge(IFlowState? other) => throw new NotImplementedException();

    public IFlowState Transform(ValueId? valueId, ValueId intoValueId, DataType withType) => throw new NotImplementedException();

    public IFlowState Combine(ValueId left, ValueId? right, ValueId intoValueId) => throw new NotImplementedException();

    public IFlowState FreezeVariable(IBindingNode? binding, ValueId valueId, ValueId intoValueId) => throw new NotImplementedException();

    public IFlowState FreezeValue(ValueId valueId, ValueId intoValueId) => throw new NotImplementedException();

    public IFlowState MoveVariable(IBindingNode? binding, ValueId valueId, ValueId intoValueId) => throw new NotImplementedException();

    public IFlowState MoveValue(ValueId valueId, ValueId intoValueId) => throw new NotImplementedException();

    public IFlowState TempFreeze(ValueId valueId, ValueId intoValueId) => throw new NotImplementedException();

    public IFlowState TempMove(ValueId valueId, ValueId intoValueId) => throw new NotImplementedException();

    public IFlowState DropBindings(IEnumerable<INamedBindingNode> bindings) => throw new NotImplementedException();

    public IFlowState DropValue(ValueId valueId) => throw new NotImplementedException();

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

        public FlowState ToImmutable() => new FlowState(values.ToImmutable());
    }
}
