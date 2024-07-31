using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow.Sharing;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;
using Azoth.Tools.Bootstrap.Framework;
using DataType = Azoth.Tools.Bootstrap.Compiler.Types.DataType;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;

/// <summary>
/// Wraps up all the state that changes with the flow of the code to make it easy to attach to each
/// node in the semantic tree.
/// </summary>
public sealed class LegacyFlowState : IFlowState
{
    public static readonly LegacyFlowState Empty = new();

    private readonly ImmutableDictionary<ICapabilityValue, FlowCapability> capabilities;

    /// <summary>
    /// All the distinct subsets of values.
    /// </summary>
    private readonly ImmutableHashSet<SharingSet> sets;

    /// <summary>
    /// This is a lookup of what set each value is contained in.
    /// </summary>
    private readonly ImmutableDictionary<IValue, SharingSet> setFor;

    public bool IsEmpty
        => ReferenceEquals(this, Empty)
           || (capabilities.IsEmpty && sets.IsEmpty && setFor.IsEmpty);

    private LegacyFlowState()
    {
        capabilities = ImmutableDictionary<ICapabilityValue, FlowCapability>.Empty;
        sets = ImmutableHashSet<SharingSet>.Empty;
        setFor = ImmutableDictionary<IValue, SharingSet>.Empty;
    }

    private LegacyFlowState(
        ImmutableDictionary<ICapabilityValue, FlowCapability> capabilities,
        ImmutableHashSet<SharingSet> sets,
        ImmutableDictionary<IValue, SharingSet> setFor)
    {
        this.capabilities = capabilities;
        this.sets = sets;
        this.setFor = setFor;
    }

    /// <summary>
    /// Declare the given parameter as part of the flow state including any independent parameters.
    /// </summary>
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

    private LegacyFlowState Declare(
        IParameterNode parameter,
        Pseudotype bindingType,
        bool sharingIsTracked, bool isLent)
    {
        // TODO other types besides CapabilityType might participate in sharing
        if (bindingType.ToUpperBound() is not CapabilityType capabilityType)
            return this;

        var bindingValuePairs = BindingValue.ForType(parameter.BindingValueId, capabilityType);
        var builder = ToBuilder();
        builder.AddFlowCapabilities(bindingValuePairs);

        if (!sharingIsTracked)
            return builder.ToFlowState();

        var capability = capabilityType.Capability;
        // These capabilities don't have to worry about external references
        var needsExternalReference = capability != Capability.Isolated
                                     && capability != Capability.TemporarilyIsolated
                                     && capability != Capability.Constant
                                     && capability != Capability.Identity;
        if (!needsExternalReference)
        {
            var newSharingSets = bindingValuePairs.Select(p => SharingSet.Declare(isLent, p.Value))
                                                  .ToFixedList();
            builder.AddSets(newSharingSets);
        }
        else if (isLent)
        {
            // Lent parameters each have their own external reference
            var newSharingSets = bindingValuePairs
                .Select(p => SharingSet.Declare(isLent, p.Value, ExternalReference.CreateLentParameter(p.Value)))
                .ToFixedList();
            builder.AddSets(newSharingSets);
        }
        else
        {
            // Non-lent parameters share the same external reference
            var newSharingSet = SharingSet.Declare(isLent, bindingValuePairs.Select(p => p.Value));
            builder.AddSet(newSharingSet);
            builder.Union([newSharingSet, builder.NonLentParametersSet()]);
        }

        return builder.ToFlowState();
    }

    public IFlowState Declare(INamedBindingNode binding, ValueId? initializerValueId)
    {
        var initializerValue = initializerValueId is ValueId v ? ResultValue.Create(v) : null;
        // TODO other types besides CapabilityType might participate in sharing
        if (binding.BindingType is not CapabilityType capabilityType)
        {
            if (initializerValue is null)
                return this;
            // TODO is this needed? Or does the fact that it isn't a capability type mean the initializer is already untracked?
            var b = ToBuilder();
            b.Drop(initializerValue);
            return b.ToFlowState();
        }

        var bindingValuePairs = BindingValue.ForType(binding.BindingValueId, capabilityType);
        var builder = ToBuilder();
        builder.AddFlowCapabilities(bindingValuePairs);

        if (!binding.BindingType.SharingIsTracked())
            return builder.ToFlowState();

        var initializerSet = initializerValue is not null ? builder.TrySetFor(initializerValue) : null;
        var isLent = initializerSet?.IsLent ?? false;
        var newSharingSets = bindingValuePairs.Select(p => SharingSet.Declare(isLent, p.Value))
                                              .ToFixedList();
        builder.AddSets(newSharingSets);

        if (initializerValue is not null)
        {
            // NOTE: this is assuming that the top level value is always first
            builder.Union(initializerSet.YieldValue().Append(newSharingSets.First()));
            builder.Drop(initializerValue);
        }

        return builder.ToFlowState();
    }

