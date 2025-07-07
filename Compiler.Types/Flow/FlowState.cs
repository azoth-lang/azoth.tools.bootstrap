using System.Collections.Immutable;
using System.Runtime.InteropServices;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;
using Azoth.Tools.Bootstrap.Compiler.Types.Flow.Sharing;
using Azoth.Tools.Bootstrap.Framework;
using Azoth.Tools.Bootstrap.Framework.Collections;
using DotNet.Collections.Generic;
using InlineMethod;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Flow;

internal sealed class FlowState : IFlowState
{
    public static FlowState Empty { get; } = new FlowState();

    /// <summary>
    /// A mapping of <see cref="ValueId"/>s to the values in scope. This includes both tracked and
    /// <i>untracked</i> values.
    /// </summary>
    private readonly ImmutableDictionary<ValueId, IFixedSet<ICapabilityValue>> valuesForId;

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

    public bool IsEmpty => valuesForId.IsEmpty && values.IsEmpty && untrackedValues.IsEmpty;

    private FlowState()
    {
        valuesForId = ImmutableDictionary<ValueId, IFixedSet<ICapabilityValue>>.Empty;
        values = ImmutableDisjointHashSets<IValue, FlowCapability, SharingSetState>.Empty;
        untrackedValues = [];
    }

    private FlowState(
        ImmutableDictionary<ValueId, IFixedSet<ICapabilityValue>> valuesForId,
        IImmutableDisjointSets<IValue, FlowCapability, SharingSetState> values,
        ImmutableHashSet<IValue> untrackedValues)
    {
        this.valuesForId = valuesForId;
        this.values = values;
        this.untrackedValues = untrackedValues;
    }

    #region Value Lookup
    private IEnumerable<ICapabilityValue> TrackedValues(ValueId valueId)
        => valuesForId[valueId].Where(v => !untrackedValues.Contains(v));

    private IEnumerable<ICapabilityValue> TrackedValues(IFixedList<ValueId> valueIds)
        => valueIds.SelectMany(id => valuesForId[id]).Where(v => !untrackedValues.Contains(v));

    private IEnumerable<ICapabilityValue> TrackedValues(IEnumerable<ArgumentValueId> argumentValueIds)
        => argumentValueIds.SelectMany(a => valuesForId[a.ValueId]).Where(v => !untrackedValues.Contains(v));

    /// <summary>
    /// Build a mapping from new <see cref="CapabilityValue"/>s to the old <see cref="ICapabilityValue"/>s.
    /// </summary>
    /// <remarks>Since this is an alias, all old values can be mapped directly to new values without
    /// concern for the types involved.</remarks>
    private IReadOnlyDictionary<CapabilityValue, ICapabilityValue> AliasValueMapping(ValueId oldValueId, ValueId newValueId)
        => valuesForId[oldValueId].ToDictionary(v => CapabilityValue.Create(newValueId, v.Index));

    /// <summary>
    /// Build a mapping from new <see cref="CapabilityValue"/>s to the old <see cref="ICapabilityValue"/>s.
    /// Return that mapping as a <see cref="MultiMapHashSet{TKey,TValue}"/> even though it has only
    /// one value per key so that it can be the starting point for mappings that have multiple values.
    /// </summary>
    /// <remarks>Since this is an alias, all old values can be mapped directly to new values without
    /// concern for the types involved.</remarks>
    private MultiMapHashSet<CapabilityValue, ICapabilityValue> AliasValueMultiMapping(ValueId oldValueId, ValueId newValueId)
    {
        var valueMap = new MultiMapHashSet<CapabilityValue, ICapabilityValue>();
        foreach (var oldValue in valuesForId[oldValueId])
        {
            var newValue = CapabilityValue.Create(newValueId, oldValue.Index);
            valueMap.TryToAddMapping(newValue, oldValue);
        }
        return valueMap;
    }

