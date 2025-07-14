using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

/// <summary>
/// A generic type as accessed from a capability (e.g. `const |> T`).
/// </summary>
// TODO should `iso |> T` and `temp iso |> T` be disallowed? Those types require upcasting to access
public sealed class CapabilityViewpointType : NonVoidType
{
    public static IMaybeType Create(Capability capability, IMaybeType referent)
        => referent switch
        {
            GenericParameterType t => Create(capability, t),
            // In an error case, don't actually make a capability type.
            _ => referent.AccessedVia(capability),
        };

    public static NonVoidType Create(Capability capability, GenericParameterType referent)
    {
        // TODO is this really a result of the implicit `aliasable` constraint on most type parameters?
        if (capability == Capability.Mutable || capability == Capability.InitMutable) return referent;
        return new CapabilityViewpointType(capability, referent);
    }

    public override NonVoidType? BaseType => null;

    public Capability Capability { [DebuggerStepThrough] get; }

    public GenericParameterType Referent { [DebuggerStepThrough] get; }

    public override GenericParameterPlainType PlainType => Referent.PlainType;

    public override bool HasIndependentTypeArguments => false;

    internal override GenericParameterTypeReplacements BareTypeReplacements => Referent.BareTypeReplacements;

    private CapabilityViewpointType(Capability capability, GenericParameterType referent)
    {
        Requires.That(referent is not { Parameter.HasIndependence: true }, nameof(referent),
            "Must not be an independent generic parameter.");
        Capability = capability;
        Referent = referent;
    }

    #region Equals
    public override bool Equals(IMaybeType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is CapabilityViewpointType otherType
               && Capability == otherType.Capability
               && Referent.Equals(otherType.Referent);
    }

    public override int GetHashCode() => HashCode.Combine(Capability, Referent);
    #endregion

    public override string ToSourceCodeString()
        => $"{Capability.ToSourceCodeString()} |> {Referent.ToSourceCodeString()}";

    public override string ToILString() => $"{Capability.ToILString()} |> {Referent.ToILString()}";
}