    public IFlowState Constant(ValueId valueId)
        // Constants are not tracked at all in this implementation
        => this;

    /// <summary>
    /// Make <paramref name="aliasValueId"/> an alias to the <paramref name="binding"/>.
    /// </summary>
    /// <remarks>This does not alias any independent parameters of the binding because only an alias
    /// to the top level object has been created. For example, if <c>iso List[iso Foo]</c> is aliased
    /// the list elements are still isolated. Only the list itself has been aliased and is now
    /// <c>mut</c>.</remarks>
    public IFlowState Alias(IBindingNode? binding, ValueId aliasValueId)
    {
        if (binding is null || !binding.SharingIsTracked())
            // If the binding isn't tracked, then the alias isn't either
            return this;

        var builder = ToBuilder();
        var bindingValue = BindingValue.CreateTopLevel(binding);
        var flowCapability = builder.SetFlowCapability(bindingValue, capabilities[bindingValue].WhenAliased());

        if (binding.SharingIsTracked(flowCapability))
        {
            var set = builder.SetFor(bindingValue);

            var result = ResultValue.Create(aliasValueId);
            var newSet = set.Declare(result);
            builder.UpdateSet(set, newSet);

            builder.TrackFlowCapability(result, flowCapability.OfAlias());
        }

        return builder.ToFlowState();
    }

    /// <summary>
    /// Gives the current flow type of the symbol.
    /// </summary>
    /// <remarks>This is named for it to be used as <c>flow.Type(symbol)</c></remarks>
    public DataType Type(IBindingNode? binding) => Type(binding, Functions.Identity);

    /// <summary>
    /// Gives the type of an alias to the symbol
    /// </summary>
    /// <remarks>This is named for it to be used as <c>flow.AliasType(symbol)</c></remarks>
    public DataType AliasType(IBindingNode? binding) => Type(binding, c => c.OfAlias());

    private DataType Type(IBindingNode? binding, Func<Capability, Capability> transform)
    {
        if (binding is null) return DataType.Unknown;
        if (!binding.SharingIsTracked())
            // Other types don't have capabilities and don't need to be tracked
            return binding.BindingType.ToUpperBound();

        var bindingValue = BindingValue.CreateTopLevel(binding);
        var current = capabilities[bindingValue].Current;
        // TODO what about independent parameters?
        return ((CapabilityType)binding.BindingType.ToUpperBound()).With(transform(current));
    }

    public bool IsIsolated(IBindingNode? binding)
        => binding is null || (TrySetFor(BindingValue.CreateTopLevel(binding))?.IsIsolated ?? false);

    public bool IsIsolatedExceptFor(IBindingNode? binding, ValueId? valueId)
    {
        return binding is null
               || (valueId is ValueId v ? TrySetFor(BindingValue.CreateTopLevel(binding))?.IsIsolatedExceptFor(ResultValue.Create(v)) ?? false
                   : IsIsolated(binding));
    }

    public bool CanFreezeExceptFor(IBindingNode? binding, ValueId? valueId)
    {
        if (binding is null) return true;
        var bindingValue = BindingValue.CreateTopLevel(binding);
        var set = TrySetFor(bindingValue);
        if (set is null) return false;
        if (set.IsIsolated) return true;

        var exceptValue = valueId is ValueId v ? ResultValue.Create(v) : null;
        foreach (var value in set.Except(bindingValue).Except(exceptValue))
        {
            if (value is ICapabilityValue capabilityValue)
            {
                // The modified capability is what matters because lending can go out of scope later
                var capability = capabilities[capabilityValue].Modified;
                if (capability.AllowsWrite) return false;
            }
            else
                // All other sharing variable types prevent freezing
                return false;
        }

        return true;
    }

