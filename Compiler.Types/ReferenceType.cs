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
    public bool IsMovableReference => Capability.IsMovable;
    public bool IsConstReference => Capability == ReferenceCapability.Constant;
    public bool IsIsolatedReference => Capability == ReferenceCapability.Isolated;
    public bool IsIdentityReference => Capability == ReferenceCapability.Identity;

    public override TypeSemantics Semantics => TypeSemantics.Reference;

    private protected ReferenceType(ReferenceCapability capability)
    {
        Capability = capability;
    }

    public ReferenceType ToMutable() => To(Capability.ToMutable());

    public override ReferenceType WithoutWrite() => To(Capability.WithoutWrite());

    /// <summary>
    /// Return the same type except with the given reference capability
    /// </summary>
    public abstract ReferenceType To(ReferenceCapability referenceCapability);
}
