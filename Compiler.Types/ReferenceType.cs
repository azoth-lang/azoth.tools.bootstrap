using System;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

[Closed(
    typeof(ObjectType),
    typeof(AnyType))]
public abstract class ReferenceType : DataType
{
    public ReferenceCapability Capability { get; }
    public bool IsReadOnlyReference => Capability == ReferenceCapability.ReadOnly;
    public bool IsWritableReference => Capability.AllowsWrite;
    public bool IsMovableReference => Capability.AllowsMovable;
    public bool IsConstReference => Capability == ReferenceCapability.Constant;
    public bool IsIsolatedReference => Capability == ReferenceCapability.Isolated;
    public bool IsIdentityReference => Capability == ReferenceCapability.Identity;

    public BareReferenceType BareType { get; }

    public override TypeSemantics Semantics => TypeSemantics.Reference;

    private protected ReferenceType(ReferenceCapability capability, BareReferenceType bareType)
    {
        Capability = capability;
        BareType = bareType;
    }

    public ReferenceType ToMutable()
    {
        if (!Capability.AllowsWrite)
            throw new InvalidOperationException($"Can't convert '{Capability.ToILString()}' to mutable because it does not allow write.");
        return To(ReferenceCapability.Mutable);
    }

    public override ReferenceType WithoutWrite() => To(Capability.WithoutWrite());

    /// <summary>
    /// Return the same type except with the given reference capability
    /// </summary>
    public abstract ReferenceType To(ReferenceCapability referenceCapability);
}
