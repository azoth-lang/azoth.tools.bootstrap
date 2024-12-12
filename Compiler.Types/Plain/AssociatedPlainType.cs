using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Plain;

[Closed(
    typeof(OrdinaryAssociatedPlainType),
    typeof(SelfPlainType))]
public abstract class AssociatedPlainType : ConstructedOrAssociatedPlainType, BareType
{
    ConstructedOrAssociatedPlainType BareType.PlainType => this;
    public TypeConstructor ContainingType { get; }
    public sealed override TypeConstructor? TypeConstructor => null;
    public override TypeSemantics? Semantics => null;
    public sealed override bool AllowsVariance => false;
    public IFixedList<TypeParameterArgument> TypeParameterArguments => [];
    internal override PlainTypeReplacements TypeReplacements => PlainTypeReplacements.None;

    protected AssociatedPlainType(TypeConstructor containingType)
    {
        ContainingType = containingType;
    }

    public CapabilityType With(Capability capability)
        => CapabilityType.Create(capability, this);

    public CapabilityType WithRead()
        => With(ContainingType.IsDeclaredConst ? Capability.Constant : Capability.Read);

    public CapabilityType WithMutate()
        => With(ContainingType.IsDeclaredConst ? Capability.Constant : Capability.Mutable);

    #region Equality
    public bool Equals(BareType? other)
        => ReferenceEquals(this, other)
           || other is AssociatedPlainType otherType && Equals((IMaybePlainType)otherType);
    #endregion

    public sealed override string ToString() => $"{ContainingType}.{Name}";

    string BareType.ToSourceCodeString() => ToString();

    string BareType.ToILString() => ToString();

    public sealed override string ToBareString() => ToString();
}
