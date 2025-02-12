using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

/// <summary>
/// A self type with a capability set (e.g. `readable Self`).
/// </summary>
/// <remarks><para>This is used as the type of the self parameter of methods that use capability
/// sets on the self parameter.</para>
///
/// <para>This is distinct from <see cref="SelfViewpointType"/> even though that combines a
/// capability set with a type because that combines with a full type whereas this combines with a
/// bare `Self` type.</para></remarks>
public sealed class CapabilitySetSelfType : NonVoidType
{
    public CapabilitySet CapabilitySet { [DebuggerStepThrough] get; }

    public BareType BareType { [DebuggerStepThrough] get; }

    public SelfTypeConstructor TypeConstructor { [DebuggerStepThrough] get; }

    public override BarePlainType PlainType => BareType.PlainType;

    internal override GenericParameterTypeReplacements BareTypeReplacements => BareType.TypeReplacements;

    public override bool HasIndependentTypeArguments => false;

    public CapabilitySetSelfType(CapabilitySet capabilitySet, BareType bareType)
    {
        CapabilitySet = capabilitySet;
        BareType = bareType;
        if (bareType.TypeConstructor is not SelfTypeConstructor typeConstructor)
            throw new ArgumentException("Must be on a self type.", nameof(bareType));
        TypeConstructor = typeConstructor;
    }

    #region Equality
    public override bool Equals(IMaybeType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return other is CapabilitySetSelfType otherType
               && CapabilitySet.Equals(otherType.CapabilitySet)
               && BareType.Equals(otherType.BareType);
    }

    public override int GetHashCode() => HashCode.Combine(CapabilitySet, BareType);
    #endregion

    public override string ToSourceCodeString()
        => $"{CapabilitySet.ToSourceCodeString()} {BareType.ToSourceCodeString()}";

    public override string ToILString() => $"{CapabilitySet.ToILString()} {BareType.ToILString()}";
}
