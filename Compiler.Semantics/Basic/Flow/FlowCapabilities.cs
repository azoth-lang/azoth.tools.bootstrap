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
    public FixedDictionary<TypeParameterIndex, FlowCapability> TypeParameters { get; }

    public FlowCapabilities(ReferenceType type)
    {
        Outer = type.Capability;
        // TODO properly compute type parameters capabilities
        TypeParameters = FixedDictionary<TypeParameterIndex, FlowCapability>.Empty;
    }

    public FlowCapabilities(
        FlowCapability outerCapability,
        FixedDictionary<TypeParameterIndex, FlowCapability>? typeParametersCapabilities = null)
    {
        Outer = outerCapability;
        TypeParameters = typeParametersCapabilities ?? FixedDictionary<TypeParameterIndex, FlowCapability>.Empty;
    }

    public FlowCapabilities AfterMove() => new(Outer.AfterMove(), TypeParameters);
    public FlowCapabilities AfterFreeze() => new(Outer.AfterFreeze(), TypeParameters);
    public FlowCapabilities WhenAliased() => new(Outer.WhenAliased(), TypeParameters);
    public FlowCapabilities OfAlias() => new(Outer.OfAlias(), TypeParameters);
    public FlowCapabilities With(ReferenceCapability capability) => new(Outer.With(capability), TypeParameters);
    public FlowCapabilities WithRestrictions(CapabilityRestrictions restrictions)
        => new(Outer.WithRestrictions(restrictions), TypeParameters);
    public FlowCapabilities Restrict(ReferenceType referenceType)
        // TODO restrict type parameters
        => new(Outer.With(referenceType.Capability), TypeParameters);

    public override string ToString()
    {
        if (TypeParameters.IsEmpty)
            return Outer.ToString();

        var typeParameters = string.Join(", ", TypeParameters.Select(kvp => $"{kvp.Key}: {kvp.Value}"));
        return $"{Outer} [{typeParameters}]";
    }
}
