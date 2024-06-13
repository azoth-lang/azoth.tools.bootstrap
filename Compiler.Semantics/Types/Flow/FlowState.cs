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

        ExternalReference? externalReference = null;

        var capability = capabilityType.Capability;
        // These capabilities don't have to worry about external references
        if (capability != Capability.Isolated && capability != Capability.TemporarilyIsolated
            && capability != Capability.Constant && capability != Capability.Identity)
            externalReference = isLent
                ? ExternalReference.CreateLentParameter(parameter.ValueId.Value)
                : ExternalReference.NonLentParameters;

        // TODO is this correct? Went lent, doesn't each one need its own external reference?
        var newSharingSets = bindingValuePairs.Select(p => new SharingSet(isLent, p.Key, externalReference)).ToFixedList();
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
            var set = builder.SharingSet(bindingValue);

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
        var existingSets = argumentResults.Where(a => !a.IsLent).Select(a => builder.SharingSet(a.Value)).ToList();
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
        if (other is null) return this;
        throw new NotImplementedException();
    }

    public FlowState Combine(ValueId left, ValueId? right, ValueId intoValueId)
    {
        var resultValues = right.YieldValue().Prepend(left).Select(ResultValue.Create).ToFixedList();

        var builder = ToBuilder();
        var existingSets = resultValues.Select(builder.TrySharingSet).WhereNotNull().ToList();
        var unionIsLent = existingSets.Any(s => s.IsLent);
        var values = existingSets.SelectMany(Functions.Identity).Append(ResultValue.Create(intoValueId))
                                 .Except(resultValues);
        var combinedSet = new SharingSet(unionIsLent, values);

        builder.ReplaceSets(existingSets, combinedSet);

        return builder.ToFlowState();
    }

    private Builder ToBuilder()
        => new Builder(capabilities.ToBuilder(), sets.ToBuilder(), setFor.ToBuilder());

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

        public SharingSet SharingSet(IValue value)
        {
            if (!setFor.TryGetValue(value, out var set))
                throw new InvalidOperationException($"Sharing value {value} no longer declared.");

            return set;
        }

        public SharingSet? TrySharingSet(IValue value)
            => setFor.GetValueOrDefault(value);

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

        private void AddSet(SharingSet set)
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
    }
}
