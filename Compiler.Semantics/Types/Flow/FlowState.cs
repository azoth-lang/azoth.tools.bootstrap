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
public sealed class FlowState
{
    public static readonly FlowState Empty = new FlowState();

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

    private FlowState()
    {
        capabilities = ImmutableDictionary<ICapabilityValue, FlowCapability>.Empty;
        sets = ImmutableHashSet<SharingSet>.Empty;
        setFor = ImmutableDictionary<IValue, SharingSet>.Empty;
    }

    private FlowState(
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
    public FlowState Declare(INamedParameterNode parameter)
    {
        var bindingType = parameter.BindingType;
        bool sharingIsTracked = parameter.ParameterType.SharingIsTracked();

        return Declare(parameter, bindingType, sharingIsTracked, parameter.IsLentBinding);
    }

    public FlowState Declare(ISelfParameterNode parameter)
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
        // TODO other types besides CapabilityType might participate in sharing
        if (bindingType.ToUpperBound() is not CapabilityType capabilityType)
            return this;

        var bindingValuePairs = BindingValue.ForType(parameter.ValueId, capabilityType);
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
            var newSharingSets = bindingValuePairs.Select(p => new SharingSet(isLent, p.Key))
                                                  .ToFixedList();
            builder.AddSets(newSharingSets);
        }
        else if (isLent)
        {
            // Lent parameters each have their own external reference
            var newSharingSets = bindingValuePairs
                .Select(p => new SharingSet(isLent, p.Key, ExternalReference.CreateLentParameter((BindingValue)p.Key)))
                .ToFixedList();
            builder.AddSets(newSharingSets);
        }
        else
        {
            // Non-lent parameters share the same external reference
            var newSharingSet = new SharingSet(isLent, bindingValuePairs.Select(p => p.Key));
            builder.AddSet(newSharingSet);
            builder.Union([newSharingSet, builder.NonLentParametersSet()]);
        }