    /// <summary>
    /// Create a mapping of new <see cref="CapabilityValue"/>s to the old <see cref="ICapabilityValue"/>s
    /// that handles upcasting/supertype relationships.
    /// </summary>
    private MultiMapHashSet<CapabilityValue, ICapabilityValue> SupertypeValueMapping(
        ValueId oldValueId,
        CapabilityType oldValueType,
        ValueId newValueId,
        CapabilityType newValueType)
    {
        Requires.That(oldValueType.IsSubtypeOf(newValueType), nameof(newValueType),
            $"Must be a supertype of {nameof(oldValueType)}");

        var aliasValueMap = AliasValueMultiMapping(oldValueId, newValueId);
        if (oldValueType.Equals(newValueType)) return aliasValueMap;

        var declaredSupertypes = oldValueType.PlainType.Supertypes
                                             .Where(s => s.TypeConstructor.Equals(newValueType.TypeConstructor))
                                             .ToFixedList();

        if (declaredSupertypes.Count > 1)
            throw new NotImplementedException("Type is a subtype of the new type in multiple ways");

        var declaredSupertype = declaredSupertypes.TrySingle()
            ?? throw new ArgumentException($"The type `{newValueType.ToILString()}` is not a supertype of `{oldValueType.ToILString()}`.");

        foreach (var (toValue, fromValue) in aliasValueMap)
        {
            if (toValue.Index != CapabilityIndex.TopLevel)
                throw new NotImplementedException(
                    $"Mapping actual upcast from `{oldValueType.ToILString()}` to `{newValueType.ToILString()}`.");
        }

        return aliasValueMap;
    }
    #endregion

    public IFlowState DeclareParameter(bool isLent, ValueId id, IMaybeType type)
    {
        var builder = ToBuilder();
        var bindingValuePairs = BindingValue.ForType(id, type);
        builder.AddValueId(id, bindingValuePairs.Keys);
        foreach (var (value, capability) in bindingValuePairs)
        {
            var sharingIsTracked = capability.SharingIsTracked();
            // These capabilities don't have to worry about external references
            var needsExternalReference = capability != Capability.Isolated
                                         && capability != Capability.TemporarilyIsolated;

            if (!sharingIsTracked)
                builder.AddUntracked(value);
            else if (!needsExternalReference)
                builder.AddSet(isLent, value, capability);
            else if (isLent)
                // Lent parameters each have their own external reference
                builder.AddSet(isLent, value, capability, ExternalReference.CreateLentParameter(value));
            else
                // Non-lent parameters share the same external reference
                builder.AddToNonLentParameterSet(value, capability);
        }

        return builder.ToImmutable();
    }

    public IFlowState DeclareVariable(ValueId id, IMaybeNonVoidType type, ValueId? initializerId, bool dropInitializer = true)
    {
        var builder = ToBuilder();
        var bindingValuePairs = BindingValue.ForType(id, type);
        builder.AddValueId(id, bindingValuePairs.Keys);
        foreach (var (value, capability) in bindingValuePairs)
        {
            if (!capability.SharingIsTracked())
            {
                builder.AddUntracked(value);
                continue;
            }

            if (initializerId is ValueId valueId)
            {
                // TODO this isn't correct. If the value is upcast there may not be a direct correspondence
                var initializerValue = CapabilityValue.Create(valueId, value.Index);
                if (!untrackedValues.Contains(initializerValue))
                {
                    var initializerSet = builder.TrySetFor(initializerValue)
                                         ?? throw new InvalidOperationException("Value should be in a set");
                    builder.AddToSet(initializerSet, value, capability);
                    continue;
                }
            }

            // Otherwise, add the value to a new set
            builder.AddSet(false, value, capability);
        }

        if (dropInitializer && initializerId is ValueId initializerValueId)
            builder.Remove(initializerValueId);

        return builder.ToImmutable();
    }

    public IFlowState Constant(ValueId valueId) => AddUntracked(valueId);

    private IFlowState AddUntracked(ValueId valueId)
    {
        var builder = ToBuilder();
        var value = CapabilityValue.CreateTopLevel(valueId);
        builder.AddValueId(valueId, value);
        builder.AddUntracked(value);
        return builder.ToImmutable();
    }