    /// <summary>
    /// Combine the non-lent values representing the arguments into one sharing set with the return
    /// value id and drop the values for all arguments.
    /// </summary>
    // TODO should storage of return value be based on whether the return type requires tracking?
    public IFlowState CombineArguments(IEnumerable<ArgumentValueId> arguments, ValueId returnValueId, DataType returnType)
    {
        // TODO what about independent parameters?
        var argumentResults = arguments.Select(a => new ArgumentResultValue(a.IsLent, a.ValueId)).ToFixedList();

        var builder = ToBuilder();
        var existingSets = argumentResults.Where(a => !a.IsLent)
                                          .Select(a => builder.TrySetFor(a.Value))
                                          .WhereNotNull().ToList();
        var argumentValues = argumentResults.Select(a => a.Value);

        var resultSet = SharingSet.CombineArguments(existingSets, returnValueId, argumentValues);

        builder.ReplaceSets(existingSets, resultSet);
        builder.Drop(argumentResults.Where(a => a.IsLent).Select(a => a.Value));
        return builder.ToFlowState();
    }

    public IFlowState AccessField(IFieldAccessExpressionNode node)
    {
        var contextValueId = node.Context.ValueId;
        var valueId = node.ValueId;
        var memberType = node.Type;
        var contextResultValue = ResultValue.Create(contextValueId);
        // If accessing from non-tracked type, then it's not tracked
        // TODO this might be bug prone if the value was accidentally not in the flow state
        if (!setFor.TryGetValue(contextResultValue, out var set))
            return this;

        var builder = ToBuilder();
        SharingSet? newSet;
        if (memberType.SharingIsTracked())
        {
            var resultValue = ResultValue.Create(valueId);
            newSet = set.Replace(contextResultValue, resultValue);
        }
        else
            newSet = set.Drop(contextResultValue);

        builder.UpdateSet(set, newSet);
        return builder.ToFlowState();
    }

    public IFlowState Merge(IFlowState? other)
    {
        if (other is null
            || ReferenceEquals(this, other)
            || other.IsEmpty)
            return this;
        if (IsEmpty)
            return other;

        if (other is not LegacyFlowState otherFlowState)
            throw new InvalidOperationException($"Cannot merge flow state of type {other.GetType()}.");

        var builder = ToBuilder();
        foreach (var otherSet in otherFlowState.sets)
        {
            if (!otherSet.Any(builder.Contains))
            {
                // Whole set is missing, add it to the flow state (safe because it is immutable)
                builder.AddSet(otherSet);
                continue;
            }

            // Union all the sets together since they all contain items in the same set in the
            // other flow state. (Doing this as a single operation is more efficient). Also include
            // the other set for any values or conversions that don't exist in the current flow state.
            var setsToUnion = otherSet.Select(builder.TrySetFor).WhereNotNull().Append(otherSet);
            builder.Union(setsToUnion);
        }

        foreach (var (value, flowCapability) in otherFlowState.capabilities)
        {
            var existingCapability = builder.TryCapabilityFor(value);
            if (existingCapability is null)
                builder.TrackFlowCapability(value, flowCapability);
            else if (existingCapability != flowCapability)
                throw new InvalidOperationException($"Flow capability for {value} changed from {existingCapability} to {flowCapability}.");
        }
        return builder.ToFlowState();
    }

    public IFlowState Transform(ValueId? valueId, ValueId toValueId, DataType withType)
    {
        if (valueId is not ValueId fromValueId)
            return this;

        var fromValue = ResultValue.Create(fromValueId);
        var oldSet = TrySetFor(fromValue);
        if (oldSet is null)
            return this;

        var builder = ToBuilder();
        SharingSet? newSet;
        if (withType.SharingIsTracked())
        {
            var intoValue = ResultValue.Create(toValueId);
            newSet = oldSet.Replace(fromValue, intoValue);
            if (withType is CapabilityType withCapabilityType)
            {
                var maybeOldCapability = builder.TryCapabilityFor(fromValue);
                if (maybeOldCapability is FlowCapability oldCapability)
                    builder.TrackFlowCapability(intoValue, oldCapability.With(withCapabilityType.Capability));
            }
        }
        else
        {
            newSet = oldSet.Drop(fromValue);
        }
        builder.UpdateSet(oldSet, newSet);
        builder.RemoveCapability(fromValue);
        return builder.ToFlowState();
    }

    public IFlowState Combine(ValueId left, ValueId? right, ValueId intoValueId)
    {
        var resultValues = right.YieldValue().Prepend(left).Select(ResultValue.Create).ToFixedList();

        var builder = ToBuilder();
        var existingSets = resultValues.Select(TrySetFor).WhereNotNull().ToList();
        var resultSet = SharingSet.CombineArguments(existingSets, intoValueId, resultValues);
        builder.ReplaceSets(existingSets, resultSet);

        return builder.ToFlowState();
    }