        return builder.ToFlowState();
    }

    public FlowState Declare(INamedBindingNode binding)
    {
        // TODO other types besides CapabilityType might participate in sharing
        if (binding.BindingType is not CapabilityType capabilityType)
            return this;

        var bindingValuePairs = BindingValue.ForType(binding.ValueId, capabilityType);
        var builder = ToBuilder();
        builder.AddFlowCapabilities(bindingValuePairs);

        if (!binding.BindingType.SharingIsTracked())
            return builder.ToFlowState();

        var newSharingSets = bindingValuePairs.Select(p => new SharingSet(false, p.Key))
                                              .ToFixedList();
        builder.AddSets(newSharingSets);

        return builder.ToFlowState();
    }

    /// <summary>
    /// Make <paramref name="valueId"/> an alias to the <paramref name="binding"/>.
    /// </summary>
    /// <remarks>This does not alias any independent parameters of the binding because only an alias
    /// to the top level object has been created. For example, if <c>iso List[iso Foo]</c> is aliased
    /// the list elements are still isolated. Only the list itself has been aliased and is now
    /// <c>mut</c>.</remarks>
    public FlowState Alias(IBindingNode? binding, ValueId valueId)
    {
        if (binding is null || !binding.SharingIsTracked())
            // If the binding isn't tracked, then the alias isn't either
            return this;

        var builder = ToBuilder();
        var bindingValue = BindingValue.TopLevel(binding);
        var flowCapability = builder.SetFlowCapability(bindingValue, capabilities[bindingValue].WhenAliased());

        if (binding.SharingIsTracked(flowCapability))
        {
            var set = builder.SetFor(bindingValue);

            var result = ResultValue.Create(valueId);
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

        var bindingValue = BindingValue.TopLevel(binding);
        var current = capabilities[bindingValue].Current;
        // TODO what about independent parameters?
        return ((CapabilityType)binding.BindingType.ToUpperBound()).With(transform(current));
    }

    /// <summary>
    /// Combine the non-lent values representing the arguments into one sharing set with the return
    /// value id and drop the values for all arguments.
    /// </summary>
    public FlowState CombineArguments(IEnumerable<ArgumentValueId> arguments, ValueId valueId)
    {
        var argumentResults = arguments.Select(a => new ArgumentResultValue(a.IsLent, a.ValueId)).ToFixedList();

        var builder = ToBuilder();
        var existingSets = argumentResults.Where(a => !a.IsLent)
                                          .Select(a => builder.TrySetFor(a.Value))
                                          .WhereNotNull().ToList();
        var unionIsLent = existingSets.Any(s => s.IsLent);
        var values = existingSets.SelectMany(Functions.Identity).Append(ResultValue.Create(valueId))
                                 .Except(argumentResults.Select(a => a.Value));
        var combinedSet = new SharingSet(unionIsLent, values);

        builder.ReplaceSets(existingSets, combinedSet);

        return builder.ToFlowState();
    }

    public FlowState AccessMember(ValueId contextValueId, ValueId valueId, DataType memberType)
    {
        var contextResultValue = ResultValue.Create(contextValueId);
        // If accessing from non-tracked type, then it's not tracked
        // TODO this might be bug prone if the value was accidentally not in the flow state
        if (!setFor.TryGetValue(contextResultValue, out var set))
            return this;

        var builder = ToBuilder();
        SharingSet newSet;
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

    public FlowState Merge(FlowState? other)
    {
        if (other is null
            || ReferenceEquals(this, other)
            || other.IsEmpty)
            return this;
        if (IsEmpty)
            return other;

        var builder = ToBuilder();
        foreach (var otherSet in other.sets)
        {
            var valueLookup = otherSet.ToLookup(builder.Contains);
            if (valueLookup[true].IsEmpty())
            {
                // Whole set is missing, add it to the flow state (safe because it is immutable)
                builder.AddSet(otherSet);
                continue;
            }

            var setsToUnion = valueLookup[true].Select(builder.SetFor);

            var newValues = valueLookup[false].ToImmutableHashSet();
            if (newValues.Any())
            {
                // Add all the values that are not already in the flow state at once to optimize
                var newSet = new SharingSet(otherSet.IsLent, newValues);
                builder.AddSet(newSet);
                setsToUnion = setsToUnion.Append(newSet);
            }

            // Union all the sets together since they are all contain items in the same set in the
            // other flow state. (Doing this as a single operation is more efficient).
            builder.Union(setsToUnion);
        }

        foreach (var (value, flowCapability) in other.capabilities)
        {
            var existingCapability = builder.TryCapabilityFor(value);
            if (existingCapability is null)
                builder.TrackFlowCapability(value, flowCapability);
            else if (existingCapability != flowCapability)
                throw new InvalidOperationException($"Flow capability for {value} changed from {existingCapability} to {flowCapability}.");
        }
        return builder.ToFlowState();
    }

    public FlowState Combine(ValueId left, ValueId? right, ValueId intoValueId)
    {
        var resultValues = right.YieldValue().Prepend(left).Select(ResultValue.Create).ToFixedList();
        var existingSets = resultValues.Select(TrySetFor).WhereNotNull().ToList();
        if (existingSets.IsEmpty())
            return this;

        var builder = ToBuilder();
        var unionIsLent = existingSets.Any(s => s.IsLent);
        var values = existingSets.SelectMany(Functions.Identity).Append(ResultValue.Create(intoValueId))
                                 .Except(resultValues);
        var combinedSet = new SharingSet(unionIsLent, values);

        builder.ReplaceSets(existingSets, combinedSet);

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

        public FlowState ToFlowState()
            => new FlowState(capabilities.ToImmutable(), sets.ToImmutable(), setFor.ToImmutable());

        public void AddFlowCapabilities(IEnumerable<KeyValuePair<ICapabilityValue, FlowCapability>> valueCapability)
            => capabilities.AddRange(valueCapability);

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

        public SharingSet? TrySetFor(IValue value)
            => setFor.GetValueOrDefault(value);

        public SharingSet NonLentParametersSet()
        {
            if (TrySetFor(ExternalReference.NonLentParameters) is not null and var set)
                return set;

            set = new(false, ExternalReference.NonLentParameters);
            AddSet(set);
            return set;
        }

        public bool Contains(IValue value)
            => setFor.ContainsKey(value);

        public void UpdateSet(SharingSet oldSet, SharingSet newSet)
        {
            sets.Remove(oldSet);
            // Remove items that have been removed from the set
            foreach (IValue value in oldSet.Except(newSet))
                setFor.Remove(value);
            sets.Add(newSet);
            // Add or update items that have been added or updated
            foreach (IValue value in newSet)
                setFor[value] = newSet;
        }

        public void ReplaceSets(IReadOnlyCollection<SharingSet> oldSets, SharingSet newSet)
        {
            sets.ExceptWith(oldSets);
            // Remove items that have been removed from the sets
            foreach (IValue value in oldSets.SelectMany(Functions.Identity).Except(newSet))
                setFor.Remove(value);
            sets.Add(newSet);
            // Add or update items that have been added or updated
            foreach (IValue value in newSet)
                setFor[value] = newSet;
        }

        public void AddSet(SharingSet set)
        {
            sets.Add(set);
            foreach (IValue value in set)
                setFor.Add(value, set);
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
            foreach (IValue value in union)
                setFor[value] = union;
        }
    }
}