    public IFlowState Alias(ValueId? id, ValueId aliasId)
    {
        var builder = ToBuilder();

        if (id is not { } valueId)
            // An alias of an unknown thing cannot be tracked. It will have an unknown type.
            return AddUntracked(aliasId);

        // An alias has the same type (modulo aliasing) as the original value and as such the same
        // capability values. Thus, we can simply map the original values to the alias values.

        var valueMap = AliasValueMapping(valueId, aliasId);
        foreach (var (aliasValue, bindingValue) in valueMap)
        {
            // Aliases match the tracking of the original value
            if (untrackedValues.Contains(bindingValue))
                builder.AddUntracked(aliasValue);
            else
            {
                // Add the alias to the same set as the original value
                var set = builder.TrySetFor(bindingValue) ?? throw new InvalidOperationException();
                var aliasCapability = builder[bindingValue].OfAlias();
                builder.AddToSet(set, aliasValue, aliasCapability);

                // Update the capability of the original value to reflect that an alias exists
                builder.UpdateCapability(bindingValue, c => c.WhenAliased());
            }
        }
        builder.AddValueId(aliasId, valueMap.Keys);

        return builder.ToImmutable();
    }

    public IMaybeType Type(ValueId id, IMaybeType declaredType)
        => Type(id, declaredType, Functions.Identity);

    public IMaybeType AliasType(ValueId id, IMaybeType declaredType)
        => Type(id, declaredType, c => c.OfAlias());

    private IMaybeType Type(ValueId id, IMaybeType declaredType, Func<Capability, Capability> transform)
    {
        if (!declaredType.SharingIsTracked())
            // Other types don't have capabilities and don't need to be tracked
            return declaredType;

        var bindingValue = BindingValue.CreateTopLevel(id);
        var current = values[bindingValue].Current;
        // TODO what about independent parameters?
        return ((CapabilityType)declaredType).With(transform(current));
    }

    public bool IsIsolated(ValueId valueId)
        // TODO what about independent parameters?
        => IsIsolated(values.Sets.TrySetFor(CapabilityValue.CreateTopLevel(valueId)));

    private static bool IsIsolated(IImmutableDisjointSet<IValue, SharingSetState>? set)
        => set?.Count == 1;


    public bool IsIsolatedExceptFor(ValueId id, ValueId exceptForId)
        => IsIsolatedExceptFor(values.Sets.TrySetFor(BindingValue.CreateTopLevel(id)), CapabilityValue.CreateTopLevel(exceptForId));

    private static bool IsIsolatedExceptFor(IImmutableDisjointSet<IValue, SharingSetState>? set, IValue value)
        => set?.Count <= 2 && set.Except(value).Count() == 1;