    public IFlowState FreezeVariable(IBindingNode? binding, ValueId valueId, ValueId intoValueId)
        => Freeze(binding, valueId, intoValueId);

    public IFlowState FreezeValue(ValueId valueId, ValueId intoValueId)
        => Freeze(null, valueId, intoValueId);

    private LegacyFlowState Freeze(IBindingNode? binding, ValueId valueId, ValueId intoValueId)
    {
        var oldValue = ResultValue.Create(valueId);
        if (TrySetFor(oldValue) is not SharingSet oldSet)
            // TODO shouldn't this be an error?
            return this;

        var builder = ToBuilder();
        if (binding is not null)
        {
            var bindingValuePairs = BindingValue.ForType(binding.BindingValueId, (CapabilityType)binding.BindingType.ToUpperBound());
            foreach (var bindingValue in bindingValuePairs.Select(p => p.Value))
                builder.SetFlowCapability(bindingValue, capabilities[bindingValue].AfterFreeze());
        }

        var newValue = ResultValue.Create(intoValueId);
        // If the value could reference `temp const` data, then it needs to be tracked. (However,
        // that could be detected by looking at whether the set is lent or not, correct?)
        builder.UpdateSet(oldSet, oldSet.Replace(oldValue, newValue));
        return builder.ToFlowState();
    }

    public IFlowState MoveVariable(IBindingNode? binding, ValueId valueId, ValueId intoValueId)
        => Move(binding, valueId, intoValueId);

    public IFlowState MoveValue(ValueId valueId, ValueId intoValueId)
        => Move(null, valueId, intoValueId);

    private LegacyFlowState Move(IBindingNode? binding, ValueId valueId, ValueId intoValueId)
    {
        var oldValue = ResultValue.Create(valueId);
        if (TrySetFor(oldValue) is not SharingSet oldSet)
            return this;

        var builder = ToBuilder();
        IEnumerable<IValue> removeValues = oldValue.Yield();
        if (binding is not null)
        {
            var bindingValues = BindingValue.ForType(binding.BindingValueId, (CapabilityType)binding.BindingType.ToUpperBound())
                                            .Select(p => p.Value).ToList();
            foreach (var bindingValue in bindingValues)
                builder.SetFlowCapability(bindingValue, capabilities[bindingValue].AfterMove());

            // Old binding values are now `id` and no longer need tracked
            // TODO this may not properly remove independent parameters
            removeValues = removeValues.Concat(bindingValues);
        }

        var newValue = ResultValue.Create(intoValueId);
        builder.UpdateSet(oldSet, oldSet.Replace(removeValues, newValue));
        return builder.ToFlowState();
    }

    public IFlowState TempFreeze(ValueId valueId, ValueId intoValueId)
        => TemporarilyConvert(valueId, intoValueId, TempConversionTo.Constant(intoValueId));

    public IFlowState TempMove(ValueId valueId, ValueId intoValueId)
        => TemporarilyConvert(valueId, intoValueId, TempConversionTo.Isolated(intoValueId));

    private LegacyFlowState TemporarilyConvert(ValueId valueId, ValueId intoValueId, TempConversionTo to)
    {
        var oldValue = ResultValue.Create(valueId);
        if (TrySetFor(oldValue) is not SharingSet oldSet)
            // TODO shouldn't this be an error?
            return this;

        var builder = ToBuilder();
        var currentFlowCapabilities = builder.CapabilityFor(oldValue);
        // This is the new version of the old set, not the set with the new value
        var newSet = oldSet.Replace(oldValue, to.From);
        builder.UpdateSet(oldSet, newSet);
        builder.ApplyRestrictions(newSet);

        var newValue = ResultValue.Create(intoValueId);
        builder.TrackFlowCapability(newValue, currentFlowCapabilities.With(to.Capability));
        builder.AddSet(SharingSet.DeclareConversion(true, newValue, to));
        return builder.ToFlowState();
    }

    public IFlowState DropBindings(IEnumerable<INamedBindingNode> bindings)
    {
        var builder = ToBuilder();
        // TODO what about independent parameters?
        var bindingValues = bindings.Select(BindingValue.CreateTopLevel);
        builder.Drop(bindingValues);
        return builder.ToFlowState();
    }

