using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow;

/// <summary>
/// Tracks the original and modified capabilities for a reference.
/// </summary>
/// <remarks>A flow capability is always applied to the outermost capability of a type. However, if
/// a type has independent type parameters, then this may apply to the type parameters as well. This
/// struct encapsulates the flow capabilities applied to both.</remarks>
[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
public readonly struct FlowCapabilities
{
    public FlowCapability Outer { get; }
    public FixedDictionary<TypeArgumentIndex, FlowCapability> TypeParameters { get; }

    public FlowCapabilities(CapabilityType type)
    {
        Outer = type.Capability;
        TypeParameters = FlowCapabilitiesForTypeParameters(type);
    }

    private static FixedDictionary<TypeArgumentIndex, FlowCapability> FlowCapabilitiesForTypeParameters(
        CapabilityType type)
    {
        if (!type.HasIndependentTypeArguments)
            return FixedDictionary<TypeArgumentIndex, FlowCapability>.Empty;

        var index = new Stack<int>();
        return FlowCapabilitiesForTypeParameters(type, index).ToFixedDictionary();
    }

    private static IEnumerable<(TypeArgumentIndex, FlowCapability)> FlowCapabilitiesForTypeParameters(
        CapabilityType type,
        Stack<int> parentIndex)
        => FlowCapabilitiesForImmediateTypeParameters(type, parentIndex)
            .Concat(FlowCapabilitiesForChildTypeParameters(type, parentIndex));


    private static IEnumerable<(TypeArgumentIndex, FlowCapability)> FlowCapabilitiesForImmediateTypeParameters(
        CapabilityType type, Stack<int> parentIndex)
    {

        var joinedTypeArguments = type.BareType.GenericParameterArguments.Enumerate();
        var independentTypeArguments = joinedTypeArguments.Where((p, _) => p.ParameterHasIndependence);
        var independentReferenceTypeArguments = independentTypeArguments.Select((p, i) => (i, p.Argument as ReferenceType))
                                                                        .Where((int _, ReferenceType? arg) => arg is not null);

        foreach (var (i, arg) in independentReferenceTypeArguments)
        {
            parentIndex.Push(i);
            yield return (new TypeArgumentIndex(parentIndex.ToFixedList()), arg!.Capability);
            parentIndex.Pop();
        }
    }

    private static IEnumerable<(TypeArgumentIndex, FlowCapability)> FlowCapabilitiesForChildTypeParameters(
        CapabilityType type,
        Stack<int> parentIndex)
    {
        var flowCapabilities = Enumerable.Empty<(TypeArgumentIndex, FlowCapability)>();
        foreach (var (arg, i) in type.TypeArguments.Enumerate().Where(tuple => tuple.Value is ReferenceType))
        {
            parentIndex.Push(i);
            flowCapabilities = flowCapabilities.Concat(FlowCapabilitiesForTypeParameters((ReferenceType)arg, parentIndex));
            parentIndex.Pop();
        }

        return flowCapabilities;
    }

    public FlowCapabilities(
        FlowCapability outerCapability,
        FixedDictionary<TypeArgumentIndex, FlowCapability>? typeParametersCapabilities = null)
    {
        Outer = outerCapability;
        TypeParameters = typeParametersCapabilities ?? FixedDictionary<TypeArgumentIndex, FlowCapability>.Empty;
    }

    public FlowCapabilities AfterMove() => new(Outer.AfterMove(), TypeParameters);
    public FlowCapabilities AfterFreeze() => new(Outer.AfterFreeze(), TypeParameters);
    public FlowCapabilities WhenAliased() => new(Outer.WhenAliased(), TypeParameters);
    public FlowCapabilities OfAlias() => new(Outer.OfAlias(), TypeParameters);
    public FlowCapabilities With(Capability capability) => new(Outer.With(capability), TypeParameters);
    public FlowCapabilities WithRestrictions(CapabilityRestrictions restrictions)
        => new(Outer.WithRestrictions(restrictions), TypeParameters);
    public FlowCapabilities Restrict(CapabilityType capabilityType)
    {
        var newOuter = Outer.With(capabilityType.Capability);
        if (TypeParameters.IsEmpty)
            return new(newOuter, TypeParameters);

        return new(newOuter, RestrictTypeParameters(capabilityType).ToFixedDictionary());
    }

    private IEnumerable<(TypeArgumentIndex, FlowCapability)> RestrictTypeParameters(CapabilityType capabilityType)
    {
        foreach (var (index, capability) in TypeParameters)
        {
            var argument = capabilityType.ArgumentAt(index);
            yield return (index, capability.With(argument.Capability));
        }
    }

    public override string ToString()
    {
        if (TypeParameters.IsEmpty)
            return Outer.ToString();

        var typeParameters = string.Join(", ", TypeParameters.Select(kvp => $"{kvp.Key}: {kvp.Value}"));
        return $"{Outer} [{typeParameters}]";
    }
}