    public bool CanFreezeExceptFor(ValueId id, ValueId exceptForId)
    {
        // TODO what about independent parameters?
        var bindingValue = BindingValue.CreateTopLevel(id);
        var set = values.Sets.TrySetFor(bindingValue);
        if (set is null) return false;
        if (IsIsolated(set)) return true;

        var exceptValue = CapabilityValue.CreateTopLevel(exceptForId);
        foreach (var otherValue in set.Except(bindingValue).Except(exceptValue))
        {
            if (otherValue is ICapabilityValue capabilityValue)
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

    public bool CanFreeze(ValueId valueId)
    {
        // TODO what about independent parameters?
        var value = CapabilityValue.CreateTopLevel(valueId);
        var set = values.Sets.TrySetFor(value);
        if (set is null) return false;
        if (IsIsolated(set)) return true;

        foreach (var otherValue in set.Except(value))
        {
            if (otherValue is ICapabilityValue capabilityValue)
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

    public bool IsLent(ValueId valueId)
        => TrackedValues(valueId).Select(value => values.Sets[value]).Any(set => set.Data.IsLent);

    public IFlowState CombineArguments(IEnumerable<ArgumentValueId> arguments, ValueId returnValueId, IMaybeType returnType)
    {
        var argumentsList = arguments.ToFixedList();

        var builder = ToBuilder();
        // TODO properly handle the correlation between the arguments and the return value
        // Union sets for all non-lent arguments
        int? set = builder.Union(TrackedValues(argumentsList.Where(a => !a.IsLent)));

        // Add the return value(s) to the unioned set
        var capabilityValuePairs = CapabilityValue.ForType(returnValueId, returnType);
        foreach (var (returnValue, capability) in capabilityValuePairs)
        {
            if (capability.SharingIsTracked())
                builder.AddToSet(set, false, returnValue, capability);
            else
                builder.AddUntracked(returnValue);
        }
        builder.AddValueId(returnValueId, capabilityValuePairs.Keys);

        // Now remove all the arguments
        builder.Remove(argumentsList.Select(a => a.ValueId));

        return builder.ToImmutable();
    }

    /// <summary>
    /// Determine which value ids cannot be combined due to lent sharing sets.
    /// </summary>
    /// <remarks>Two lent sharing sets cannot be combined. In addition, a lent sharing set cannot be
    /// combined with a non-lent, non-isolated sharing set. Isolated sharing sets are not a problem
    /// because they just become part of the lent set and nothing else references them.</remarks>
    public IEnumerable<ValueId> CombineArgumentsDisallowedDueToLent(IEnumerable<ArgumentValueId> arguments)
    {
        var valueIds = arguments.Where(a => !a.IsLent).Select(a => a.ValueId).ToFixedList();
        if (valueIds.Count <= 1)
            // When there is only a single argument, even if it is in a lent set, it isn't being unioned with anything else
            return [];

        return CombineDisallowedDueToLent(valueIds);
    }

    /// <summary>
    /// Determine which value ids cannot be combined due to lent sharing sets.
    /// </summary>
    /// <remarks>Two lent sharing sets cannot be combined. In addition, a lent sharing set cannot be
    /// combined with a non-lent, non-isolated sharing set. Isolated sharing sets are not a problem
    /// because they just become part of the lent set and nothing else references them.</remarks>
    private IEnumerable<ValueId> CombineDisallowedDueToLent(IEnumerable<ValueId> valueIds)
    {
        bool nonIsolatedBeingCombined = false;
        var lentValueIds = new HashSet<ValueId>();
        foreach (var valueId in valueIds)
            foreach (var set in TrackedValues(valueId).Select(v => values.Sets[v]))
            {
                if (set.Data.IsLent)
                    lentValueIds.Add(valueId);
                else if (!IsIsolated(set))
                    nonIsolatedBeingCombined = true;
            }
        if (nonIsolatedBeingCombined || lentValueIds.Count > 1)
            return lentValueIds;
        return [];
    }

    public IFlowState AccessField(
        ValueId contextId,
        IMaybeType contextType,
        BareTypeConstructor declaringTypeConstructor,
        ValueId id,
        IMaybeNonVoidType bindingType,
        IMaybeType memberType)
    {
        var builder = ToBuilder();
        var newValueCapabilities = CapabilityValue.ForType(id, memberType);
        var valueMap = AccessFieldValueMapping(contextId, contextType, declaringTypeConstructor,
            bindingType, id, newValueCapabilities.Keys);
        foreach (var (newValue, capability) in newValueCapabilities)
        {
            if (capability.SharingIsTracked())
            {
                var set = builder.Union(valueMap[newValue]);
                // TODO is `isLent: false` correct?
                builder.AddToSet(set, isLent: false, newValue, capability);
            }
            else
                builder.AddUntracked(newValue);
        }

        builder.AddValueId(id, newValueCapabilities.Keys);
        builder.Remove(contextId);

        return builder.ToImmutable();
    }

    /// <summary>
    /// Construct a mapping from the <see cref="CapabilityValue"/>s of the member being accessed to
    /// the <see cref="ICapabilityValue"/> of the context it is being accessed in.
    /// </summary>
    private MultiMapHashSet<CapabilityValue, ICapabilityValue> AccessFieldValueMapping(
        ValueId contextValueId,
        IMaybeType contextType,
        BareTypeConstructor declaringTypeConstructor,
        IMaybeType bindingType,
        ValueId valueId,
        IEnumerable<CapabilityValue> newValues)
    {
        // TODO this switch mostly duplicates logic inside of ICapabilityValue.ForType
        switch (contextType)
        {
            case OptionalType t:
                // Traverse into optional types
                return AccessFieldValueMapping(contextValueId, t.Referent, declaringTypeConstructor,
                    bindingType, valueId, newValues);
            case SelfViewpointType t:
                // TODO if capture is true, shouldn't the viewpoint somehow affect the flow capability?
                return AccessFieldValueMapping(contextValueId, t.Referent, declaringTypeConstructor,
                    bindingType, valueId, newValues);
            case CapabilitySetSelfType t:
                // TODO I really don't know that this is right
                var newContextType = t.BareType.ContainingType!.With(t.CapabilitySet.UpperBound);
                return AccessFieldValueMapping(contextValueId, newContextType,
                    declaringTypeConstructor, bindingType, valueId, newValues);
        }

        // TODO handle other types?
        var contextCapabilityType = (CapabilityType)contextType;
        var effectiveContextType = contextCapabilityType.UpcastTo(declaringTypeConstructor);
        var valueMap = SupertypeValueMapping(contextValueId, contextCapabilityType, valueId, effectiveContextType);

        var rootValue = CapabilityValue.CreateTopLevel(valueId);
        var mapping = new MultiMapHashSet<CapabilityValue, ICapabilityValue>();
        foreach (var newValue in newValues)
        {
            var typeAtIndex = bindingType.TypeAt(newValue.Index);
            // If the value corresponds to an independent parameter of the containing class, then
            // map to corresponding in independent value. Otherwise, map to the root value.
            CapabilityValue mapFromValue;
            if (typeAtIndex is GenericParameterType { Parameter.HasIndependence: true } t)
            {
                var parameterIndex = declaringTypeConstructor.Parameters.IndexOf(t.Parameter)
                    // Types nested in a type with independent parameters are not implemented
                    ?? throw new NotImplementedException("Independent parameter not from containing type.");
                // Find the corresponding value in the context
                mapFromValue = CapabilityValue.Create(valueId, newValue.Index.Append(parameterIndex));
            }
            else
                mapFromValue = rootValue;

            mapping.Add(newValue, valueMap[mapFromValue]);
        }

        return mapping;
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
        foreach (var (valueId, values) in otherFlowState.valuesForId)
            builder.AddOrMergeValueId(valueId, values);

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

    public IFlowState Transform(ValueId? valueId, ValueId toValueId, IMaybeType withType)
    {
        // After a return statement the state is empty and there is nothing to do
        if (IsEmpty || valueId is not ValueId fromValueId) return this;

        var builder = ToBuilder();
        // TODO map the original capability values to the result capability values
        var valueMap = AliasValueMapping(fromValueId, toValueId);
        foreach (var (toValue, fromValue) in valueMap)
        {
            var typeAtIndex = withType.TypeAt(fromValue.Index);
            // TODO if the original value was untracked, does that mean the result should be untracked?
            if (typeAtIndex.SharingIsTracked())
            {
                FlowCapability flowCapability = default;
                if (typeAtIndex is CapabilityType withCapabilityType)
                    flowCapability = builder[fromValue].With(withCapabilityType.Capability);
                var set = builder.TrySetFor(fromValue) ?? throw new InvalidOperationException();
                builder.AddToSet(set, toValue, flowCapability);
            }
            else
                builder.AddUntracked(toValue);
        }
        builder.AddValueId(toValueId, valueMap.Keys);
        builder.Remove(fromValueId);
        return builder.ToImmutable();
    }

    public IFlowState Combine(ValueId? left, ValueId? right, ValueId intoValueId)
    {
        var valueIds = new[] { left, right }.WhereNotNull().ToFixedList();
        if (valueIds.IsEmpty) return this;

        var builder = ToBuilder();
        int? set = builder.Union(TrackedValues(valueIds));

        // TODO properly handle or restrict independent parameters
        // Add the return value to the unioned set
        // TODO what is the correct flow capability for the result?
        var capabilityValue = CapabilityValue.CreateTopLevel(intoValueId);
        builder.AddToSet(set, false, capabilityValue, default);
        builder.AddValueId(intoValueId, capabilityValue);

        // Now remove all the inputs
        builder.Remove(valueIds);

        return builder.ToImmutable();
    }

    public IEnumerable<ValueId> CombineDisallowedDueToLent(ValueId? left, ValueId? right)
    {
        if (left is null || right is null)
            // When there is only a single argument, even if it is in a lent set, it isn't being unioned with anything else
            return [];

        return CombineDisallowedDueToLent([left.Value, right.Value]);
    }

    public IFlowState FreezeVariable(ValueId bindingId, IMaybeType bindingType, ValueId id, ValueId intoValueId)
        => Freeze(BindingValue.ForType(bindingId, bindingType).Keys, id, intoValueId);

    public IFlowState FreezeValue(ValueId id, ValueId intoValueId)
        => Freeze([], id, intoValueId);

    private FlowState Freeze(IEnumerable<BindingValue> bindingValues, ValueId id, ValueId intoValueId)
    {
        var builder = ToBuilder();
        foreach (var bindingValue in bindingValues)
            builder.UpdateCapability(bindingValue, c => c.AfterFreeze());

        var valueMap = AliasValueMapping(id, intoValueId);
        foreach (var (newValue, oldValue) in valueMap)
            // New value matches the tracking of the old value
            if (untrackedValues.Contains(oldValue))
                builder.AddUntracked(newValue);
            else
            {
                // If the value could reference `temp const` data, then it needs to be tracked. (However,
                // that could be detected by looking at whether the set is lent or not, correct?)
                var set = builder.TrySetFor(oldValue) ?? throw new InvalidOperationException();
                builder.AddToSet(set, newValue, default); // TODO what is the correct flow capability for the result?}
            }
        builder.AddValueId(intoValueId, valueMap.Keys);
        builder.Remove(id);

        return builder.ToImmutable();
    }

    public IFlowState MoveVariable(ValueId bindingId, IMaybeType bindingType, ValueId valueId, ValueId intoValueId)
        => Move(BindingValue.ForType(bindingId, bindingType)
                           .Where(p => p.Value.SharingIsTracked())
                           .Select(p => p.Key), valueId, intoValueId);

    public IFlowState MoveValue(ValueId valueId, ValueId intoValueId)
        => Move([], valueId, intoValueId);

    private FlowState Move(IEnumerable<BindingValue> bindingValues, ValueId valueId, ValueId intoValueId)
    {
        var builder = ToBuilder();
        var valueMap = AliasValueMapping(valueId, intoValueId);
        foreach (var (newValue, oldValue) in valueMap)
        {
            if (builder.TrySetFor(oldValue) is int set)
                builder.AddToSet(set, newValue, default); // TODO what is the correct flow capability for the result?
            else
                builder.AddUntracked(newValue);
        }

        foreach (var bindingValue in bindingValues)
            // TODO these are now `id`, doesn't that mean they no longer need tracked?
            builder.UpdateCapability(bindingValue, c => c.AfterMove());

        builder.AddValueId(intoValueId, valueMap.Keys);
        builder.Remove(valueId);

        return builder.ToImmutable();
    }

    public IFlowState TempFreeze(ValueId valueId, ValueId intoValueId)
        => TemporarilyConvert(valueId, intoValueId, TempConversionTo.Constant(intoValueId));

    public IFlowState TempMove(ValueId valueId, ValueId intoValueId)
        => TemporarilyConvert(valueId, intoValueId, TempConversionTo.Isolated(intoValueId));

    private FlowState TemporarilyConvert(ValueId valueId, ValueId intoValueId, TempConversionTo to)
    {
        var builder = ToBuilder();
        var valueMap = AliasValueMapping(valueId, intoValueId);
        foreach (var (newValue, oldValue) in valueMap)
        {
            if (untrackedValues.Contains(oldValue))
                builder.AddUntracked(newValue);
            else
            {
                var currentFlowCapabilities = builder[oldValue];
                var oldSet = builder.TrySetFor(oldValue) ?? throw new InvalidOperationException();
                builder.UpdateSet(oldSet, state => state.Add(to.From));
                builder.ApplyRestrictions(oldSet);

                var newFlowCapabilities = currentFlowCapabilities.With(to.Capability);
                builder.AddSet(new SharingSetState(true, to), newValue, newFlowCapabilities);
            }
        }

        builder.AddValueId(intoValueId, valueMap.Keys);
        builder.Remove(valueId);

        return builder.ToImmutable();
    }

    public IFlowState DropBindings(IEnumerable<ValueId> bindingIds)
    {
        // return statements etc can lead to an empty state where all bindings have already been dropped
        if (IsEmpty) return this;
        var builder = ToBuilder();
        builder.Remove(bindingIds);
        return builder.ToImmutable();
    }

    public IFlowState DropValue(ValueId valueId)
    {
        var builder = ToBuilder();
        builder.Remove(valueId);
        return builder.ToImmutable();
    }

    public IFlowState DropBindingsForReturn()
    {
        var builder = ToBuilder();
        foreach (var (valueId, values) in valuesForId)
            if (values.Any(v => v.IsVariableOrParameter))
                builder.Remove(valueId);
        return builder.ToImmutable();
    }

    #region Equality
    public bool Equals(FlowState? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        // Check collection sizes first to avoid iterating over the collections
        if (valuesForId.Count != other.valuesForId.Count
            || values.Count != other.values.Count
            || values.Sets.Count != other.values.Sets.Count
            || untrackedValues.Count != other.untrackedValues.Count
            // Check the hash code first since it is cached
            || GetHashCode() != other.GetHashCode())
            return false;

        // Already checked that the sizes are the same, so it suffices to check that all the
        // entries in one are in the other and equal.
        return values.All(p => other.values.TryGetValue(p.Key, out var value) && p.Value.Equals(value))
               && untrackedValues.SetEquals(other.untrackedValues)
               && valuesForId.All(p => other.valuesForId.TryGetValue(p.Key, out var values) && p.Value.Equals(values))
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
        var unorderedHash = new UnorderedHashCode();
        foreach (var value in set)
            unorderedHash.Add(value);
        return HashCode.Combine(unorderedHash, set.Data);
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
        hash.Add(valuesForId.Count);
        hash.Add(values.Count);
        var unorderedHash = new UnorderedHashCode();
        foreach (var set in values.Sets)
            unorderedHash.Add(set.Count);
        hash.Add(unorderedHash);
        unorderedHash = new();
        foreach (var value in untrackedValues)
            unorderedHash.Add(value);
        hash.Add(unorderedHash);
        return hash.ToHashCode();
    }
    #endregion

    private Builder ToBuilder() => new(valuesForId, values, untrackedValues);

    [StructLayout(LayoutKind.Auto)]
    private readonly struct Builder
    {
        private readonly ImmutableDictionary<ValueId, IFixedSet<ICapabilityValue>>.Builder valuesForId;
        private readonly IImmutableDisjointSets<IValue, FlowCapability, SharingSetState>.IBuilder values;
        private readonly ImmutableHashSet<IValue>.Builder untrackedValues;

        public Builder(
            ImmutableDictionary<ValueId, IFixedSet<ICapabilityValue>> valuesForId,
            IImmutableDisjointSets<IValue, FlowCapability, SharingSetState> values,
            ImmutableHashSet<IValue> untrackedValues)
        {
            this.valuesForId = valuesForId.ToBuilder();
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

        /// <remarks>Note that <paramref name="values"/> can be empty for types like <see cref="NeverType"/>.</remarks>
        public void AddValueId(ValueId valueId, IEnumerable<ICapabilityValue> values)
            => valuesForId.Add(valueId, values.ToFixedSet());

        public void AddValueId(ValueId valueId, ICapabilityValue value)
            => valuesForId.Add(valueId, [value]);

        public void AddOrMergeValueId(ValueId valueId, IFixedSet<ICapabilityValue> values)
        {
            if (valuesForId.TryGetValue(valueId, out var existingValues))
                valuesForId[valueId] = existingValues.Union(values);
            else
                valuesForId.Add(valueId, values);
        }

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

        public void Remove(ValueId valueId)
        {
            foreach (var value in valuesForId[valueId])
                Remove(value);
            valuesForId.Remove(valueId);
        }

        private int? Remove(IValue value)
            => untrackedValues.Remove(value) ? null : values.Remove(value);

        public void Remove(IEnumerable<ValueId> valueIds)
        {
            foreach (var valueId in valueIds)
                Remove(valueId);
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
            => new(valuesForId.ToImmutable(), values.ToImmutable(), untrackedValues.ToImmutable());
    }
}