    public IFlowState DropValue(ValueId valueId)
    {
        var builder = ToBuilder();
        var value = ResultValue.Create(valueId);
        builder.Drop(value);
        return builder.ToFlowState();
    }

    private Builder ToBuilder()
        => new Builder(capabilities.ToBuilder(), sets.ToBuilder(), setFor.ToBuilder());

    private SharingSet? TrySetFor(IValue value) => setFor.GetValueOrDefault(value);

    private readonly struct Builder
    {
        private readonly ImmutableDictionary<ICapabilityValue, FlowCapability>.Builder capabilities;
        private readonly ImmutableHashSet<SharingSet>.Builder sets;
        private readonly ImmutableDictionary<IValue, SharingSet>.Builder setFor;

        public Builder(
            ImmutableDictionary<ICapabilityValue, FlowCapability>.Builder capabilities,
            ImmutableHashSet<SharingSet>.Builder sets,
            ImmutableDictionary<IValue, SharingSet>.Builder setFor)
        {
            this.capabilities = capabilities;
            this.sets = sets;
            this.setFor = setFor;
        }

        public LegacyFlowState ToFlowState()
            => new LegacyFlowState(capabilities.ToImmutable(), sets.ToImmutable(), setFor.ToImmutable());

        public void AddFlowCapabilities(IEnumerable<(BindingValue Value, FlowCapability FlowCapability)> valueCapability)
            => capabilities.AddRange(valueCapability.Select(p => KeyValuePair.Create((ICapabilityValue)p.Value, p.FlowCapability)));

        public FlowCapability SetFlowCapability(ICapabilityValue value, FlowCapability flowCapability)
        {
            // TODO would it be more efficient to check if the flow capability already has the same value?
            capabilities[value] = flowCapability;
            return flowCapability;
        }

        public FlowCapability CapabilityFor(ICapabilityValue value)
        {
            if (!capabilities.TryGetValue(value, out var capability))
                throw new InvalidOperationException($"Flow capability for {value} no longer declared.");
            return capability;
        }

        public FlowCapability? TryCapabilityFor(ICapabilityValue value)
            => capabilities.TryGetValue(value, out var capability) ? capability : (FlowCapability?)null;

        public SharingSet SetFor(IValue value)
        {
            if (!setFor.TryGetValue(value, out var set))
                throw new InvalidOperationException($"Sharing value {value} no longer declared.");

            return set;
        }

        public SharingSet? TrySetFor(IValue value) => setFor.GetValueOrDefault(value);

        public SharingSet NonLentParametersSet()
        {
            if (TrySetFor(ExternalReference.NonLentParameters) is not null and var set) return set;

            set = SharingSet.Declare(false, ExternalReference.NonLentParameters);
            AddSet(set);
            return set;
        }

        public bool Contains(IValue value) => setFor.ContainsKey(value);

        public void UpdateSet(SharingSet oldSet, SharingSet? newSet)
        {
            if (newSet is null)
            {
                RemoveSet(oldSet);
                return;
            }

            sets.Remove(oldSet);
            // Remove items that have been removed from the set
            foreach (IValue value in oldSet.Except(newSet)) setFor.Remove(value);
            sets.Add(newSet);
            // Add or update items that have been added or updated
            foreach (IValue value in newSet) setFor[value] = newSet;
        }

        private void RemoveSet(SharingSet set)
        {
            sets.Remove(set);
            foreach (IValue value in set) setFor.Remove(value);
        }

        public void ReplaceSets(IReadOnlyCollection<SharingSet> oldSets, SharingSet newSet)
        {
            sets.ExceptWith(oldSets);
            // Remove items that have been removed from the sets
            foreach (IValue value in oldSets.SelectMany(Functions.Identity).Except(newSet)) setFor.Remove(value);
            sets.Add(newSet);
            // Add or update items that have been added or updated
            foreach (IValue value in newSet) setFor[value] = newSet;
        }

        public void AddSet(SharingSet set)
        {
            sets.Add(set);
            foreach (IValue value in set) setFor.Add(value, set);
        }

        public void AddSets(IReadOnlyCollection<SharingSet> sets)
        {
            this.sets.UnionWith(sets);
            setFor.AddRange(sets.SelectMany(s => s.Select(v => KeyValuePair.Create(v, s))));
        }

        public void TrackFlowCapability(ICapabilityValue value, FlowCapability flowCapability)
            => capabilities.Add(value, flowCapability);

        public void Union(IEnumerable<SharingSet> setsToUnion)
        {
            var distinctSets = setsToUnion.Distinct().ToList();
            var union = SharingSet.Union(distinctSets);

            sets.ExceptWith(distinctSets);
            sets.Add(union);

            // Update all items in the union to point to the new set
            foreach (IValue value in union) setFor[value] = union;
        }

        public void ApplyRestrictions(SharingSet? set)
        {
            if (set is null) return;
            var restrictions = set.Restrictions;
            if (restrictions == CapabilityRestrictions.None) return;
            foreach (var value in set.OfType<ICapabilityValue>())
                // TODO it might be a bug if the value is not in the capabilities dictionary
                if (capabilities.TryGetValue(value, out var flowCapability))
                    capabilities[value] = flowCapability.WithRestrictions(restrictions);
        }

        public void Drop(IEnumerable<IValue> values)
        {
            List<IConversion>? conversionsRemoved = null;
            foreach (var group in values.GroupBy(TrySetFor))
            {
                var set = group.Key;
                if (set is null) continue;
                var newSet = set.Drop(group);
                UpdateSet(set, newSet);
                foreach (var capabilityValue in group.OfType<ICapabilityValue>())
                    RemoveCapability(capabilityValue);
                if (newSet is null)
                    (conversionsRemoved ??= []).AddRange(set.Conversions.OfType<TempConversionTo>()
                                                            .Select(c => c.From));
            }
            if (conversionsRemoved is not null && conversionsRemoved.Any())
                DropConversions(conversionsRemoved);
        }

        private void DropConversions(IEnumerable<IConversion> conversionsRemoved)
        {
            var setForConversion = GetSetsForConversions();
            foreach (var conversions in conversionsRemoved.GroupBy(c => setForConversion[c]))
            {
                var oldSet = conversions.Key;
                sets.Remove(oldSet);
                var newSet = oldSet.Drop(conversions);
                sets.Add(newSet);
                LiftRemovedRestrictions(newSet);
            }
        }

        public void Drop(IValue value)
        {
            if (TrySetFor(value) is not SharingSet set) return;
            var newSet = set.Drop(value);
            UpdateSet(set, newSet);
            RemoveCapability(value);
            if (newSet is not null)
                return;

            var conversionsRemoved = set.Conversions.OfType<TempConversionTo>().Select(c => c.From).ToList();
            if (conversionsRemoved.Any())
                DropConversions(conversionsRemoved);
        }

        public void RemoveCapability(IValue value)
        {
            if (value is ICapabilityValue capabilityValue)
                capabilities.Remove(capabilityValue);
        }

        public void RemoveCapability(ICapabilityValue value)
            => capabilities.Remove(value);

        private void LiftRemovedRestrictions(SharingSet set)
            // Don't apply read/write restrictions since they have already been applied
            => SetRestrictions(set, applyReadWriteRestriction: false);

        private void SetRestrictions(SharingSet sharingSet, bool applyReadWriteRestriction)
        {
            var restrictions = sharingSet.Restrictions;
            if (!applyReadWriteRestriction && restrictions == CapabilityRestrictions.ReadWrite) return;
            foreach (var value in sharingSet.OfType<ICapabilityValue>())
                capabilities[value] = capabilities[value].WithRestrictions(restrictions);
        }

        private Dictionary<IConversion, SharingSet> GetSetsForConversions()
            => sets.SelectMany(s => s.Conversions.Select(c => (c, s))).ToDictionary();
    }

    #region Equality
    public bool Equals(LegacyFlowState? other)
    {
        if (other is null)
            return false;
        if (ReferenceEquals(this, other))
            return true;

        // Check collection sizes first to avoid iterating over the collections
        if (capabilities.Count != other.capabilities.Count
            || sets.Count != other.sets.Count
            // This checks that the total number of elements in the sets are the same
            || setFor.Count != other.setFor.Count)
            return false;

        // setFor does not need to be compared because it is implied by the sets
        return sets.SetEquals(other.sets)
               // Already checked that the sizes are the same, so it suffices to check that all the
               // entries in one are in the other.
               && capabilities.All(p => other.capabilities.TryGetValue(p.Key, out var value) && p.Value.Equals(value));
    }

    public bool Equals(IFlowState? obj)
        => ReferenceEquals(this, obj) || obj is LegacyFlowState other && Equals(other);

    public override bool Equals(object? obj)
        => ReferenceEquals(this, obj) || obj is LegacyFlowState other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(capabilities.Count, sets.Count, setFor.Count);
    #endregion
}
